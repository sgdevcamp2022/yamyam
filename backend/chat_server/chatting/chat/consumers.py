import json

from asgiref.sync import async_to_sync
from channels.generic.websocket import JsonWebsocketConsumer

from .models import Room, User


class SingleConsumer(JsonWebsocketConsumer):
    has_team = False

    def new_message(self, data):
        # produce log in here.
        content = {
            'type': 'new_message',
            'nickname': self.user_nickname,
            'message': data['message'],
        }
        self.send_new_message(content)

    types = {
        'new_message': new_message,
    }

    def connect(self):
        self.user_id = self.scope["url_route"]["kwargs"]["user_id"]
        self.user_nickname = self.scope["url_route"]["kwargs"]["user_nickname"]
        self.notification_group_name = "notification_%s" % self.user_nickname
        self.lobby = Room.objects.get(name="lobby")
        self.user = User.objects.get_or_create(
            id=self.user_id, nickname=self.user_nickname)

        async_to_sync(self.channel_layer.group_add)(
            self.notification_group_name, self.channel_name
        )
        async_to_sync(self.channel_layer.group_add)(
            "lobby", self.channel_name
        )

        self.send_json(
            {
                "type": "user_list",
                "users": [user.nickname for user in self.lobby.online.all()]
            }
        )
        async_to_sync(self.channel_layer.group_send)(
            "lobby",
            {
                "type": "user_join",
                "user_nickname": self.user_nickname,
            }
        )
        self.lobby.online.add(self.user)

        self.accept()

    def disconnect(self, close_code):
        async_to_sync(self.channel_layer.group_discard)(
            self.notification_group_name, self.channel_name
        )
        async_to_sync(self.channel_layer.group_discard)(
            "lobby", self.channel_name
        )
        async_to_sync(self.channel_layer.group_send)(
            "lobby",
            {
                "type": "user_leave",
                "user_nickname": self.user_nickname
            }
        )
        self.lobby.online.remove(self.user)

    def receive(self, text_data):
        data = json.loads(text_data)
        self.types[data['type']](self, data)

    def send_new_message(self, message):
        async_to_sync(self.channel_layer.group_send)(
            "lobby",
            {
                "type": "chat_message",
                "message": message,
            }
        )

    def chat_message(self, event):
        message = event["message"]
        self.send(text_data=json.dumps(message))

    def notification_team_invitation(self, event):
        if self.has_team == False:
            self.send_json(event)
        else:
            pass

    def user_join(self, event):
        self.send_json(event)

    def user_leave(self, event):
        self.send_json(event)


class TeamConsumer(JsonWebsocketConsumer):
    pass
