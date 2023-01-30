import jwt

from django.shortcuts import render, get_object_or_404
from django.views.generic.base import View
from django.utils.encoding import force_str
from django.utils.http import urlsafe_base64_decode
from django.core.cache import cache
from rest_framework import generics, status
from rest_framework.views import APIView
from rest_framework.response import Response

from .models import User
from .serializers import CreateUserSerializer
from .hash import check_account_activate_token
from .utils import issue_token
from config.base import SECRET_KEY, ALGORITHM


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
                user.is_active = True
                user.save()
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
