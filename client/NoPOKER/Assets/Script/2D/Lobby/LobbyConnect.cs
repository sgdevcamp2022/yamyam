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
using System.Collections.Concurrent;

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
    team_message,
    inviter_playing,
    team_excess,
    invitee_playing,
    team_redundancy,
}


public class DefaultMessageSocketData
{

    public string type;
    public int leader_id;
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

    public void TeamMessageSetting(string message)
    {
        type = "team_message";
        leader_id = Team.Instance.LeaderData.id;
        id = UserInfo.Instance.UserID;
        nickname = UserInfo.Instance.NickName;
        this.type = type;
        this.message = message;
    }
}

public class LobbyUserListSocketData
{
    public string type;
    public UserSocketData[] users;
}



public class UserSocketData
{
    public int id;
    public string nickname;
    public int order;
    public int card;

    public UserSocketData() { }

    public UserSocketData(int id, string nickname)
    {
        this.id = id;
        this.nickname = nickname;
    }

    public UserSocketData(int id, string nickname, int card)
    {
        this.id = id;
        this.nickname = nickname;
        this.card = card;
    }

}

public class UserDefaultData
{
    public int id;
    public string nickname;
    public int order;
    public int card;
    public UserDefaultData(int id, string nickname, int card)
    {
        this.id = id;
        this.nickname = nickname;
        this.card = card;
    }

}
public class UserPokerData
{
    public int id;
    public string nickname;
    public int order;
    public int card;
    public int currentChip;
    public bool result;
    public UserPokerData(int id, string nickname, int card)
    {
        this.id = id;
        this.nickname = nickname;
        this.card = card;
    }
    public UserPokerData(int id, int currentChip, bool result)
    {
        this.id = id;
        this.currentChip = currentChip;
        this.result = result;
    }

    public void SetNewData(int currentChip)
    {
        this.currentChip = currentChip;
    }

}


public class DefaultUserSocketData
{
    public string type;
    public UserSocketData user;
}


public class LobbyMessageType
{
    public string type;
}

public class GameExitRequestSocketData
{
    public string type = "game_exit";
    public UserSocketData player;

}

public class InviteRequestSocketData
{
    public string type = "invite_request";
    public UserSocketData inviter;
    public UserSocketData invitee;

    public void SetInviteRequestSocketData(UserSocketData inviter, UserSocketData invitee)
    {
        this.inviter = inviter;
        this.invitee = invitee;
    }
}

public class TeamSocketData
{
    public string type;
    public UserSocketData leader;
    public UserSocketData[] invitees;
}

public class TeamMemberExitSocketData
{
    public string type;
    public UserSocketData requester;
    public UserSocketData leader;
    public UserSocketData[] invitees;
}

public class LobbyConnect : MonoBehaviour
{
    private static LobbyConnect s_instance = null;
    public static LobbyConnect Instance { get => s_instance; }
    public WebSocket _lobbySocket;
    StringBuilder _urlBuilder = new StringBuilder();
    LobbyUserListSocketData _userListData;
    public LobbyUserListSocketData UserListData { get => _userListData; }
    private void Start()
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

