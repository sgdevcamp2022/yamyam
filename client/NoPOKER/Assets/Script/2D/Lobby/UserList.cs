using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using Newtonsoft.Json.Linq;

public enum LobbyUserChangeType
{
    None,
    Setting,
    Add,
    Sub
}
public class UserList : MonoBehaviour
{
    private static UserList s_instance = null;
    public static UserList Instance { get => s_instance; }
    [SerializeField] AllUserRecycleViewController _allUserRecycleViewController = new AllUserRecycleViewController();
    LobbyUserListSocketData _userList = null;
    UserSocketData _user;
    public LobbyUserChangeType IsUserCountChanged = LobbyUserChangeType.None;


    private void Awake()
    {
        Init();
    }

    void Init()
    {
        if (s_instance == null)
            s_instance = this;
    }

    private void Update()
    {
        if (IsUserCountChanged != LobbyUserChangeType.None)
        {
            switch (IsUserCountChanged)
            {
                case LobbyUserChangeType.Setting:
                    _allUserRecycleViewController.SetDatas(_userList.users);
                    break;
                case LobbyUserChangeType.Add:
                    _allUserRecycleViewController.AddData(new UserSocketData(_user.id, _user.nickname));
                    break;
                case LobbyUserChangeType.Sub:
                    _allUserRecycleViewController.DeleteData(_user);
                    break;
            }
            IsUserCountChanged = LobbyUserChangeType.None;
        }
    }


    IEnumerator SetDatas()
    {
        yield return new WaitUntil(() => _userList.users != null);
        _allUserRecycleViewController.SetDatas(_userList.users);
    }

    public void JoinUser(DefaultUserSocketData newData) //한명 더 들어왔을 때
    {
        _user = new UserSocketData(newData.user.id, newData.user.nickname);
    }

    public void SetUserList(LobbyUserListSocketData userList)
    {
        _userList = userList;
    }

    public void LeaveUser(DefaultUserSocketData leaveData) //한명이 빠졌을 때
    {
        _user = new UserSocketData(leaveData.user.id, leaveData.user.nickname);
    }


}
