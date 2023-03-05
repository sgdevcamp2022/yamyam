using UnityEngine;
using TMPro;
using System.Text;

public enum LobbyChoiceAlertMessage
{
    GameExit,
    InviteTeam
}

public class LobbyChoiceAlert : MonoBehaviour
{
    [SerializeField] private TMP_Text _alertContent;

    private string[] _twoAlertMessage = {"게임을 종료하시겠습니까?",  "님이 팀초대를 하였습니다.\n수락 하시겠습니까?" };
    private LobbyChoiceAlertMessage _alertMessage;
    private StringBuilder _stingBuilder = new StringBuilder();

    public void SetChoiceAlertContent(LobbyChoiceAlertMessage message)
    {
        _alertMessage = message;
        _alertContent.text = _twoAlertMessage[(int)message];
    }
    public void SetChoiceAlertContent(LobbyChoiceAlertMessage message, string who)
    {
        _alertMessage = message;
        _stingBuilder.Clear();
        _stingBuilder.Append(who);
        _stingBuilder.Append(_twoAlertMessage[(int)message]);
        _alertContent.text = _stingBuilder.ToString();
    }

    public void ClickedAcceptButton()
    {

        switch(_alertMessage)
        {
            case LobbyChoiceAlertMessage.InviteTeam:
                Team.Instance._inviteRequestData.type = "invite_accept";
                LobbyConnect.Instance.SendInviteAccept(Team.Instance._inviteRequestData);
                break;
            case LobbyChoiceAlertMessage.GameExit:
                Application.Quit();
                break;

        }
        LobbyWindowController.Instance.InActiveChoiceAlertWindow();
    }

    public void ClickedRejectButton()
    {

        switch (_alertMessage)
        {
            case LobbyChoiceAlertMessage.InviteTeam:
                Team.Instance.RejectedInvite();
                break;
        }
        LobbyWindowController.Instance.InActiveChoiceAlertWindow();
    }
}
