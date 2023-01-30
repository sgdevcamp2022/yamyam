import jwt
import datetime

from config.base import SECRET_KEY, ALGORITHM


def issue_token(username, days, hours):
    payload = {
        'username': username,
        'exp': datetime.datetime.utcnow() + datetime.timedelta(days=days, hours=hours),
    }
    return jwt.encode(payload, SECRET_KEY, ALGORITHM)
