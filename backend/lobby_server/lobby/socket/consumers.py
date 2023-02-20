import json

from asgiref.sync import async_to_sync
from channels.generic.websocket import JsonWebsocketConsumer
from django.core.cache import caches


class LobbyConsumer(JsonWebsocketConsumer):
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
        invitee_status = caches['default'].get("%s_%s" % (
            data["invitee"]["id"], data["invitee"]["nickname"]))
        if invitee_status == "":
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
            return
        if invitee_status == "game":
            self.send_json(
                {
                    'type': 'invitee_playing',
                }
            )
            return
        if invitee_status == "invitee":
            self.send_json(
                {
                    'type': 'team_redundancy',
                }
            )
        self.send_json(
            {
                'type': 'invalid_request',
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

    def leader_exit_send(self, data):
        leader_id = data["leader"]["id"]
        leader = "%s_%s" % (leader_id, data["leader"]["nickname"])
        caches['default'].set(leader, "")
        caches['team'].delete(leader)
        invitees = data["invitees"]
        for invitee in invitees:
            temp = "%s_%s" % (invitee["id"], invitee["nickname"])
            caches['default'].set(temp, "")
        async_to_sync(self.channel_layer.group_send)(
            "team_%s" % leader_id,
            {
                'type': 'leader_exit',
                'leader_id': leader_id
            }
        )

    def invitee_exit_send(self, data):
        reqeuester_id = data["requester"]["id"]
        requester = "%s_%s" % (reqeuester_id, data["requester"]["nickname"])
        leader_id = data["leader"]["id"]
        leader_nickname = data["leader"]["nickname"]
        leader = "%s_%s" % (leader_id, data["leader"]["nickname"])
        caches['default'].set(requester, "")
        invitees = data["invitees"]
        for invitee in invitees[:]:
            if invitee["id"] == reqeuester_id:
                invitees.remove(invitee)
                break
        update = {
            "leader": {
                'id': leader_id,
                'nickname': leader_nickname,
            },
            "invitees": invitees,
        }
        async_to_sync(self.channel_layer.group_discard)(
            "team_%s" % leader_id, self.channel_name
        )
        async_to_sync(self.channel_layer.group_send)(
            "team_%s" % leader_id,
            {
                'type': 'invitee_exit',
                "leader": {
                        'id': leader_id,
                        'nickname': leader_nickname,
                },
                "invitees": invitees,
            }
        )
        if len(invitees) == 0:
            caches['team'].delete(leader)
            caches['default'].set(leader, "")
            async_to_sync(self.channel_layer.group_send)(
                "team_%s" % leader_id,
                {
                    'type': 'no_invitee',
                    'leader_id': leader_id
                }
            )
            return
        caches['team'].set(leader, json.dumps(update))

    def game_start_send(self, data):
        leader_id = data["leader"]["id"]
        leader = "%s_%s" % (leader_id, data["leader"]["nickname"])
        invitee = data["invitee"]
        caches['default'].set(leader, "game")
        if invitee == {}:
            return
        temp = "%s_%s" % (invitee["id"], invitee["nickname"])
        caches['default'].set(temp, "game")
        caches['team'].delete(leader)
        async_to_sync(self.channel_layer.group_discard)(
            "team_%s" % leader_id, self.channel_name
        )

    def game_exit_send(self, data):
        player = data["player"]
        caches['default'].set("%s_%s" % (player["id"], player["nickname"]), "")

    types = {
        'lobby_message': lobby_message_send,
        'team_message': team_message_send,
        'invite_request': invite_request_send,
        'invite_accept': invite_accept_send,
        'leader_exit': leader_exit_send,
        'invitee_exit': invitee_exit_send,
        'game_start': game_start_send,
        'game_exit': game_exit_send,
    }

    def connect(self):
        self.user_id = self.scope["url_route"]["kwargs"]["user_id"]
        self.user_nickname = self.scope["url_route"]["kwargs"]["user_nickname"]
        self.notification_group_name = "notification_%s" % self.user_id
        self.user_info = "%s_%s" % (self.user_id, self.user_nickname)
        self.accept()

        async_to_sync(self.channel_layer.group_add)(
            self.notification_group_name, self.channel_name
        )
        async_to_sync(self.channel_layer.group_add)(
            "lobby", self.channel_name
        )

        user_list = caches['default']._cache.get_client().keys("*")
        user_json = []
        for user in user_list:
            temp = user.decode('utf-8')[3:].split("_")
            content = {
                "id": int(temp[0]),
                "nickname": temp[1],
            }
            user_json.append(content)
        self.send_json(
            {
                "type": "user_list",
                "users": user_json
            }
        )
        async_to_sync(self.channel_layer.group_send)(
            "lobby",
            {
                "type": "user_join",
                "user": {
                    "id": self.user_id,
                    "nickname": self.user_nickname
                }
            }
        )
        caches['default'].set(self.user_info, "")

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
                "user": {
                    "id": self.user_id,
                    "nickname": self.user_nickname
                }
            }
        )
        caches['default'].delete(self.user_info)
        # need to delete room redis too.

    def receive(self, text_data):
        data = json.loads(text_data)
        self.types[data["type"]](self, data)

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
        if caches['default'].get(leader) == "game":
            self.send_json(
                {
                    "type": "inviter_playing",
                }
            )
        if caches['team'].get(leader) is not None:
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
            team_json["invitees"].append(new_invitee_json)
            update = {
                "leader": {
                    'id': leader_id,
                    'nickname': leader_nickname,
                },
                "invitees": team_json["invitees"],
            }
            caches['team'].set(leader, json.dumps(update))
            caches['default'].set(invitee, "invitee")
            async_to_sync(self.channel_layer.group_add)(
                "team_%s" % leader_id, self.channel_name
            )
            async_to_sync(self.channel_layer.group_send)(
                "team_%s" % leader_id, {
                    "type": "team_list",
                    "leader": {
                        'id': leader_id,
                        'nickname': leader_nickname,
                    },
                    "invitees": team_json["invitees"],
                }
            )
            return
        # inviter have no team, invitee have no team
        invitee_json_str = '[{"id": %d, "nickname": "%s"}]' % (
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
        if caches['team'].get(leader) is not None:
            return
        # inviter have no team, invitee have no team
        invitee_json_str = '[{"id": %d, "nickname": "%s"}]' % (
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
        update = {
            "leader": {
                'id': leader_id,
                'nickname': leader_nickname,
            },
            "invitees": invitee_json,
        }
        caches['default'].set(leader, "leader")
        caches['team'].set(leader, json.dumps(update))
        async_to_sync(self.channel_layer.group_add)(
            "team_%s" % leader_id, self.channel_name
        )

    def team_list(self, event):
        self.send_json(event)

    def leader_exit(self, event):
        async_to_sync(self.channel_layer.group_discard)(
            "team_%s" % event["leader_id"], self.channel_name
        )
        self.send_json(
            {
                'type': 'leader_exit',
            }
        )

    def invitee_exit(self, event):
        self.send_json(event)

    def no_invitee(self, event):
        async_to_sync(self.channel_layer.group_discard)(
            "team_%s" % event["leader_id"], self.channel_name
        )

    def user_join(self, event):
        self.send_json(event)

    def user_leave(self, event):
        self.send_json(event)
