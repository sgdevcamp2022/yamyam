using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITurn : MonoBehaviour
{
    [SerializeField] private Image[] _playersTurnUI;
    IEnumerator _battingTurn;
    IEnumerator _turnWait;
    int _nowTurn = 0;
    float _turnTime;
    float _currentTime;
    float _startTime;


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
            //time 끝남.
            //자동으로 콜 하고, 턴을 끝낸다고알림.
           
            FinishTurn();
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
    }    
    

    void FinishTurn()
    {
        StopCoroutine(_battingTurn);
        StopCoroutine(_turnWait);
       

        _playersTurnUI[_nowTurn].gameObject.SetActive(false);
       PokerGameManager.Instance.FinishTurn();

        Debug.Log("턴 끝났씁니다~");
    }    

    public void ClearTurnUI()
    {
        _playersTurnUI[_nowTurn].gameObject.SetActive(false);
    }

}
