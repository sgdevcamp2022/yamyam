from asgiref.sync import async_to_sync
from channels.layers import get_channel_layer
from rest_framework import status
from rest_framework.views import APIView
from rest_framework.response import Response


class InviteRequestUser(APIView):
    def post(self, request, user_nickname, *args, **kwargs):
        try:
            inviter_nickname = request.data.get("inviter_nickname")
            channel_layer = get_channel_layer()
            async_to_sync(channel_layer.group_send)(
                user_nickname,
                {
                    'type': 'notification_team_invitation',
                    'inviter': inviter_nickname,
                }
            )
        except:
            return Response(status=status.HTTP_404_NOT_FOUND)
        return Response(status=status.HTTP_200_OK)


class InviteAcceptUser(APIView):
    # user_nickname : inviter nickname
    def post(self, request, *args, **kwargs):
        # group_add to team
        try:
            inviter_nickname = request.data.get("inviter_nickname")
            invitee_nickname = request.data.get("invitee_nickname")
            channel_layer = get_channel_layer()
            async_to_sync(channel_layer.group_send)(
                invitee_nickname,
                {
                    'type': 'notification_team_invitation_accept',
                    'team_leader': inviter_nickname,
                }
            )
            async_to_sync(channel_layer.group_send)(
                inviter_nickname,
                {
                    'type': 'notification_team_invitation_accept',
                    'team_leader': inviter_nickname,
                }
            )
        except:
            return Response(status=status.HTTP_404_NOT_FOUND)
        return Response(status=status.HTTP_200_OK)
