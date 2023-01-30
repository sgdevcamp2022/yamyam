import hashlib


def make_account_activate_token(nickname):
    return hashlib.sha256(nickname.encode('utf-8')).hexdigest()


def check_account_activate_token(nickname, token):
    if hashlib.sha256(nickname.encode('utf-8')).hexdigest() == token:
        return True
    else:
        return False
