# Backend Installation

docker-compose로 backend를 손쉽게 설치

## Before Installation

1. docker를 설치해야 함
2. 제공받은 .env 파일을 backend/ 디렉토리에 저장해야 함

- .env.auth.dev
- .env.auth.prod
- .env.auth.prod.db
- .env.lobby.dev
- .env.lobby.prod

## development version docker-compose

- nginx, gunicorn, daphne가 빠진 버전
- 인증 서비스 : 8000 포트
- 로비 서비스 : 8001 포트

#### 빌드

```
  $ docker-compose up -d --build
```

#### 다시 빌드하기 전에 down 명령어를 꼭 해야 함

```
  $ docker-compose down -v
```

## production version docker-compose

- development version에서 nginx, gunicorn, daphne가 추가 됨
- 모든 서비스가 1337 포트로 통신함

#### 빌드

```
$ docker-compose -f docker-compose.prod.yml up -d --build
$ docker-compose -f docker-compose.prod.yml exec auth python manage.py migrate --noinput
$ docker-compose -f docker-compose.prod.yml exec auth python manage.py collectstatic --no-input --clear
```

#### 다시 빌드하기 전에 down 명령어를 꼭 해야 함

```
$ docker-compose -f docker-compose.prod.yml down -v
```
