using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITurn : MonoBehaviour
{
    [SerializeField] private Image[] _playersTurnUI;
    [SerializeField] private GameObject[] _playersTurnObject;
    private IEnumerator _battingTurn;
    private IEnumerator _turnWait;
    private int _nowTurn = 0;
    private float _turnTime;
    private float _currentTime;
    private float _startTime;


    public void StartTurn(int num)
    {
        _nowTurn = num;
        Init();

        StartCoroutine(_turnWait);
    }

    IEnumerator TurnWait()
    {
        StartCoroutine(_battingTurn);
        yield return new WaitUntil(()=> PokerGameManager.Instance.IsBattingFinish);
        _playersTurnUI[_nowTurn].gameObject.SetActive(false);

        FinishTurn();
    }

    IEnumerator Turn()
    {
        _turnTime = PokerGameManager.Instance.TurnTime;
        _startTime = Time.time;
        while (true)
        {          
            CheckTime();
            yield return new WaitForSeconds(0.1f);
        }
    }

    void  CheckTime()
    {
        _currentTime = Time.time - _startTime;
        if(_currentTime < _turnTime)
        {
            SetFillAmout(_turnTime - _currentTime);
        }
        else
        {
            FinishTurn();
            Batting.Instance.Call();         
        }
    }

    void SetFillAmout(float value)
    {
        _playersTurnUI[_nowTurn].fillAmount = value / _turnTime;
    }

    void Init()
    {
        _playersTurnUI[_nowTurn].gameObject.SetActive(true);
        _playersTurnUI[_nowTurn].fillAmount = 1f;

        _turnWait = TurnWait();
        _battingTurn = Turn();

        SettingPeople();
    }     
    

    void SettingPeople()
    {
        switch (PokerGameManager.Instance.PeopleNum)
        {
            case 3:
                _playersTurnObject[1].SetActive(true);
                break;
            case 4:
                _playersTurnObject[1].SetActive(true);
                _playersTurnObject[3].SetActive(true);
                break;
        }     
    }

    void FinishTurn()
    {
        StopCoroutine(_battingTurn);
        StopCoroutine(_turnWait);
       
       _playersTurnUI[_nowTurn].gameObject.SetActive(false);
    }    

    public void ClearTurnUI()
    {
        _playersTurnUI[_nowTurn].gameObject.SetActive(false);
    }
}
