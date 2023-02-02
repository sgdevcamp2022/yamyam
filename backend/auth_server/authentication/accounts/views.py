import jwt

from django.contrib.auth.tokens import default_token_generator
from django.core.exceptions import ImproperlyConfigured, ValidationError
from django.shortcuts import render, get_object_or_404
from django.views.generic.base import View
from django.utils.encoding import force_str, force_bytes
from django.utils.http import urlsafe_base64_decode, urlsafe_base64_encode
from django.core.cache import cache
from django.http import HttpResponseRedirect
from django.template.loader import render_to_string
from rest_framework import generics, status
from rest_framework.views import APIView
from rest_framework.response import Response

from .models import User
from .forms import PasswordResetForm, SetPasswordForm
from .serializers import CreateUserSerializer, RetreiveUserSerializer
from .hash import check_account_activate_token, make_token
from .utils import issue_token
from config.base import SECRET_KEY, ALGORITHM, SITE_URL


class CreateAccount(generics.GenericAPIView):
    serializer_class = CreateUserSerializer

    def post(self, request, format=None):
        serializer = self.get_serializer(
            data=request.data, context={'request': request})
        serializer.is_valid(raise_exception=True)
        serializer.save()
        return Response(
            serializer.data,
            status=status.HTTP_201_CREATED
        )


class ActivateAccount(View):
    def get(self, request, uidb64, token, *args, **kwargs):
        try:
            uid = force_str(urlsafe_base64_decode(uidb64))
            user = User.objects.get(pk=uid)
        except (TypeError, ValueError, OverflowError, User.DoesNotExist):
            user = None

        if user is not None and user.is_active == False:
            if check_account_activate_token(user.nickname, token):
                user.activate()
                return render(request, 'accounts/accounts_register_success.html')
        return render(request, 'accounts/accounts_register_invalid.html')


class LoginAccount(APIView):
    def post(self, request):
        user = get_object_or_404(User, username=request.data.get("username"))
        username = user.username
        if user.check_password(request.data.get('password')):
            access_token = issue_token(username, days=0, hours=6)
            refresh_token = issue_token(username, days=14, hours=0)
            if cache.get(username) is None:
                cache.set(username, refresh_token, 60*60*24*14)
            else:
                cache.delete(username)
                cache.set(username, refresh_token, 60*60*24*14)
            return Response(status=status.HTTP_200_OK,
                            headers={
                                'Access-Token': access_token,
                                'Refresh-Token': refresh_token
                            })
        else:
            return Response({'detail': 'invalid user'}, status=status.HTTP_401_UNAUTHORIZED)


class LogoutAccount(APIView):
    def post(self, request):
        try:
            refresh_token = jwt.decode(
                request.headers['Refresh-Token'], SECRET_KEY, ALGORITHM)
            username = refresh_token.get('username')
            if cache.get(username) is None:
                return Response(status=status.HTTP_404_NOT_FOUND)
            else:
                cache.delete(username)
                return Response(status=status.HTTP_200_OK)
        except (jwt.exceptions.InvalidTokenError):
            return Response(status=status.HTTP_404_NOT_FOUND)


class CheckToken(APIView):
    def post(self, request):
        try:
            access_token = jwt.decode(
                request.headers['Access-Token'], SECRET_KEY, ALGORITHM)
            user = get_object_or_404(
                User, username=access_token.get('username'))
            return Response({'username': user.username}, status=status.HTTP_200_OK)
        except (jwt.exceptions.DecodeError, jwt.exceptions.InvalidAlgorithmError,
                jwt.exceptions.InvalidKeyError, jwt.exceptions.MissingRequiredClaimError):
            return Response(status=status.HTTP_401_UNAUTHORIZED)
        except (jwt.exceptions.ExpiredSignatureError):
            try:
                refresh_token = jwt.decode(
                    request.headers['Refresh-Token'], SECRET_KEY, ALGORITHM)
                username = refresh_token.get('username')
                if cache.get(username) is None:
                    return Response(status=status.HTTP_401_UNAUTHORIZED)
                else:
                    access_token = issue_token(username, days=0, hours=6)
                    refresh_token = issue_token(username, days=14, hours=0)
                    cache.delete(username)
                    cache.set(username, refresh_token, 60*60*24*14)
                    return Response(status=status.HTTP_200_OK,
                                    headers={
                                        'Access-Token': access_token,
                                        'Refresh-Token': refresh_token
                                    })
            except (jwt.exceptions.InvalidTokenError):
                return Response(status=status.HTTP_401_UNAUTHORIZED)


