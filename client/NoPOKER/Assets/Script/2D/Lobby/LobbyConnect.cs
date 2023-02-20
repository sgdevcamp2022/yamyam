using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Threading;
using System;
using System.Text;
using Newtonsoft.Json;
using StompHelper;
using Unity.VisualScripting;

public enum LobbySocketType 
{
    None,
    user_list,
    user_join,
    lobby_message,
    invite_request,
    invite_accept,
    leader_exit,
    invitee_exit,
    game_exit,
    team_list,
    user_leave,

}


public class LobbyMessageSocketData {

    public string type;
    public int id;
    public string nickname;
    public string message;
    
    public void LobbyMessageSetting(string message)
    {
        type = "lobby_message";
        id = UserInfo.Instance.UserID;
        nickname = UserInfo.Instance.NickName;
        this.type = type;
        this.message = message;
    }
}

public class LobbyUserListSocketData {
    public string type;
    public LobbyUserSocketData[] users;
}

public class LobbyUserSocketData {
    public int id;
    public string nickname;

    public LobbyUserSocketData(int id, string nickname)
    {
        this.id = id;
        this.nickname = nickname;
    }
}

public class DefaultUserSocketData {
    public string type;
   public LobbyUserSocketData user;
}


public class LobbyMessageType {
    public string type;
}

public class InviteRequestSocketData {
    public string type = "invite_request";
    public LobbyUserSocketData inviter;
    public LobbyUserSocketData invitee;

    public void SetInviteRequestSocketData(LobbyUserSocketData inviter , LobbyUserSocketData invitee)
    {
        this.inviter = inviter;
        this.invitee = invitee;
    }
}

public class TeamSocketData {
    public string type;
    public LobbyUserSocketData leader;
    public LobbyUserSocketData[] invitees;
}

public class TeamMemberExitSocketData {
    public string type;
    public LobbyUserSocketData requester;
    public LobbyUserSocketData leader;
    public LobbyUserSocketData[] invitees;
}

