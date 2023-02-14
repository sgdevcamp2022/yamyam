using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    //�� ��������� ����ϴ� �κ�:
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

        //�޴°� test��
        LobbyWindowController.Instance.ActiveInviteTeamWindow(name);
        //
    }

    public void AcceptedInvite() //�ʴ��� ��밡 ������ �Ͽ��ٸ�
    {
        //���� �߰�.
        List<string> members = new List<string>();
        members.Add("���");
        members.Add("����");
        //�� ȭ�� ���̵���.
        _uiTeam.SetTeamMember(members);
        LobbyWindowController.Instance.ActiveTeamWindow();
        
    }

    public void RejectedInvite()
    {
        //�������ߴ� �޼��� ���̰��ϱ�.
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
