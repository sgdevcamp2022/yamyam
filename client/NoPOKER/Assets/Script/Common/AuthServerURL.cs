using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthServerURL : MonoBehaviour
{
    public string LoginURL = "http://127.0.0.1:8000/accounts/login/";
    public string SignUpURL = "http://127.0.0.1:8000/accounts/";
    public string FindIdUrl = "http://127.0.0.1:8000/accounts/find_username/";
    public string ResetPwUrl = "http://127.0.0.1:8000/accounts/password_reset";
    public string _logoutUrl = "http://127.0.0.1:8000/accounts/logout/";



}
