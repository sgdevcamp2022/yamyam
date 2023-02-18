using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITeam : MonoBehaviour
{
    [SerializeField] GameObject[] _teamMemberList = new GameObject[4];
    [SerializeField] TMP_Text[] _teamMemberName = new TMP_Text[4];
    int _memberNum = 1;
    List<string> _names = new List<string>();

    public void ActiveTeamMember()
    {
        for(int i=0;i<_names.Count;i++)
        {
            _teamMemberList[i].SetActive(true);
            _teamMemberName[i].SetText(_names[i]);
        }

        for (int i = _names.Count; i < 4; i++)
        {
            _teamMemberList[i].SetActive(false);
        }

    }
    public void InActiveTeamMember()
    {
        for (int i = _names.Count; i < 4; i++)
        {
            _teamMemberList[i].SetActive(false);
        }
    }

    public void AddTeamMember(string name)
    {
        _names.Add(name);
        _memberNum++;
    }

    public void SetTeamMember(List<string> names)
    {
        _names = names;
        ActiveTeamMember();
    }

    public void SubTeamMember(string name)
    {
        _names.RemoveAt(_memberNum);
        _memberNum--;
        InActiveTeamMember();
    }

    public void ExitTeam()
    {
        //Team클래스에 서버통신: 팀나가기
        LobbyWindowController.Instance.InActiveTeamWindow();
        LobbyWindowController.Instance.ActiveAllChatWindow();
    }

    public void SettingTeam(string[] members)
    {
        _memberNum = members.Length;
    }



}
