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
            //time ����.
            //�ڵ����� �� �ϰ�, ���� �����ٰ�˸�.
           
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

        Debug.Log("�� �������ϴ�~");
    }    

    public void ClearTurnUI()
    {
        _playersTurnUI[_nowTurn].gameObject.SetActive(false);
    }

}
