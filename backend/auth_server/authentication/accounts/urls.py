from django.urls import path
from .views import CreateAccount, ActivateAccount, LoginAccount, LogoutAccount, CheckToken, FindUsername, HandleAccount

app_name = 'accounts'
urlpatterns = [
    path('', CreateAccount.as_view(), name='create_account'),
    path('activate/<uidb64>/<token>/',
         ActivateAccount.as_view(), name='activate_account'),
    path('login/', LoginAccount.as_view(), name='login_account'),
    path('logout/', LogoutAccount.as_view(), name='logout_account'),
    path('check_token/', CheckToken.as_view(), name='check_token'),
    path('find_username/', FindUsername.as_view(), name='find_username'),
    path('<id>/', HandleAccount.as_view(), name='handle_account'),
]
