import hashlib


def make_token(content):
    return hashlib.sha256(content.encode('utf-8')).hexdigest()


def check_account_activate_token(nickname, token):
    if hashlib.sha256(nickname.encode('utf-8')).hexdigest() == token:
        return True
    else:
        return False
