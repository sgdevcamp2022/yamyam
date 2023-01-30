from django.urls import path
from .views import CreateAccount, ActivateAccount, LoginAccount, CheckToken

app_name = 'accounts'
urlpatterns = [
    path('', CreateAccount.as_view(), name='create_account'),
    path('activate/<uidb64>/<token>/',
         ActivateAccount.as_view(), name='activate_account'),
    path('login/', LoginAccount.as_view(), name='login_account'),
    path('check_token/', CheckToken.as_view(), name='check_token'),
]
