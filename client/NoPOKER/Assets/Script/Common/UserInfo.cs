using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfo : MonoBehaviour
{
    static UserInfo _instance = null;
    public static UserInfo Instance { get => _instance; }
    private string _userNickName;
    public string NickName { get => _userNickName; }
    private int _userID;
    public int UserID { get => _userID; }
    private void Start()
    {
        Init();
    }

    public void Init()
    {
        if (_instance == null)
            _instance = this;

        _userID = 1;
        _userNickName = "¾ä¾ä";
    }
}
