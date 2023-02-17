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

    private ChattMode _chattMode = ChattMode.All;
    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        if (s_instance == null)
            s_instance = this;
    }

   public void SendChatting(UICellData chattingData)
    {
        switch(_chattMode)
        {
            case ChattMode.All:
                _allChattingRecycleViewController.AddData(chattingData);
                _allChattingRecycleViewController.UpdateMyData();
                break;
            case ChattMode.Team:
                _teamChattingRecycleViewController.AddData(chattingData);
                _teamChattingRecycleViewController.UpdateMyData();
                break;
        }
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
