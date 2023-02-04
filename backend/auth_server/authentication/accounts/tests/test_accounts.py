import re
import json
import hashlib
import datetime

from django.core import mail
from django.urls import reverse
from django.test import TestCase
from django.utils.encoding import force_bytes
from django.utils.http import urlsafe_base64_encode
from django.contrib.auth.hashers import make_password

from authentication.accounts.models import User


class AccountCreateTests(TestCase):
    def setUp(self):
        self.create_account_url = reverse('accounts:create_account')

    def test_create_account(self):
        post = {"username": "user2", "nickname": "nickname2", "email": "user2@example.com",
                "password": "password2"}
        response = self.client.post(self.create_account_url, post)
        data = json.loads(response.content)
        self.assertEqual(response.status_code, 201)
        content = {"username": "user2", "nickname": "nickname2",
                   "email": "user2@example.com"}
        self.assertEqual(data, content)


class AccountsTests(TestCase):
    def setUp(self):
        User.objects.get_or_create(id=1,
                                   username="user1", nickname="nickname1", email="user1@example.com", password=make_password('password1'))
        self.create_account_url = reverse('accounts:create_account')
        self.get_activate_url = reverse('accounts:activate_account', kwargs={
                                        "uidb64": urlsafe_base64_encode(force_bytes(1)), "token": hashlib.sha256('nickname1'.encode('utf-8')).hexdigest()})
        self.login_account_url = reverse('accounts:login_account')
        self.check_access_token_url = reverse('accounts:check_access_token')
        self.logout_account_url = reverse('accounts:logout_account')
        self.find_username_url = reverse('accounts:find_username')
        self.password_reset_url = reverse('accounts:password_reset')
        self.password_reset_valid_url = reverse('accounts:password_reset_confirm', kwargs={
            "uidb64": urlsafe_base64_encode(force_bytes(1)), "token": "set-password"
        })
        self.handle_account_url = reverse('accounts:handle_account', kwargs={
            "id": 1
        })

    def test_create_duplicated_username_account(self):
        post = {"username": "user1", "nickname": "nickname2", "email": "user2@example.com",
                "password": "password2"}
        response = self.client.post(self.create_account_url, post)
        data = json.loads(response.content)
        self.assertEqual(response.status_code, 400)
        content = {"username": ['user with this username already exists.']}
        self.assertEqual(data, content)

    def test_create_duplicated_nickname_account(self):
        post = {"username": "user2", "nickname": "nickname1", "email": "user2@example.com",
                "password": "password2"}
        response = self.client.post(self.create_account_url, post)
        data = json.loads(response.content)
        self.assertEqual(response.status_code, 400)
        content = {"nickname": ['user with this nickname already exists.']}
        self.assertEqual(data, content)

    def test_create_duplicated_email_account(self):
        post = {"username": "user2", "nickname": "nickname2", "email": "user1@example.com",
                "password": "password2"}
        response = self.client.post(self.create_account_url, post)
        data = json.loads(response.content)
        self.assertEqual(response.status_code, 400)
        content = {"email": ['user with this email already exists.']}
        self.assertEqual(data, content)

    def test_activate_account(self):
        response = self.client.get(self.get_activate_url)
        self.assertTemplateUsed(
            response, 'accounts/accounts_register_success.html')

    def test_login_account(self):
        post = {"username": "user1", "password": "password1"}
        response = self.client.post(self.login_account_url, post)
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.has_header('Access-Token'), True)
        self.assertEqual(response.has_header('Refresh-Token'), True)

    def test_check_access_token(self):
        post = {"username": "user1", "password": "password1"}
        response = self.client.post(self.login_account_url, post)
        header = {
            'HTTP_ACCESS_TOKEN': response['Access-Token'],
            'HTTP_REFRESH_TOKEN': response['Refresh-Token']
        }
        response = self.client.get(self.check_access_token_url, **header)
        data = json.loads(response.content)
        content = {"username": "user1"}
        self.assertEqual(response.status_code, 200)
        self.assertEqual(data, content)

    # def test_check_refresh_token(self):

    def test_logout_account(self):
        post = {"username": "user1", "password": "password1"}
        response = self.client.post(self.login_account_url, post)
        header = {
            'HTTP_ACCESS_TOKEN': response['Access-Token'],
            'HTTP_REFRESH_TOKEN': response['Refresh-Token']
        }
        response = self.client.post(self.logout_account_url, **header)
        self.assertEqual(response.status_code, 200)

    def test_find_username(self):
        post = {"email": "user1@example.com"}
        response = self.client.post(self.find_username_url, post)
        self.assertEqual(response.status_code, 200)

    def test_password_reset(self):
        post = {"username": "user1", "email": "user1@example.com"}
        response = self.client.post(self.password_reset_url, post)
        self.assertEqual(response.status_code, 200)
        self.assertEqual(len(mail.outbox), 1)
        self.assertEqual(mail.outbox[0].subject,
                         'NoPOKER Password reset')
        urlmatch = re.search(
            r"http?://[^/]*(/.*reset/\S*)", mail.outbox[0].body)
        response = self.client.get(urlmatch[1])
        self.assertEqual(response.status_code, 302)
        urlmatch_list = urlmatch[1].split('/')
        session = self.client.session
        session["_password_reset_token"] = urlmatch_list[4]
        session.save()
        response = self.client.post(
            self.password_reset_valid_url, {"new_password1": "password12345", "new_password2": "password12345"})
        self.assertTemplateUsed(
            response, 'accounts/accounts_password_complete.html')
        u = User.objects.get(email="user1@example.com")
        self.assertTrue(u.check_password("password12345"))

    def test_handle_account_delete_integrate(self):
        response = self.client.delete(self.handle_account_url)
        self.assertEqual(response.status_code, 200)
        self.assertEqual(len(mail.outbox), 1)
        self.assertEqual(mail.outbox[0].subject,
                         'Withdraw Your NoPOKER Account')
        urlmatch = re.search(
            r"http?://[^/]*(/.*withdraw/\S*)", mail.outbox[0].body)
        response = self.client.get(urlmatch[0])
        self.assertTemplateUsed(
            response, 'accounts/accounts_withdraw_success.html')
        with self.assertRaises(User.DoesNotExist):
            user = User.objects.get(pk=1)

    def test_handle_account_get(self):
        response = self.client.get(self.handle_account_url)
        self.assertEqual(response.status_code, 200)
        data = json.loads(response.content)
        content = {'nickname': 'nickname1', 'victory': 0, 'loose': 0,
                   'date_joined': str(datetime.datetime.now())}
        # self.assertEqual(content, data)
