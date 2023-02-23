using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PokerExit : MonoBehaviour
{
    [SerializeField] GameObject _exitAlertWindow;
    [SerializeField] Button _exitButton;

    private void Start()
    {
        _exitButton.onClick.AddListener(()=> ActiveAlertWindow());
        _exitButton.onClick.AddListener(() => Exit());
    }

    public void ActiveAlertWindow()
    {
        _exitAlertWindow.SetActive(true);
    }
    public void InActiveAlertWindow()
    {
        _exitAlertWindow.SetActive(false);
    }

    public void Exit()
    {
        //초기자금 확인하고,
        //자금이 있는상태에서 나간다면 패배1 적립
        //자금이 없는 상태에서 나간다면 그냥 나가기. => 근데 어차피 패배자나??
        //로비화면으로 이동
       
    }

}
