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
from drf_spectacular.utils import extend_schema, OpenApiParameter, OpenApiResponse, OpenApiExample, inline_serializer
from drf_spectacular.types import OpenApiTypes
from rest_framework import generics, status, serializers
from rest_framework.views import APIView
from rest_framework.response import Response

from .models import User
from .forms import SetPasswordForm
from .serializers import CreateUserSerializer, RetreiveUserSerializer, ResetPasswordSerializer
from .hash import check_account_activate_token, make_token
from .utils import issue_token
from config.base import SECRET_KEY, ALGORITHM, SITE_URL


class CreateAccount(generics.GenericAPIView):
    serializer_class = CreateUserSerializer

    @extend_schema(
        summary="사용자를 DB에 등록, 등록을 확정하기 위한 이메일을 보냄.",
        request=CreateUserSerializer,
        responses={
            status.HTTP_201_CREATED: OpenApiResponse(
                response=CreateUserSerializer,
                description="User created."
            ),
            status.HTTP_400_BAD_REQUEST: OpenApiResponse(
                response=None,
                description="Failed to create user."
            )
        },
        examples=[
            OpenApiExample(
                request_only=True,
                summary="Request Body Example입니다.",
                name="success_example",
                value={
                    "username": "user1",
                    "nickname": "nickname1",
                    "email": "user1@example.com",
                    "password": "password1",
                },
            ),
            OpenApiExample(
                response_only=True,
                summary="Response Body Example입니다.",
                name="success_example",
                value={
                    "username": "user1",
                    "nickname": "nickname1",
                    "email": "user1@example.com",
                }
            )
        ]
    )
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
    @extend_schema(
        summary="사용자가 로그인 할 수 있음",
        request=inline_serializer(
            "login", {"username": serializers.CharField(), "password": serializers.CharField()}),
        responses={
            status.HTTP_200_OK: OpenApiResponse(
                response=None,
                description="Login successfully, and Access Token and Refresh Token are issued at Header."
            ),
            status.HTTP_404_NOT_FOUND: OpenApiResponse(
                response=None,
                description="Login failed."
            )
        },
        parameters=[
            OpenApiParameter(
                name="Access-Token",
                description="Access Token입니다. 200일때만 저장됩니다.",
                type=str,
                location=OpenApiParameter.HEADER,
                response=True
            ),
            OpenApiParameter(
                name="Refresh-Token",
                description="Refresh Token입니다. 200일때만 저장됩니다.",
                type=str,
                location=OpenApiParameter.HEADER,
                response=True
            ),
        ],
        examples=[
            OpenApiExample(
                request_only=True,
                name="success_example",
                value={
                    "username": "user1",
                    "password": "password1",
                }
            )
        ]
    )
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
            return Response(status=status.HTTP_404_NOT_FOUND)


class LogoutAccount(APIView):
    @extend_schema(
        summary="사용자가 로그아웃 할 수 있음, 게임을 종료할 때 이 API를 호출해야 함.",
        description="클라이언트에서 저장된 token들도 삭제해야 함",
        request=None,
        responses={
            status.HTTP_200_OK: OpenApiResponse(
                response=None,
                description="Logout successfully, Refresh Token in DB is deleted."
            ),
            status.HTTP_404_NOT_FOUND: OpenApiResponse(
                response=None,
                description="Logout failed."
            )
        },
        parameters=[
            OpenApiParameter(
                name="Refresh-Token",
                description="Refresh Token입니다.",
                required=True,
                type=str,
                location=OpenApiParameter.HEADER
            )
        ],
    )
    def get(self, request):
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


class CheckAccessToken(APIView):
    @extend_schema(
        summary="Header에 있는 Access Token이 유효한지 점검",
        request=None,
        responses={
            status.HTTP_200_OK: OpenApiResponse(
                response=None,
                description="Access Token is valid."
            ),
            status.HTTP_401_UNAUTHORIZED: OpenApiResponse(
                response=None,
                description="Access Token is invalid."
            )
        },
        parameters=[
            OpenApiParameter(
                name="Access-Token",
                description="Access Token입니다.",
                required=True,
                type=str,
                location=OpenApiParameter.HEADER
            )
        ]
    )
    def get(self, request):
        try:
            access_token = jwt.decode(
                request.headers['Access-Token'], SECRET_KEY, ALGORITHM)
            return Response(status=status.HTTP_200_OK)
        except (jwt.exceptions.InvalidTokenError):
            return Response(status=status.HTTP_401_UNAUTHORIZED)