        LobbyMessageType _messageType = JsonConvert.DeserializeObject<LobbyMessageType>(e.Data);
        try
        {
            switch (Enum.Parse(typeof(LobbySocketType), _messageType.type))
            {
                case LobbySocketType.lobby_message:
                    DefaultMessageSocketData _lobbyData = JsonConvert.DeserializeObject<DefaultMessageSocketData>(e.Data);
                    Chatting.Instance.ReceiveChatting(_lobbyData);
                    Chatting.Instance.SetChattingMode(ChattMode.All);
                    Chatting.Instance.IsReceiveMessage = true;
                    break;
                case LobbySocketType.team_message:
                    DefaultMessageSocketData _teamChattData = JsonConvert.DeserializeObject<DefaultMessageSocketData>(e.Data);
                    Chatting.Instance.ReceiveChatting(_teamChattData);
                    Chatting.Instance.SetChattingMode(ChattMode.Team);
                    Chatting.Instance.IsReceiveMessage = true;
                    break;
                case LobbySocketType.user_join:
                    try
                    {

                        DefaultUserSocketData _userData = JsonConvert.DeserializeObject<DefaultUserSocketData>(e.Data);
                        if (_userData.user.id != UserInfo.Instance.UserID)
                        {
                            UserList.Instance.JoinUser(_userData);
                            UserList.Instance.IsUserCountChanged = LobbyUserChangeType.Add;
                        }


                    }
                    catch (Exception ex)
                    {
                        Debug.Log("ERROR : " + ex);
                    }
                    break;
                case LobbySocketType.user_list:
                    _userListData = JsonConvert.DeserializeObject<LobbyUserListSocketData>(e.Data);

                    UserList.Instance.IsUserCountChanged = LobbyUserChangeType.Setting;
                    UserList.Instance.SetUserList(_userListData);
                    break;
                case LobbySocketType.invite_request:
                    InviteRequestSocketData _requestUserData = JsonConvert.DeserializeObject<InviteRequestSocketData>(e.Data);
                    Team.Instance.ReceiveInviteRequest(_requestUserData);
                    Team.Instance.TeamType = LobbySocketType.invite_request;
                    Team.Instance.ChangedRequestState = true;
                    break;
                case LobbySocketType.team_list:
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
                    {
                        Team.Instance.TeamType = LobbySocketType.leader_exit;
                        Team.Instance.ChangedRequestState = true;
                    }
                    break;
                case LobbySocketType.invitee_exit:
                    TeamSocketData _newTeamData = JsonConvert.DeserializeObject<TeamSocketData>(e.Data);
                    if (_newTeamData.invitees.Length == 0)
                    {
                        Team.Instance.TeamType = LobbySocketType.invitee_exit;
                        Team.Instance.ChangedRequestState = true;
                    }
                    else
                    {
                        Team.Instance.SetTeamData(_newTeamData);
                        Team.Instance.TeamType = LobbySocketType.team_list;
                        Team.Instance.ChangedRequestState = true;
                    }
                    break;
                case LobbySocketType.invitee_playing:
                    LobbyWindowController.Instance.ActiveAlertWindow(LobbyAlertMessage.Invitee_Playing);
                    break;
                case LobbySocketType.inviter_playing:
                    LobbyWindowController.Instance.ActiveAlertWindow(LobbyAlertMessage.Inviter_Playing);
                    break;
                case LobbySocketType.team_excess:
                    LobbyWindowController.Instance.ActiveAlertWindow(LobbyAlertMessage.Team_Excess);
                    break;
                case LobbySocketType.team_redundancy:
                    LobbyWindowController.Instance.ActiveAlertWindow(LobbyAlertMessage.Team_Redundancy);
                    break;

            }
        }
        catch (Exception ex)
        {
            Debug.Log("ERROR : " + ex);
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

    public void SendAllChattMessage(DefaultMessageSocketData sendMessage)
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
    public void SendTeamChattMessage(DefaultMessageSocketData sendMessage)
    {
        sendMessage.type = "team_message";
        sendMessage.leader_id = Team.Instance.LeaderData.id;
        _lobbySocket.Send(JsonConvert.SerializeObject(sendMessage));
    }

    public void SendGameStartMessage(TeamSocketData sendMessage)
    {
        try
        {
            sendMessage.type = "game_start";
            _lobbySocket.Send(JsonConvert.SerializeObject(sendMessage));
        }
        catch (Exception ex)
        {
            Debug.Log("EXCEPTION : " + ex);
        }
    }

    public void SendGameExitMessage(GameExitRequestSocketData sendMessage)
    {
        _lobbySocket.Send(JsonConvert.SerializeObject(sendMessage));
    }
}
