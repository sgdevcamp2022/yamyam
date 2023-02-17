using UnityEngine;

public class LobbyWindowController : MonoBehaviour
{
    private static LobbyWindowController s_instance = null;
    public static LobbyWindowController Instance
    { get => s_instance; }

    [SerializeField] private GameObject _matchingWindow;
    [SerializeField] private Match _match;
    [SerializeField] private GameObject _myPageWindow;
    [SerializeField] private GameObject _freindWindow;
    [SerializeField] private GameObject _alertWindow;
    [SerializeField] private LobbyAlert _lobbyAlert;
    [SerializeField] private GameObject _choiceAlertWindow;
    [SerializeField] private LobbyChoiceAlert _lobbyChoiceAlert;
    [SerializeField] private GameObject _settingWindow;
    [SerializeField] private GameObject _removeWindow;
    [SerializeField] private GameObject _teamWindow;
    [SerializeField] private GameObject _allChatWindow;
    [SerializeField] private GameObject _teamChatWindow;
 
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


    public void ActiveAlertWindow(LobbyAlertMessage message)
    {
        //AlertMessage에 따라 다르게 되도록 
        switch(message)
        {
            case LobbyAlertMessage.RejectInvite:
                _lobbyAlert.SetAlertContent(LobbyAlertMessage.RejectInvite);
                break;
            case LobbyAlertMessage.Logout:
                _lobbyAlert.SetAlertContent(LobbyAlertMessage.Logout);
                break;
        }

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

    public void ActiveInviteTeamWindow(string userName)
    {
        _lobbyChoiceAlert.SetChoiceAlertContent(LobbyChoiceAlertMessage.InviteTeam, userName);
        _choiceAlertWindow.SetActive(true);
    }

    public void InActiveChoiceAlertWindow()
    {
        _choiceAlertWindow.SetActive(false);
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

    public void ActiveTeamWindow()
    {
        _teamWindow.SetActive(true);
        Chatting.Instance.ActiveTeamChatting();
    }

    public void InActiveTeamWindow()
    {
        _teamWindow.SetActive(false);
        Chatting.Instance.InActiveTeamChatting();
    }

    public void ActiveAllChatWindow()
    {
        _allChatWindow.SetActive(true);
    }

    public void InActiveAllChatWindow()
    {
        _allChatWindow.SetActive(false);
    }

    public void ActiveTeamChatWindow()
    {
        _teamChatWindow.SetActive(true);
    }

    public void InActiveTeamChatWindow()
    {
        _teamChatWindow.SetActive(false);
    }
}
