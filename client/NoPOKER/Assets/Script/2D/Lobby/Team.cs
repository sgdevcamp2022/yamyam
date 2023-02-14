using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    //팀 서버통신을 담당하는 부분:
    static Team s_instance = null;
    public static Team Instance { get => s_instance; }
    [SerializeField] UITeam _uiTeam;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        if (s_instance == null)
            s_instance = this;
    }

    public void Invite(string name)
    {

        //받는거 test용
        LobbyWindowController.Instance.ActiveInviteTeamWindow(name);
        //
    }

    public void AcceptedInvite() //초대한 상대가 수락을 하였다면
    {
        //팀원 추가.
        List<string> members = new List<string>();
        members.Add("얌얌");
        members.Add("뇸뇸");
        //팀 화면 보이도록.
        _uiTeam.SetTeamMember(members);
        LobbyWindowController.Instance.ActiveTeamWindow();
        
    }

    public void RejectedInvite()
    {
        //거절당했단 메세지 보이게하기.
        LobbyWindowController.Instance.ActiveAlertWindow(LobbyAlertMessage.RejectInvite);
    }

    public void AcceptInvite()
    {

    }

    public void RejectInvite()
    {

    }

    public void ExitTeam()
    {

    }

}
