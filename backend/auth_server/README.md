# Auth Server

인증 서버

## Before Installation

#### Docker로 PostgreSQL, Redis를 설치해야 함

```
$ docker pull postgres:14.6-alpine
$ docker pull redis
```

#### postgreSQL 세팅 방법

```
$ docker run -p 5432:5432 --name postgres -e POSTGRES_PASSWORD=12345678 -d postgres:14.6-alpine
$ docker exec -it postgres /bin/bash
root@ac61c662ee4c:/# psql -U postgres
postgres=# create database nopoker;
CREATE DATABASE
postgres=# \connect nopoker;
You are now connected to database "nopoker" as user "postgres".
nopoker=# create user root with password '12345678';
CREATE ROLE
nopoker=# alter role root set client_encoding to 'utf-8';
ALTER ROLE
nopoker=# alter role root set timezone to 'Asia/Seoul';
ALTER ROLE
nopoker=# grant all privileges on database nopoker to root;
GRANT
```

#### Redis 세팅 방법

```
$ docker network create redis-net
$ docker run --name djangoredisserver -p 6379:6379 --network redis-net -d redis redis-server --appendonly yes
$ docker run -it --network redis-net --rm redis redis-cli -h djangoredisserver
```

#### docker로 posrgreSQL, Redis가 모두 컨테이너로 실행 중이어야 서버가 작동함

#### SECRET_KEY 등 서버의 중요 정보들을 secrets.json으로 관리 하고 있음

#### 따로 제공받은 secrets.json을 auth_server/ 경로에 배치해야 서버가 작동함

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
   $ python manage.py migrate
   $ python manage.py runserver
   ```
7. http://127.0.0.1:8000/docs/swagger/ 에서 API 문서를 확인할 수 있음
