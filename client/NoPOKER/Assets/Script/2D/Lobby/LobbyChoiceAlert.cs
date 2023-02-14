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

    private string[] _twoAlertMessage = {"������ �����Ͻðڽ��ϱ�?",  "���� ���ʴ븦 �Ͽ����ϴ�.\n���� �Ͻðڽ��ϱ�?" };
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
        _stingBuilder.Append(UserInfo.Instance.NickName);
        _stingBuilder.Append(_twoAlertMessage[(int)message]);
        _alertContent.text = _stingBuilder.ToString();
    }

    public void ClickedAcceptButton()
    {
        /*��� �����߳Ŀ� ���� �ش� ��ɿ� �´� Ŭ������ ����.*/
        switch(_alertMessage)
        {
            case LobbyChoiceAlertMessage.InviteTeam:
                Team.Instance.AcceptedInvite(); 
                break;
        }
        LobbyWindowController.Instance.InActiveChoiceAlertWindow();
    }

    public void ClickedRejectButton()
    {
        /*��� �����߳Ŀ� �����ش� ��ɿ� �´� Ŭ������ ����
        switch(_alertMessage) ������ �б��� ��������*/
        switch (_alertMessage)
        {
            case LobbyChoiceAlertMessage.InviteTeam:
                //test��
                Team.Instance.RejectedInvite();
                //������ Team.Instance.RejectInvite();
                break;
        }
        LobbyWindowController.Instance.InActiveChoiceAlertWindow();
    }
}
