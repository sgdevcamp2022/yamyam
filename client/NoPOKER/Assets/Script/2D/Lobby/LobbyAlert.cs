using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public enum LobbyAlertMessage
{
    RejectInvite,
    Logout,
    FailAuth,
    Invitee_Playing,
    Team_Redundancy,
    Inviter_Playing,
    Team_Excess
}
public class LobbyAlert : MonoBehaviour
{
    [SerializeField] private TMP_Text _alertContent;
    private LobbyAlertMessage _alertState;
    private string[] _alertMessage = { "님이 초대를 거절하였습니다.", "로그인화면으로 이동합니다."  ,"인증에 실패하였습니다.\n로그인화면으로 이동합니다."
                                        ,"이미 게임중인 유저입니다.", "이미 다른팀에 속한 유저입니다" , "이미 플레이중인 팀입니다.","팀에 인원이 다 차있습니다"};


    public void SetAlertContent(LobbyAlertMessage message)
    {
        _alertContent.text = _alertMessage[(int)message];
        _alertState = message;
    }

    public void ClickedAlertCheckButton()
    {
        switch(_alertState)
        {
            case LobbyAlertMessage.Logout:
            case LobbyAlertMessage.FailAuth:
                GameManager.Instance.ChangeScene(Scenes.LoginScene);
                LobbyWindowController.Instance.InActiveAlertWindow();
                break;
            case LobbyAlertMessage.RejectInvite:
                LobbyWindowController.Instance.InActiveAlertWindow();
                break;
        }
    }
}
