from django.db import models
from django.core.mail import send_mail
from django.contrib.auth.models import (
    BaseUserManager, AbstractBaseUser
)
from django.utils import timezone

'''
username, password, nickname, email, status(대체), victory, loose, created_at, modified_at, accessed_at
AbstractBaseUser의 default attribute:
password, last_login, is_active
'''


class UserManager(BaseUserManager):
    def create_user(self, username, nickname, email, password=None):
        if not username:
            raise ValueError('Users must have an username')
        if not email:
            raise ValueError('Users must have an email address')
        if not nickname:
            raise ValueError('Users must have an nickname')

        user = self.model(username=username, nickname=nickname, email=email)
        user.is_active = True
        user.set_password(password)
        user.save(using=self._db)
        return user

    def create_superuser(self, username, nickname, email, password=None):
        user = self.create_user(
            username=username, nickname=nickname, email=email, password=password)
        user.is_active = True
        user.is_admin = True
        user.save(using=self._db)
        return user


class User(AbstractBaseUser):
    username = models.CharField(max_length=150, unique=True)
    nickname = models.CharField(max_length=150, unique=True)
    email = models.EmailField(max_length=255, unique=True)
    is_active = models.BooleanField(default=False)
    is_admin = models.BooleanField(default=False)
    victory = models.PositiveIntegerField(default=0)
    loose = models.PositiveIntegerField(default=0)
    report = models.PositiveSmallIntegerField(default=0)
    date_joined = models.DateTimeField(default=timezone.now)

    objects = UserManager()

    USERNAME_FIELD = 'username'
    REQUIRED_FIELDS = ['nickname', 'email']

    def __str__(self):
        return self.username

    def has_perm(self, perm, obj=None):
        return self.is_admin

    def has_module_perms(self, app_label):
        return self.is_admin

    def email_user(self, subject, message, from_email=None, **kwargs):
        send_mail(subject, message, from_email, [self.email], **kwargs)

    def activate(self):
        self.is_active = True
        self.save()

    def winner(self):
        self.victory += 1
        self.save()

    def looser(self):
        self.loose += 1
        self.save()

    @property
    def is_staff(self):
        return self.is_admin
