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

    private string[] _alertMessage = { "님이 초대를 거절하였습니다." };

    public void SetAlertContent(LobbyAlertMessage message)
    {
        _alertContent.text = _alertMessage[(int)message];
    }
}