public class LobbyConnect : MonoBehaviour {
    private static LobbyConnect s_instance = null;
    public static LobbyConnect Instance { get => s_instance; }
    WebSocket _lobbySocket;
    StringBuilder _urlBuilder = new StringBuilder();
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (s_instance == null)
            s_instance = this;
        DontDestroyOnLoad(this);
        LobbyServerConnect();
    }
    public void LobbyServerConnect()
    {
        _urlBuilder.Append("ws://127.0.0.1:8001/ws/lobby/");
        _urlBuilder.Append(UserInfo.Instance.UserID + "/");
        _urlBuilder.Append(UserInfo.Instance.NickName);
        _lobbySocket = new WebSocket(_urlBuilder.ToString());

        _lobbySocket.OnOpen += ws_OnOpen;
        _lobbySocket.OnMessage += ws_OnMessage;
        _lobbySocket.OnClose += ws_OnClose;
        _lobbySocket.Connect();

    }

    private void ws_OnClose(object sender, CloseEventArgs e)
    {
        Debug.Log("closeEvent : " + e.Reason);
        Debug.Log("close");
    }

    private void ws_OnOpen(object sender, EventArgs e)
    {
        Debug.Log("open");
    }

    bool isCompleteMessage = true;

    private void ws_OnMessage(object sender, MessageEventArgs e)
    {
        Debug.Log(e.Data);
        //MessageDataDecoding(e.Data);

        LobbyMessageType _messageType = JsonConvert.DeserializeObject<LobbyMessageType>(e.Data);

        Debug.Log(_messageType.type);
        try
        {
            switch (Enum.Parse(typeof(LobbySocketType), _messageType.type))
            {
                case LobbySocketType.lobby_message:
                    LobbyMessageSocketData _lobbyData = JsonConvert.DeserializeObject<LobbyMessageSocketData>(e.Data);
                    Chatting.Instance.ReceiveChatting(_lobbyData);
                    Chatting.Instance.IsReceiveMessage = true;
                    break;
            case LobbySocketType.user_join:
                //유저리스트 업데이트. 한명들어온거 반영
                try
                {
                        DefaultUserSocketData _userData = JsonConvert.DeserializeObject<DefaultUserSocketData>(e.Data);
                    Debug.Log(_userData);
                    UserList.Instance.JoinUser(_userData);
                    UserList.Instance.IsUserCountChanged = LobbyUserChangeType.Add;
                    }
                catch (Exception ex)
                {
                    Debug.Log("ERROR : " + ex);
                }
                break;
                case LobbySocketType.user_list:
                    LobbyUserListSocketData _userListData = JsonConvert.DeserializeObject<LobbyUserListSocketData>(e.Data);
                    Debug.Log(_userListData);
                    UserList.Instance.SetUserList(_userListData);
                    UserList.Instance.IsUserCountChanged = LobbyUserChangeType.Setting;
                    break;
                case LobbySocketType.invite_request:
                    InviteRequestSocketData _requestUserData = JsonConvert.DeserializeObject<InviteRequestSocketData>(e.Data);
                    Debug.Log(_requestUserData);
                    Team.Instance.ReceiveInviteRequest(_requestUserData);
                    Team.Instance.TeamType = LobbySocketType.invite_request;
                    Team.Instance.ChangedRequestState = true;
                    break;
                case LobbySocketType.team_list:
                    // StartCoroutine(SocketConnectAndGetData(e.Data));
                    TeamSocketData _teamData = JsonConvert.DeserializeObject<TeamSocketData>(e.Data);
                    Team.Instance.SetTeamData(_teamData);
                    Team.Instance.TeamType = LobbySocketType.team_list;
                    Team.Instance.ChangedRequestState = true;
                    break;
                case LobbySocketType.user_leave:
                    DefaultUserSocketData _leavUserData = JsonConvert.DeserializeObject<DefaultUserSocketData>(e.Data);                    
                    UserList.Instance.LeaveUser(_leavUserData);
                    UserList.Instance.IsUserCountChanged = LobbyUserChangeType.Sub;
                    break;
                case LobbySocketType.leader_exit:
                    LobbyWindowController.Instance.InActiveTeamWindow();
                    LobbyWindowController.Instance.InActiveTeamChatWindow();
                    break;
                case LobbySocketType.invitee_exit:
                    TeamSocketData _newTeamData = JsonConvert.DeserializeObject<TeamSocketData>(e.Data);
                    Debug.Log("Length : " + _newTeamData.invitees.Length);
                    if(_newTeamData.invitees.Length == 0 )
                    {
                        LobbyWindowController.Instance.InActiveTeamWindow();
                        LobbyWindowController.Instance.InActiveTeamChatWindow();
                    }
                    else
                    {
                        Team.Instance.SetTeamData(_newTeamData);
                        Team.Instance.TeamType = LobbySocketType.team_list;
                        Team.Instance.ChangedRequestState = true;
                    }
          
                    break;


            }
        }
        catch(Exception ex)
        {
            Debug.Log("ERROR : " +ex);
        }

    }

    IEnumerator SocketConnectAndGetData(string data)
    {
        yield return new WaitUntil(() => GameManager.Instance.CheckNowScene() == Scenes.LobbyScene);
        TeamSocketData _teamData = JsonConvert.DeserializeObject<TeamSocketData>(data);
        Team.Instance.SetTeamData(_teamData);
        Team.Instance.TeamType = LobbySocketType.team_list;
        Team.Instance.ChangedRequestState = true;

    }


    public void SendInviteRequest(InviteRequestSocketData sendMessage)
    {
        _lobbySocket.Send(JsonConvert.SerializeObject(sendMessage));
    }

    public void SendInviteAccept(InviteRequestSocketData sendMessage)
    {
        _lobbySocket.Send(JsonConvert.SerializeObject(sendMessage));
    }

    public void SendAllChattMessage(LobbyMessageSocketData sendMessage)
    {
        sendMessage.type = "lobby_message";
        _lobbySocket.Send(JsonConvert.SerializeObject(sendMessage));
    }

    public void SendLeaderExit(TeamSocketData sendMessage)
    {
        _lobbySocket.Send(JsonConvert.SerializeObject(sendMessage));
    }
    public void SendInviteeExit(TeamMemberExitSocketData sendMessage)
    {
        _lobbySocket.Send(JsonConvert.SerializeObject(sendMessage));
    }
    public void SendTeamChattMessage(LobbyMessageSocketData sendMessage)
    {
        sendMessage.type = "team_message";
        _lobbySocket.Send(JsonConvert.SerializeObject(sendMessage));
    }
}