class CheckRefreshToken(APIView):
    @extend_schema(
        summary="Header에 있는 Refersh Token이 유효한지 점검, 유효하다면 Access Token과 Refresh Token 재발급",
        request=None,
        responses={
            status.HTTP_200_OK: OpenApiResponse(
                description="Since the refresh token is valid, new access tokens and refresh tokens are issued.",
            ),
            status.HTTP_401_UNAUTHORIZED: OpenApiResponse(
                description="Refresh Token is invalid.",
            )
        },
        parameters=[
            OpenApiParameter(
                name="Refresh-Token",
                description="Refresh Token입니다.",
                required=True,
                type=str,
                location=OpenApiParameter.HEADER,
            ),
            OpenApiParameter(
                name="Access-Token",
                description="Access Token입니다. 200일때만 저장됩니다.",
                type=str,
                location=OpenApiParameter.HEADER,
                response=True
            ),
            OpenApiParameter(
                name="Refresh-Token",
                description="Refresh Token입니다. 200일때만 저장됩니다.",
                type=str,
                location=OpenApiParameter.HEADER,
                response=True
            ),
        ]
    )
    def get(self, request):
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
    @extend_schema(
        summary="사용자에게 username을 찾을 수 있는 이메일을 보낼 수 있음",
        request=inline_serializer(
            "email", {"email": serializers.EmailField()}),
        responses={
            status.HTTP_200_OK: OpenApiResponse(
                response=None,
                description="Email a withdraw mail to user successfully."
            ),
            status.HTTP_404_NOT_FOUND: OpenApiResponse(
                response=None,
                description="Invalid user."
            )
        },
    )
    def post(self, request):
        user = get_object_or_404(User, email=request.data.get("email"))
        message = render_to_string('accounts/accounts_find_username.html', {
            'username': user.username,
        })
        user.email_user('Here is your NoPOKER ID', message)
        return Response(status=status.HTTP_200_OK)


class PasswordReset(APIView):
    @extend_schema(
        summary="사용자의 비밀번호를 재생성할 수 있는 링크를 이메일로 보냄",
        request=ResetPasswordSerializer,
        responses={
            status.HTTP_200_OK: OpenApiResponse(
                response=None,
                description="Password reset email sent to user successfully."
            ),
            status.HTTP_404_NOT_FOUND: OpenApiResponse(
                response=None,
                description="Failed to send email."
            )
        },
        examples=[
            OpenApiExample(
                request_only=True,
                summary="Request Body Example입니다.",
                name="success_example",
                value={
                    "username": "user1",
                    "email": "user1@example.com",
                },
            ),
        ]
    )
    def post(self, request):
        serializer = ResetPasswordSerializer(data=request.data)
        if serializer.is_valid():
            username = serializer.validated_data['username']
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
            return Response(status=status.HTTP_404_NOT_FOUND)


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


class HandleAccount(generics.GenericAPIView):
    queryset = User.objects.all()
    serializer_class = RetreiveUserSerializer

    @extend_schema(
        summary="마이페이지에서 사용자의 정보를 받을 수 있음",
        request=None,
        responses={
            status.HTTP_200_OK: OpenApiResponse(
                response=RetreiveUserSerializer,
                description="Find user successfully."
            ),
            status.HTTP_404_NOT_FOUND: OpenApiResponse(
                response=None,
                description="Failed to get user."
            )
        },
        parameters=[
            OpenApiParameter(
                name="id",
                description="user의 id입니다.",
                required=True,
                type=OpenApiTypes.STR,
                location=OpenApiParameter.PATH
            )
        ],
        examples=[
            OpenApiExample(
                response_only=True,
                summary="Response Body Example입니다.",
                name="success_example",
                value={
                    "nickname": "nickname1",
                    "victory": 5,
                    "loose": 3,
                    "date_joined": "2023-02-05T05:37:14.096Z",
                }
            )
        ]
    )
    def get(self, request, id, *args, **kwargs):
        user = get_object_or_404(User, pk=id)
        serializer = RetreiveUserSerializer(user)
        return Response(serializer.data, status=status.HTTP_200_OK)

    @extend_schema(
        summary="사용자에게 회원탈퇴 이메일을 보낼 수 있음",
        request=None,
        responses={
            status.HTTP_200_OK: OpenApiResponse(
                response=None,
                description="Email a withdraw mail to user successfully."
            ),
            status.HTTP_404_NOT_FOUND: OpenApiResponse(
                response=None,
                description="Failed to delete user."
            )
        },
    )
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
