import json
import hashlib

from django.urls import reverse
from django.test import TestCase
from django.utils.encoding import force_bytes
from django.utils.http import urlsafe_base64_encode
from django.contrib.auth.hashers import make_password

from authentication.accounts.models import User


class AccountsTests(TestCase):
    def setUp(self):
        User.objects.get_or_create(
            username="user1", nickname="nickname1", email="user1@example.com", password=make_password('password1'))
        self.create_account_url = reverse('accounts:create_account')
        self.get_activate_url = reverse('accounts:activate_account', kwargs={
                                        "uidb64": urlsafe_base64_encode(force_bytes(1)), "token": hashlib.sha256('nickname1'.encode('utf-8')).hexdigest()})
        self.login_account_url = reverse('accounts:login_account')
        self.check_token_url = reverse('accounts:check_token')

    def test_create_account(self):
        post = {"username": "user2", "nickname": "nickname2", "email": "user2@example.com",
                "password": "password2"}
        response = self.client.post(self.create_account_url, post)
        data = json.loads(response.content)
        self.assertEquals(response.status_code, 201)
        content = {"username": "user2", "nickname": "nickname2",
                   "email": "user2@example.com"}
        self.assertEquals(data, content)

    def test_create_duplicated_username_account(self):
        post = {"username": "user1", "nickname": "nickname2", "email": "user2@example.com",
                "password": "password2"}
        response = self.client.post(self.create_account_url, post)
        data = json.loads(response.content)
        self.assertEquals(response.status_code, 400)
        content = {"username": ['user with this username already exists.']}
        self.assertEquals(data, content)

    def test_create_duplicated_nickname_account(self):
        post = {"username": "user2", "nickname": "nickname1", "email": "user2@example.com",
                "password": "password2"}
        response = self.client.post(self.create_account_url, post)
        data = json.loads(response.content)
        self.assertEquals(response.status_code, 400)
        content = {"nickname": ['user with this nickname already exists.']}
        self.assertEquals(data, content)

    def test_create_duplicated_email_account(self):
        post = {"username": "user2", "nickname": "nickname2", "email": "user1@example.com",
                "password": "password2"}
        response = self.client.post(self.create_account_url, post)
        data = json.loads(response.content)
        self.assertEquals(response.status_code, 400)
        content = {"email": ['user with this email already exists.']}
        self.assertEquals(data, content)

    def test_activate_account(self):
        response = self.client.get(self.get_activate_url)
        self.assertTemplateUsed(
            response, 'accounts/accounts_register_success.html')

    def test_login_account(self):
        post = {"username": "user1", "password": "password1"}
        response = self.client.post(self.login_account_url, post)
        self.assertEquals(response.status_code, 200)
        self.assertEquals(response.has_header('Access-Token'), True)
        self.assertEquals(response.has_header('Refresh-Token'), True)

    def test_check_token(self):
        post = {"username": "user1", "password": "password1"}
        response = self.client.post(self.login_account_url, post)
        header = {
            'HTTP_ACCESS_TOKEN': response['Access-Token'],
            'HTTP_REFRESH_TOKEN': response['Refresh-Token']
        }
        response = self.client.post(self.check_token_url, **header)
        self.assertEquals(response.status_code, 200)
