using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public enum LobbyAlertMessage
{
    RejectInvite,
    Logout
}
public class LobbyAlert : MonoBehaviour
{
    [SerializeField] private TMP_Text _alertContent;
    private LobbyAlertMessage _alertState;
    private string[] _alertMessage = { "���� �ʴ븦 �����Ͽ����ϴ�.", "�α���ȭ������ �̵��մϴ�." };


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
                GameManager.Instance.ChangeScene(Scenes.LoginScene);
                LobbyWindowController.Instance.InActiveAlertWindow();
                break;
            case LobbyAlertMessage.RejectInvite:
                LobbyWindowController.Instance.InActiveAlertWindow();
                break;
        }
    }
}
