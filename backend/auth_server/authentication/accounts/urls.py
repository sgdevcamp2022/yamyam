from django.urls import path
from .views import CreateAccount, ActivateAccount, LoginAccount, LogoutAccount, CheckToken, FindUsername, PasswordReset, PasswordResetConfirm, HandleAccount, WithdrawAccount

app_name = 'accounts'
urlpatterns = [
    path('', CreateAccount.as_view(), name='create_account'),
    path('activate/<uidb64>/<token>/',
         ActivateAccount.as_view(), name='activate_account'),
    path('login/', LoginAccount.as_view(), name='login_account'),
    path('logout/', LogoutAccount.as_view(), name='logout_account'),
    path('check_token/', CheckToken.as_view(), name='check_token'),
    path('find_username/', FindUsername.as_view(), name='find_username'),
    path('password_reset', PasswordReset.as_view(),
         name='password_reset'),
    path('reset/<uidb64>/<token>/', PasswordResetConfirm.as_view(),
         name='password_reset_confirm'),
    path('<id>/', HandleAccount.as_view(), name='handle_account'),
    path('withdraw/<uidb64>/<token>/',
         WithdrawAccount.as_view(), name='withdraw_account')
]
