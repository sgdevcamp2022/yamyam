using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokerWindowController : MonoBehaviour
{
    // Start is called before the first frame update
    private static PokerWindowController s_instance = null;
    public static PokerWindowController Instance
    { get => s_instance; }

    [SerializeField] private GameObject _looseWindow;
    [SerializeField] private GameObject _winWindow;


    private void Awake()
    {
        _init();
    }

    private void _init()
    {
        if (s_instance == null)
            s_instance = this;
    }

    public void ActiveLooseWindow()
    {
        _looseWindow.SetActive(true);
    }
    public void ActiveWinWindow()
    {
        _winWindow.SetActive(true);
    }

    public void ClickedResultCheckButton()
    {
        PokerGameManager.Instance.ReceiveSocketFlag = true;
        PokerGameManager.Instance._pokerGameState = PokerGameState.LOBBY;
    }
}
