using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum LobbyChoiceAlertMessage
{
    GameExit,
    InviteFriend,
    InviteTeam
}

public class LobbyChoiceAlert : MonoBehaviour
{
    [SerializeField] private TMP_Text _alertContent;
    private string[] _twoAlertMessage = {"게임을 종료하시겠습니까?", "님이 친구신청을 하였습니다.\n수락 하시겠습니까?", "님이 팀초대를 하였습니다.\n수락 하시겠습니까?" };
    private LobbyChoiceAlertMessage _alertMessage;
    /* 추후 친구 및 팀 기능 추가시 활성화 예정
    [SerializeField] private Friend _firend;
    [SerializeField] private Team _team;
     */


    private void OnEnable()
    {
        _alertContent.text = _twoAlertMessage[(int)_alertMessage];
    }

    public void SetChoiceAlertContent(LobbyChoiceAlertMessage message)
    {
        _alertMessage = message;
    }

    public void ClickedAcceptButton()
    {
        /*어떤걸 수락했냐에 따라 해당 기능에 맞는 클래스에 전달.*/
    }

    public void ClickedRejectButton()
    {
        /*어떤걸 거절했냐에 따라해당 기능에 맞는 클래스에 전달
        switch(_alertMessage) 문으로 분기점 나눌예정*/
        gameObject.SetActive(false);
    }
}
