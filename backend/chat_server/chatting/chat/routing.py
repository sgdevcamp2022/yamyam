from django.urls import re_path

from . import consumers

websocket_urlpatterns = [
    re_path(r"ws/lobby/(?P<user_id>\w+)/(?P<user_nickname>\w+)/$",
            consumers.SingleConsumer.as_asgi()),
]
