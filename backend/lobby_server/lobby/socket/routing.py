from django.urls import re_path
from django.urls import path

from . import consumers

websocket_urlpatterns = [
    path("ws/lobby/<int:user_id>/<str:user_nickname>",
         consumers.LobbyConsumer.as_asgi()),
]
