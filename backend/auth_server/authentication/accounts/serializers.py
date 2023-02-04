from django.template.loader import render_to_string
from django.utils.encoding import force_bytes
from django.utils.http import urlsafe_base64_encode
from rest_framework import serializers

from .models import User
from .hash import make_token
from config.base import SITE_URL


class CreateUserSerializer(serializers.ModelSerializer):
    class Meta:
        model = User
        fields = ['username', 'nickname', 'email', 'password']
        extra_kwargs = {'password': {'write_only': True}}

    def create(self, validated_data):
        user = User(
            username=validated_data['username'],
            nickname=validated_data['nickname'],
            email=validated_data['email']
        )
        user.set_password(validated_data['password'])
        user.save()
        message = render_to_string('accounts/accounts_register_email.html', {
            'username': user.username,
            'domain': SITE_URL,
            'uid': urlsafe_base64_encode(force_bytes(user.pk)),
            'token': make_token(user.nickname),
        })
        user.email_user('Activate Your NoPOKER Account', message)
        return user


class ResetPasswordSerializer(serializers.Serializer):
    username = serializers.CharField(max_length=150)
    email = serializers.EmailField(max_length=255)

    def validate_username(self, value):
        if not User.objects.filter(username=value).exists():
            raise serializers.ValidationError("해당 사용자ID가 존재하지 않습니다.")
        return value

    def validate(self, data):
        username = data['username']
        email = data['email']

        if User.objects.get(username=username).email != email:
            raise serializers.ValidationError("사용자의 이메일 주소가 일치하지 않습니다")
        return data


class RetreiveUserSerializer(serializers.ModelSerializer):
    class Meta:
        model = User
        fields = ['nickname', 'victory', 'loose', 'date_joined']
