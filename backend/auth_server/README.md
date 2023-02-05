# Auth Server

인증 서버

## Installation

1. Python 설치 (3.11.1)\
   https://www.python.org/downloads/ 에서 OS에 맞게 설치
2. Python 가상환경 설정\
   https://dojang.io/mod/page/view.php?id=2470 을 참고\
   python 3.11.1 을 사용하는 가상환경을 만들어야 함
3. 설정한 가상환경에 접속 후, 터미널에서 auth_server/requirements/으로 이동
4. pip로 requirements 설치
   ```
   $ pip install -r requirements.txt
   ```
5. 터미널에서 auth_server/ 로 이동
6. 서버 실행
   ```
   $ python manage.py runserver
   ```
7. http://127.0.0.1:8000/docs/swagger/ 에서 API 문서를 확인할 수 있음