class FindUsername(APIView):
    def post(self, request):
        user = get_object_or_404(User, email=request.data.get("email"))
        message = render_to_string('accounts/accounts_find_username.html', {
            'username': user.username,
        })
        user.email_user('Here is your NoPOKER ID', message)
        return Response(status=status.HTTP_200_OK)


class PasswordReset(APIView):
    def post(self, request):
        password_reset_form = PasswordResetForm(request.POST)
        if password_reset_form.is_valid():
            username = password_reset_form.cleaned_data['username']
            user = get_object_or_404(User, username=username)
            message = render_to_string('accounts/accounts_password_reset.html', {
                'username': username,
                'domain': SITE_URL,
                'uid': urlsafe_base64_encode(force_bytes(user.pk)),
                'token': default_token_generator.make_token(user),
            })
            user.email_user('NoPOKER Password reset', message)
            return Response(status=status.HTTP_200_OK)
        else:
            return Response(status=status.HTTP_400_BAD_REQUEST)


INTERNAL_RESET_SESSION_TOKEN = "_password_reset_token"


class PasswordResetConfirm(View):
    reset_url_token = "set-password"

    def dispatch(self, request, *args, **kwargs):
        if "uidb64" not in kwargs or "token" not in kwargs:
            raise ImproperlyConfigured(
                "The URL path must contain 'uidb64' and 'token' parameters."
            )
        self.validlink = False
        self.user = self.get_user(kwargs["uidb64"])
        if self.user is not None:
            token = kwargs["token"]
            if token == self.reset_url_token:
                session_token = self.request.session.get(
                    INTERNAL_RESET_SESSION_TOKEN)
                if default_token_generator.check_token(self.user, session_token):
                    self.validlink = True
                    return super().dispatch(request, *args, **kwargs)
            else:
                if default_token_generator.check_token(self.user, token):
                    self.request.session[INTERNAL_RESET_SESSION_TOKEN] = token
                    redirect_url = self.request.path.replace(
                        token, self.reset_url_token
                    )
                    return HttpResponseRedirect(redirect_url)
        return super().dispatch(request, *args, **kwargs)

    def get(self, request, *args, **kwargs):
        return render(request, "accounts/accounts_password_confirm.html", {'validlink': self.validlink})

    def post(self, request, *args, **kwargs):
        if self.validlink:
            set_password_form = SetPasswordForm(
                data=request.POST, user=self.user)
            if set_password_form.is_valid():
                set_password_form.save()
                del self.request.session[INTERNAL_RESET_SESSION_TOKEN]
                return render(request, "accounts/accounts_password_complete.html")
            else:
                return render(request, "accounts/accounts_password_reset_failed.html")
        else:
            return render(request, "accounts/accounts_password_confirm.html", {'validlink': self.validlink})

    def get_user(self, uidb64):
        try:
            uid = force_str(urlsafe_base64_decode(uidb64))
            user = User.objects.get(pk=uid)
        except (
            TypeError,
            ValueError,
            OverflowError,
            User.DoesNotExist,
            ValidationError
        ):
            user = None
        return user


class HandleAccount(APIView):
    def get(self, request, id, *args, **kwargs):
        user = get_object_or_404(User, pk=id)
        serializer = RetreiveUserSerializer(user)
        return Response(serializer.data, status=status.HTTP_200_OK)

    def delete(self, request, id, *args, **kwargs):
        user = get_object_or_404(User, pk=id)
        message = render_to_string('accounts/accounts_withdraw_email.html', {
            'username': user.username,
            'domain': SITE_URL,
            'uid': urlsafe_base64_encode(force_bytes(user.pk)),
            'token': make_token(user.nickname),
        })
        user.email_user('Withdraw Your NoPOKER Account', message)
        return Response(status=status.HTTP_200_OK)


class WithdrawAccount(View):
    def get(self, request, uidb64, token, *args, **kwargs):
        try:
            uid = force_str(urlsafe_base64_decode(uidb64))
            user = User.objects.get(pk=uid)
        except (TypeError, ValueError, OverflowError, User.DoesNotExist):
            user = None

        if user is not None:
            if check_account_activate_token(user.nickname, token):
                user.delete()
                return render(request, 'accounts/accounts_withdraw_success.html')
        return render(request, 'accounts/accounts_withdraw_invalid.html')
