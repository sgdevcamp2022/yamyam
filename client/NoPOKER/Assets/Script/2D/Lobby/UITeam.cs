using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITeam : MonoBehaviour
{
    [SerializeField] GameObject[] _teamMemberList = new GameObject[4];
    [SerializeField] TMP_Text[] _teamMemberName = new TMP_Text[4];
    int _memberNum = 1;
    List<string> _names = new List<string>();


    public void CleanTeamMember()
    {
        for (int i = 1; i < 4; i++)
        {
            _teamMemberList[i].SetActive(false);
        }
    }

    public void UpdateTeamMember()
    {
        CleanTeamMember();
        //0은 팀장자리
        _teamMemberName[0].SetText(Team.Instance.LeaderData.nickname);
        _teamMemberList[0].SetActive(true);
        for (int i = 1; i <= Team.Instance.TeamMemberData.Length; i++)
        {
            _teamMemberName[i].SetText(Team.Instance.TeamMemberData[i - 1].nickname);
            _teamMemberList[i].SetActive(true);
        }
    }

    public void AddTeamMember(string name)
    {
        _names.Add(name);
        _memberNum++;
    }

    public void SubTeamMember(string name)
    {
        _names.RemoveAt(_memberNum);
        _memberNum--;
    }

    public void ExitTeam()
    {
        LobbyWindowController.Instance.InActiveTeamWindow();
        LobbyWindowController.Instance.ActiveAllChatWindow();
        Team.Instance.ExitTeam();
    }

    public void SettingTeam(string[] members)
    {
        _memberNum = members.Length;
    }
}
