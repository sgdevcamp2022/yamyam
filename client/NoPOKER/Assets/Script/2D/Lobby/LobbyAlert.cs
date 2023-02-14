using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public enum LobbyAlertMessage
{
    RejectInvite
}
public class LobbyAlert : MonoBehaviour
{
    [SerializeField] private TMP_Text _alertContent;

    private string[] _alertMessage = { "���� �ʴ븦 �����Ͽ����ϴ�." };

    public void SetAlertContent(LobbyAlertMessage message)
    {
        _alertContent.text = _alertMessage[(int)message];
    }
}
