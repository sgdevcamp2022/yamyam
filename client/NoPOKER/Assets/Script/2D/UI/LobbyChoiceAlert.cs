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
    private string[] _twoAlertMessage = {"������ �����Ͻðڽ��ϱ�?", "���� ģ����û�� �Ͽ����ϴ�.\n���� �Ͻðڽ��ϱ�?", "���� ���ʴ븦 �Ͽ����ϴ�.\n���� �Ͻðڽ��ϱ�?" };
    private LobbyChoiceAlertMessage _alertMessage;
    /* ���� ģ�� �� �� ��� �߰��� Ȱ��ȭ ����
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
        /*��� �����߳Ŀ� ���� �ش� ��ɿ� �´� Ŭ������ ����.*/
    }

    public void ClickedRejectButton()
    {
        /*��� �����߳Ŀ� �����ش� ��ɿ� �´� Ŭ������ ����
        switch(_alertMessage) ������ �б��� ��������*/
        gameObject.SetActive(false);
    }
}
