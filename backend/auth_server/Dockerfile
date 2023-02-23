FROM python:3.11.1-alpine

WORKDIR /usr/src/auth

ENV PYTHONDONTWRITEBYTECODE 1
ENV PYTHONUNBUFFERED 1

RUN apk update && apk add postgresql-dev gcc python3-dev musl-dev

RUN pip install --upgrade pip
COPY /requirements/requirements.txt .
RUN pip install -r requirements.txt

COPY ./entrypoint.sh .
RUN sed -i 's/\r$//g' /usr/src/auth/entrypoint.sh
RUN chmod +x /usr/src/auth/entrypoint.sh

COPY . .

ENTRYPOINT [ "/usr/src/auth/entrypoint.sh" ]