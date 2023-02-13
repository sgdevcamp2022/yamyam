import json

from asgiref.sync import async_to_sync
from channels.generic.websocket import JsonWebsocketConsumer
from django.core.cache import caches


class SingleConsumer(JsonWebsocketConsumer):
    def lobby_message_send(self, data):
        # produce log here.
        async_to_sync(self.channel_layer.group_send)(
            "lobby",
            {
                'type': 'lobby_message',
                'id': data['id'],
                'nickname': data['nickname'],
                'message': data['message'],
            }
        )

    def team_message_send(self, data):
        async_to_sync(self.channel_layer.group_send)(
            "team_%s" % data['leader_id'],
            {
                'type': 'team_message',
                'id': data['id'],
                'nickname': data['nickname'],
                'message': data['message'],
            }
        )

    def invite_request_send(self, data):
        async_to_sync(self.channel_layer.group_send)(
            "notification_%s" % data['invitee']['id'],
            {
                'type': 'invite_request',
                'inviter': {
                    'id': data['inviter']['id'],
                    'nickname': data['inviter']['nickname'],
                },
                'invitee': {
                    'id': data['invitee']['id'],
                    'nickname': data['invitee']['nickname'],
                }
            }
        )

    def invite_accept_send(self, data):
        async_to_sync(self.channel_layer.group_send)(
            "notification_%s" % data['invitee']['id'],
            {
                'type': 'invitee_accept',
                'inviter': {
                    'id': data['inviter']['id'],
                    'nickname': data['inviter']['nickname'],
                },
                'invitee': {
                    'id': data['invitee']['id'],
                    'nickname': data['invitee']['nickname'],
                }
            }
        )
        async_to_sync(self.channel_layer.group_send)(
            "notification_%s" % data['inviter']['id'],
            {
                'type': 'inviter_accept',
                'inviter': {
                    'id': data['inviter']['id'],
                    'nickname': data['inviter']['nickname'],
                },
                'invitee': {
                    'id': data['invitee']['id'],
                    'nickname': data['invitee']['nickname'],
                }
            }
        )

    types = {
        'lobby_message': lobby_message_send,
        'team_message': team_message_send,
        'invite_request': invite_request_send,
        'invite_accept': invite_accept_send,
    }

    def connect(self):
        self.user_id = self.scope["url_route"]["kwargs"]["user_id"]
        self.user_nickname = self.scope["url_route"]["kwargs"]["user_nickname"]
        self.notification_group_name = "notification_%s" % self.user_id
        self.user_info = "%s_%s" % (self.user_id, self.user_nickname)

        async_to_sync(self.channel_layer.group_add)(
            self.notification_group_name, self.channel_name
        )
        async_to_sync(self.channel_layer.group_add)(
            "lobby", self.channel_name
        )

        self.send_json(
            {
                "type": "user_list",
                "users": caches['default'].get("*")
            }
        )
        async_to_sync(self.channel_layer.group_send)(
            "lobby",
            {
                "type": "user_join",
                "user_info": self.user_info,
            }
        )
        caches['default'].set(self.user_info, "")

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
                "user_info": self.user_info
            }
        )
        caches['default'].delete(self.user_info)
        # need to delete room redis too.

    def receive(self, text_data):
        data = json.loads(text_data)
        self.types[data['type']](self, data)

    def lobby_message(self, event):
        self.send_json(event)

    def team_message(self, event):
        self.send_json(event)

    def invite_request(self, event):
        if caches['default'].get("%s_%s" % (event["invitee"]["id"], event["invitee"]["nickname"])) == "":
            self.send_json(event)
        else:
            pass

    def invitee_accept(self, event):
        leader_id = event["inviter"]["id"]
        leader_nickname = event["inviter"]["nickname"]
        invitee_id = event["invitee"]["id"]
        invitee_nickname = event["invitee"]["nickname"]
        leader = "%s_%s" % (leader_id, leader_nickname)
        invitee = "%s_%s" % (invitee_id, invitee_nickname)
        if caches['default'].get(invitee) == "invitee":
            self.send_json({
                "type": "team_redundancy"
            })
            return
        if caches['default'].get(leader) == "invitee":
            self.send_json({
                "type": "team_redundancy"
            })
            return
        if caches['team'].get(leader) != None:
            team_json = json.loads(caches['team'].get(leader))
            if len(team_json["invitees"]) == 3:
                self.send_json({
                    "type": "team_excess"
                })
                return
            new_invitee_json = {
                "id": invitee_id,
                "nickname": invitee_nickname
            }
            team_invitees = team_json["invitees"].append(new_invitee_json)
            update = {
                "leader": {
                    'id': leader_id,
                    'nickname': leader_nickname,
                },
                "invitees": team_invitees,
            }
            caches['team'].set(leader, json.dumps(update))
            caches['default'].set(invitee, "invitee")
            async_to_sync(self.channel_layer.group_send)(
                "team_%s" % leader_id, {
                    "type": "team_list",
                    "leader": {
                        'id': leader_id,
                        'nickname': leader_nickname,
                    },
                    "invitees": team_invitees,
                }
            )
            return
        # inviter have no team, invitee have no team
        invitee_json_str = '["id": %s, "nickname: %s]' % (
            invitee_id, invitee_nickname)
        invitee_json = json.loads(invitee_json_str)
        self.send_json(
            {
                "type": "team_list",
                "leader": {
                    'id': leader_id,
                    'nickname': leader_nickname,
                },
                "invitees": invitee_json
            }
        )
        caches['default'].set(invitee, "invitee")
        async_to_sync(self.channel_layer.group_add)(
            "team_%s" % leader_id, self.channel_name
        )

    def inviter_accept(self, event):
        leader_id = event["inviter"]["id"]
        leader_nickname = event["inviter"]["nickname"]
        invitee_id = event["invitee"]["id"]
        invitee_nickname = event["invitee"]["nickname"]
        leader = "%s_%s" % (leader_id, leader_nickname)
        if caches['default'].get(leader) == "invitee":
            return
        if caches['team'].get(leader) != None:
            return
        # inviter have no team, invitee have no team
        invitee_json_str = '["id": %s, "nickname: %s]' % (
            invitee_id, invitee_nickname)
        invitee_json = json.loads(invitee_json_str)
        self.send_json(
            {
                "type": "team_list",
                "leader": {
                    'id': leader_id,
                    'nickname': leader_nickname,
                },
                "invitees": invitee_json
            }
        )
        caches['default'].set(leader, "leader")
        async_to_sync(self.channel_layer.group_add)(
            "team_%s" % leader_id, self.channel_name
        )

    def team_list(self, event):
        self.send_json(event)

    def user_join(self, event):
        self.send_json(event)

    def user_leave(self, event):
        self.send_json(event)
