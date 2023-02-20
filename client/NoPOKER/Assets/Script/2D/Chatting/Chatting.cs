using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;

public enum ChattMode
{
    All,
    Team
}
public class Chatting : MonoBehaviour
{
    private static Chatting s_instance = null;
    public static Chatting Instance { get => s_instance; }
    [SerializeField] AllChattRecycleViewController _allChattingRecycleViewController;
    [SerializeField] TeamChattRecycleViewController _teamChattingRecycleViewController;
    [SerializeField] GameObject _teamChattingUI;
    DefaultMessageSocketData _MessageData = new DefaultMessageSocketData();

    private ChattMode _chattMode = ChattMode.All;
    public bool IsReceiveMessage = false;
    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        if (s_instance == null)
            s_instance = this;
    }

    private void Update()
    {
        if(IsReceiveMessage)
        {
            if (_MessageData.id != UserInfo.Instance.UserID)
            {
                _allChattingRecycleViewController.AddData(new UIChattData { Name = _MessageData.nickname, Chat = _MessageData.message });
               _allChattingRecycleViewController.UpdateData();
            }
            IsReceiveMessage = false;
        }
    }

    public void SendChatting(UIChattData chattingData)
    {
        switch(_chattMode)
        {
            case ChattMode.All:
                _MessageData.LobbyMessageSetting(chattingData.Chat);
                LobbyConnect.Instance.SendAllChattMessage(_MessageData);

                _allChattingRecycleViewController.AddData(new UIChattData { Name = chattingData.Name, Chat = chattingData.Chat } );
                _allChattingRecycleViewController.UpdateMyData();
                break;
            case ChattMode.Team:
                _MessageData.TeamMessageSetting(chattingData.Chat);
                LobbyConnect.Instance.SendTeamChattMessage(_MessageData);

                _teamChattingRecycleViewController.AddData(new UIChattData { Name = chattingData.Name, Chat = chattingData.Chat });
                _teamChattingRecycleViewController.UpdateMyData();
                break;
        }
    }
    public void ReceiveChatting(DefaultMessageSocketData receiveData)
    { 
        _MessageData = receiveData;   
    }


    public void SetChattingMode(ChattMode mode)
    {
        _chattMode = mode;
    }

    public void ActiveTeamChatting()
    {
        _teamChattingUI.SetActive(true);
    }

    public void InActiveTeamChatting()
    {
        _teamChattingUI.SetActive(false);
    }
}
