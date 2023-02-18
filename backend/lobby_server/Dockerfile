FROM python:3.11.1-alpine

WORKDIR /usr/src/lobby

ENV PYTHONDONTWRITEBYTECODE 1
ENV PYTHONUNBUFFERED 1

RUN apk update && apk add postgresql-dev gcc python3-dev musl-dev libffi-dev

RUN pip install --upgrade pip
COPY /requirements/requirements.txt .
RUN pip install -r requirements.txt

COPY . .