using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    //팀 서버통신을 담당하는 부분:
    static Team s_instance = null;
    public static Team Instance { get => s_instance; }
    [SerializeField] UITeam _uiTeam;
    public InviteRequestSocketData _inviteRequestData = new InviteRequestSocketData();
    public bool ChangedRequestState = false;
    public LobbySocketType TeamType = LobbySocketType.None;
    public TeamSocketData _teamData = new TeamSocketData();
    public TeamSocketData GetTeamData { get => _teamData; }
    UserSocketData _leader;
    public UserSocketData LeaderData { get => _leader; }
    UserSocketData[] _invitees = new UserSocketData[4];
    public UserSocketData[] TeamMemberData { get => _invitees; }
    private void Start()
    {
        Init();
    }

    public void Init()
    {
        if (s_instance == null)
            s_instance = this;
    }


    private void Update()
    {
        if(ChangedRequestState)
        {
            switch (TeamType)
            {
                case LobbySocketType.invite_request:
                    InvitedRequest();
                    break;
                case LobbySocketType.team_list:
                    ShowTeamList();
                    break;
                case LobbySocketType.invitee_exit:
                case LobbySocketType.leader_exit:
                    LobbyWindowController.Instance.InActiveTeamWindow();
                    LobbyWindowController.Instance.InActiveTeamChatWindow();
                    break;
            }
            ChangedRequestState = false;
        }
    }

    public void SendInviteRequest(UserSocketData InviteUserData)
    {
        UserSocketData inviter = new UserSocketData(UserInfo.Instance.UserID, UserInfo.Instance.NickName);
        _inviteRequestData.SetInviteRequestSocketData(inviter, InviteUserData);
        LobbyConnect.Instance.SendInviteRequest(_inviteRequestData);
    }

    public void ReceiveInviteRequest(InviteRequestSocketData requestData)
    {
        _inviteRequestData = requestData;
    }

    public void InvitedRequest()
    {
        _inviteRequestData.type = "invite_request";
        //받는거 test용
        // LobbyConnect.Instance.SendInviteRequest(_inviteRequestData);
        LobbyWindowController.Instance.ActiveInviteTeamWindow(_inviteRequestData.inviter.nickname);
        UserInfo.Instance.SetLeaderState(true);

    }



    public void SetTeamData(TeamSocketData receivedData)
    {
        _invitees = new UserSocketData[4];
           _teamData = receivedData;
        _leader = new UserSocketData(receivedData.leader.id, receivedData.leader.nickname) ;
        _invitees = receivedData.invitees;   

        if(_leader.id != UserInfo.Instance.UserID)
        {
            UserInfo.Instance.SetLeaderState(false);
        }
    }
    public void ShowTeamList()
    {
        _uiTeam.UpdateTeamMember();
        LobbyWindowController.Instance.ActiveTeamWindow();
    }

    public void RejectedInvite()
    {
        //거절당했단 메세지 보이게하기.
       // LobbyWindowController.Instance.ActiveAlertWindow(LobbyAlertMessage.RejectInvite);
    }

    public void AcceptInvite()
    {

    }

    public void RejectInvite()
    {

    }

    public void ExitTeam()
    {
      if (UserInfo.Instance.IsLeader)
        {
            _teamData.type = "leader_exit";
            LobbyConnect.Instance.SendLeaderExit(_teamData);
        }
       else
        {
            TeamMemberExitSocketData _exitData = new TeamMemberExitSocketData();
            _exitData.type = "invitee_exit";
            _exitData.requester = UserInfo.Instance.UserSocketData;
            _exitData.leader = LeaderData;
            _exitData.invitees = _invitees;

            LobbyConnect.Instance.SendInviteeExit(_exitData);
        }
    }
 
 
}
