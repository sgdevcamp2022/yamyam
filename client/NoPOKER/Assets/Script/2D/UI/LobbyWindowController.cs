using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyWindowController : MonoBehaviour
{
    private static LobbyWindowController s_instance = null;
    public static LobbyWindowController Instance
    { get => s_instance; }

    [SerializeField]private GameObject _matchingWindow;
    [SerializeField] private Match _match;
    [SerializeField] private GameObject _myPageWindow;
    [SerializeField] private GameObject _freindWindow;
    [SerializeField] private GameObject _alertWindow;
    [SerializeField] private GameObject _choiceAlertWindow;
    [SerializeField] private LobbyChoiceAlert _lobbyChoiceAlert;
    [SerializeField] private GameObject _addFriendWindow;
    [SerializeField] private GameObject _settingWindow;
    [SerializeField] private GameObject _removeWindow;
    [SerializeField] private GameObject _succeedMatchWindow;


    private void Awake()
    {
        _init();
    }

    private void _init()
    {
        if (s_instance == null)
            s_instance = this;
    }

    public void Active2MatchingWindow()
    {
        //Match 클래스에다가 2인/4인 나눠서 알릴 수 있도록하기.
        _matchingWindow.SetActive(true);
    }

    public void Active4MatchingWindow()
    {
        _matchingWindow.SetActive(true);
    }

    public void InActiveMatchingWindow()
    {
        _matchingWindow.SetActive(false);
    }

    public void ActiveMyPageWindow()
    {
        _myPageWindow.SetActive(true);
    }

    public void InActiveMyPageWindow()
    {
        _myPageWindow.SetActive(false);
    }

    public void ActiveFriendWindow()
    {
        _freindWindow.SetActive(true);
    }

    public void InActiveFriendWindow()
    {
        _freindWindow.SetActive(false);
    }

    public void ActiveAlertWindow()
    {
        //AlertMessage에 따라 다르게 되도록 
        _alertWindow.SetActive(true);
    }

    public void InActiveAlertWindow()
    {
        _alertWindow.SetActive(false);
    }

    public void ClickedGameExitButton()
    {
        _lobbyChoiceAlert.SetChoiceAlertContent(LobbyChoiceAlertMessage.GameExit);
        _choiceAlertWindow.SetActive(true);
    }

    public void ActiveInviteFriendWindow()
    {
        _lobbyChoiceAlert.SetChoiceAlertContent(LobbyChoiceAlertMessage.InviteFriend);
        _choiceAlertWindow.SetActive(true);
    }

    public void ActiveInviteTeamWindow()
    {
        _lobbyChoiceAlert.SetChoiceAlertContent(LobbyChoiceAlertMessage.InviteTeam);
        _choiceAlertWindow.SetActive(true);
    }

    public void ActiveFriendAddWindow()
    {
        _addFriendWindow.SetActive(true);
    }

    public void InActiveFriendAddWindow()
    {
        _addFriendWindow.SetActive(false);        
    }

    public void ActiveSettingWindow()
    {
        _settingWindow.SetActive(true);
    }

    public void InActiveSettingWindow()
    {
        _settingWindow.SetActive(false);
    }

    public void ActiveRemoveWindow()
    {
        _removeWindow.SetActive(true);
    }

    public void InActiveRemoveWindow()
    {
        _removeWindow.SetActive(false);
    }

    public void ActiveSucceedMatchWindow()
    {
        _succeedMatchWindow.SetActive(true);
    }

    public void InActiveSucceedMatchWindow()
    {
        _succeedMatchWindow.SetActive(false);
    }
}
