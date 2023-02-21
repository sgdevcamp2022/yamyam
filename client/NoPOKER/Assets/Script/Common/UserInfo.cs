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
    private bool _isLeader = true;
    public bool IsLeader { get => _isLeader; }
    private LobbyUserSocketData _userSocketData;
    public LobbyUserSocketData UserSocketData { get => _userSocketData; }
    private void Start()
    {
        Init();
    }

    public void Init()
    {
        if (_instance == null)
            _instance = this;

        DontDestroyOnLoad(this);
    }

    public void SetUserInfo(int id, string nickName)
    {
        _userID = id;
        _userNickName = nickName;

        _userSocketData = new LobbyUserSocketData(id, nickName);
    }

    public void SetLeaderState(bool state)
    {
        _isLeader = state;
    }
}
