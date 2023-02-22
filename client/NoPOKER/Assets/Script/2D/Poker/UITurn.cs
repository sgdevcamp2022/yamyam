using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITurn : MonoBehaviour
{
    [SerializeField] private Image[] _playersTurnUI;
    [SerializeField] private GameObject[] _playersTurnObject;
    [SerializeField] private GameObject[] _playersWinObject;
    private IEnumerator _battingTurn;
    private IEnumerator _turnWait;
    private int _nowTurnUser = 0;
    private float _turnTime;
    private float _currentTime;
    private float _startTime;
    private bool _isFirst = true;
    bool _stopTimer = false;
    private int _nowUiPos;
    public void StartTurn(int userId) 
    {
        _nowTurnUser = userId;
        Init();

        StartCoroutine(_turnWait);
    }

    IEnumerator TurnWait()
    {   if (_isFirst)
        {
            yield return new WaitUntil(() => PokerGameManager.Instance.DistributeNum== PokerGameManager.Instance.PeopleNum);
            _isFirst = false;
        }      
        StartCoroutine(_battingTurn);
        yield return new WaitUntil(()=> PokerGameManager.Instance.IsBattingFinish);
        _playersTurnUI[_nowUiPos].gameObject.SetActive(false);
        PokerGameManager.Instance.StartTurn();

        FinishTurn();
    }

    IEnumerator Turn()
    {
        _turnTime = PokerGameManager.Instance.TurnTime;
        _startTime = Time.time;
        _stopTimer = false;
        while (true)
        {          
            CheckTime();
            yield return new WaitForSeconds(0.1f);
            if (_stopTimer)
                break;
        }
    }

    void  CheckTime()
    {
        if (PokerGameManager.Instance.PokerFinish)
        {
            _stopTimer = true;
            FinishTurn();
            return;
        }           
        _currentTime = Time.time - _startTime;
        if(_currentTime < _turnTime)
        {
            SetFillAmout(_turnTime - _currentTime);
        }
        else
        {
            
            FinishTurn();
            if(PokerGameManager.Instance.NowTurnUserId == UserInfo.Instance.UserID)
            Batting.Instance.Die();         
        }
    }

    void SetFillAmout(float value)
    {
        _playersTurnUI[_nowUiPos].fillAmount = value / _turnTime;
    }

    void Init()
    {
         _nowUiPos = PokerGameManager.Instance.GetPlayerUiOrders.FindIndex(x => x.id == _nowTurnUser);
        PokerGameManager.Instance.UiPos = _nowUiPos;
        Debug.Log("_NOWUIPOS = " + _nowUiPos);
        //반복문돌면서 현재 유저가 어디있는지 찾아야함.
        if(PokerGameManager.Instance.PeopleNum == 2)
        { //02020202
            if(_nowUiPos ==1)
            {
                _nowUiPos = 2;
            }
        }
        _playersTurnUI[_nowUiPos].gameObject.SetActive(true);
        _playersTurnUI[_nowUiPos].fillAmount = 1f;
        
        _turnWait = TurnWait();
        _battingTurn = Turn();

        SettingPeople();
    }     
    
    public void ShowWinnerUI(int who)
    {

        StartCoroutine(WinnerUI(who));
    }

    IEnumerator WinnerUI(int winner)
    {
        for(int i=0;i<3;i++)
        {
            _playersWinObject[winner].SetActive(true);
            yield return new WaitForSeconds(0.2f);
            _playersWinObject[winner].SetActive(false);
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void HideWinnerUI()
    {
        for(int i=0;i<4;i++)
        _playersWinObject[i].SetActive(true);
    }

    void SettingPeople()
    {
        switch (PokerGameManager.Instance.PeopleNum)
        {
            case 2:
                _playersTurnObject[1].SetActive(false);
                _playersTurnObject[3].SetActive(false);
                break;

            case 3:
                _playersTurnObject[1].SetActive(true);
                _playersTurnObject[3].SetActive(false);
                break;
            case 4:
                _playersTurnObject[1].SetActive(true);
                _playersTurnObject[3].SetActive(true);
                break;
        }     
    }

    public void FinishTurn()
    {
        _stopTimer = true;

        StopCoroutine(_battingTurn);
        StopCoroutine(_turnWait);
       
       _playersTurnUI[_nowUiPos].gameObject.SetActive(false);
     
    }    

    public void StopAllTurn()
    {
        StopCoroutine(_battingTurn);
        StopCoroutine(_turnWait);

        _playersTurnUI[_nowUiPos].gameObject.SetActive(false);
    }

    public void ClearTurnUI()
    {
        _playersTurnUI[_nowUiPos].gameObject.SetActive(false);
    }
}
