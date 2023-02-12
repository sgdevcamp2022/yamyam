from django.urls import re_path

from . import consumers

websocket_urlpatterns = [
    re_path(r"ws/single/(?P<user_id>\w+)/(?P<user_nickname>\w+)/$",
            consumers.SingleConsumer.as_asgi()),
    re_path(r"ws/team/(?P<team_leader_id>\w+)/$",
            consumers.TeamConsumer.as_asgi()),
]
