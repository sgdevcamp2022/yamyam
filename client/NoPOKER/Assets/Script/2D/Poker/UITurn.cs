using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITurn : MonoBehaviour
{


    [SerializeField] private Image[] _playersTurnUI;
    [SerializeField] private GameObject[] _2PlayersWinObject;
    [SerializeField] private GameObject[] _4PlayersWinObject;
    [SerializeField] private GameObject[] _2PlayersTurnObject;
    [SerializeField] private Image[] _2PlayersTurnUI;
    [SerializeField] private GameObject[] _4PlayersTurnObject;
    [SerializeField] private Image[] _4PlayersTurnUI;

    private IEnumerator _battingTurn;
    private IEnumerator _turnWait;

    private float _turnTime;
    private float _currentTime;
    private float _startTime;
    private bool _isWaitDistribute = true;
    private bool _stopTimer = false;
    private int _nowUiPos;

    public void StartTurn()
    {
        Init();

        StartCoroutine(_turnWait);
    }

    IEnumerator TurnWait()
    {
        
        if (!PokerGameManager.Instance.IsDistributed)
        {
            yield return new WaitForSeconds(0.5f);
        }


        StartCoroutine(_battingTurn);
        yield return new WaitUntil(() => PokerGameManager.Instance.IsBattingFinish);
        if (PokerGameSocket.Instance.GetPokerGamePeopleNum == 2)
        {
            _2PlayersTurnUI[_nowUiPos].gameObject.SetActive(false);
        }
        else
        {
            _4PlayersTurnUI[_nowUiPos].gameObject.SetActive(false);
        }

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

    void CheckTime()
    {
        if (PokerGameManager.Instance._pokerGameState != PokerGameState.FOCUS)
        {
            _stopTimer = true;
            FinishTurn();
            return;
        }
        _currentTime = Time.time - _startTime;
        if (_currentTime < _turnTime)
        {
            SetFillAmout(_turnTime - _currentTime);
        }
        else
        {
            FinishTurn();
            if (PokerGameManager.Instance.NowTurnUserId == UserInfo.Instance.UserID)
                Batting.Instance.Die();
        }
    }

    void SetFillAmout(float value)
    {
        if (PokerGameSocket.Instance.GetPokerGamePeopleNum == 2)
        {
            _2PlayersTurnUI[_nowUiPos].fillAmount = value / _turnTime;
        }
        else
        {
            _4PlayersTurnUI[_nowUiPos].fillAmount = value / _turnTime;
        }

    }

    void Init()
    {
        _nowUiPos = PokerGameManager.Instance.GetPlayerUiOrders.FindIndex(x => x.id == PokerGameManager.Instance.NowTurnUserId);
        PokerGameManager.Instance.UiPos = _nowUiPos;
        Debug.Log("NOW UI POS : " + _nowUiPos);
        if (PokerGameManager.Instance.PeopleNum == 2)
        {
            _2PlayersTurnUI[_nowUiPos].fillAmount = 1f;
            _2PlayersTurnUI[_nowUiPos].gameObject.SetActive(true);
        }
        else
        {
            _4PlayersTurnUI[_nowUiPos].fillAmount = 1f;
            _4PlayersTurnUI[_nowUiPos].gameObject.SetActive(true);
        }
        _turnWait = TurnWait();
        _battingTurn = Turn();
    }

    public void ShowWinnerUI(int who)
    {

        StartCoroutine(WinnerUI(who));
    }

    IEnumerator WinnerUI(int winner)
    {
        if (PokerGameSocket.Instance.GetPokerGamePeopleNum == 2)
        {
            for (int i = 0; i < 3; i++)
            {
                _2PlayersWinObject[winner].SetActive(true);
                yield return new WaitForSeconds(0.2f);
                _2PlayersWinObject[winner].SetActive(false);
                yield return new WaitForSeconds(0.2f);
            }
        }

        else
        {
            for (int i = 0; i < 3; i++)
            {
                _2PlayersWinObject[winner].SetActive(true);
                yield return new WaitForSeconds(0.2f);
                _2PlayersWinObject[winner].SetActive(false);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    public void HideWinnerUI()
    {
        if (PokerGameSocket.Instance.GetPokerGamePeopleNum == 2)
        {
            for (int i = 0; i < 2; i++)
                _2PlayersWinObject[i].SetActive(false);
        }
        else
        {
            for (int i = 0; i < 4; i++)
                _4PlayersWinObject[i].SetActive(false);
        }

    }

    public void SettingPeople()
    {
        InitPeople();
        switch (PokerGameManager.Instance.PeopleNum)
        {
            case 2:
                for (int i = 0; i < _2PlayersTurnObject.Length; i++)
                    _2PlayersTurnObject[i].SetActive(true);
                break;

            case 3:
                for (int i = 0; i < 3; i++)
                    _4PlayersTurnObject[i].SetActive(true);
                break;
            case 4:
                for (int i = 0; i < 4; i++)
                    _4PlayersTurnObject[i].SetActive(true);
                break;
        }
    }

    void InitPeople()
    {
        for (int i = 0; i < 4; i++)
            _4PlayersTurnObject[i].SetActive(false);
        for (int i = 0; i < 2; i++)
            _2PlayersTurnObject[i].SetActive(false);
    }

    public void FinishTurn()
    {
        _stopTimer = true;

        StopCoroutine(_battingTurn);
        StopCoroutine(_turnWait);
        if (PokerGameManager.Instance.NowTurnUserId == UserInfo.Instance.UserID)
        {
            _2PlayersTurnUI[0].gameObject.SetActive(false);
        }
        else if (PokerGameSocket.Instance.GetPokerGamePeopleNum == 2)
        {
            _2PlayersTurnUI[1].gameObject.SetActive(false);
        }
        else
        {
            _4PlayersTurnUI[PokerGameManager.Instance.ResultUserUiPos].gameObject.SetActive(false);
        }
    }

    public void StopAllTurn()
    {
        StopCoroutine(_battingTurn);
        StopCoroutine(_turnWait);

        if (PokerGameSocket.Instance.GetPokerGamePeopleNum == 2)
        {
            _2PlayersTurnUI[_nowUiPos].gameObject.SetActive(false);
        }
        else
        {
            _4PlayersTurnUI[_nowUiPos].gameObject.SetActive(false);
        }
    }

    public void ClearTurnUI()
    {
        if (PokerGameSocket.Instance.GetPokerGamePeopleNum == 2)
        {
            _2PlayersTurnUI[_nowUiPos].gameObject.SetActive(false);
        }
        else
        {
            _4PlayersTurnUI[_nowUiPos].gameObject.SetActive(false);
        }
    }
}
