from django.urls import path

from . import views

urlpatterns = [
    path("notifiaction/invitation/<str:user_nickname>/",
         views.InviteRequestUser.as_view(), name='invite_request_user'),
    path("notification/invitation/accept/",
         views.InviteAcceptUser.as_view(), name='invite_accept_user')
]
