using System.Collections.Generic;
using UnityEngine;

public enum BattingState
{
    die,
    batting,
    call,
}

public enum PokerGameState
{

    FOCUS,
    RESULT,
    BET,
    DIE,
    OPEN,
    LOBBY,
    CALL,
    RAISE,
    ALLIN,
    NONE
}


public class ReceivedBattingSocketData
{
    public int gameId;
    public int id;
    public int betAmout;
    public int totalAmount;
    public int currentAmount;

}


public class PokerGameManager : MonoBehaviour
{

    private List<PokerUserSocketData> _PlayerInfos = new List<PokerUserSocketData>();


    private static PokerGameManager s_instance = null;
    public static PokerGameManager Instance { get => s_instance; }

    [SerializeField] float _turnTiem = 10f;
    [SerializeField] private UITurn _turn;
    [SerializeField] private Card _card;
    private int _peopleNum = 4;
    public int PeopleNum { get => _peopleNum; }
    private bool _isBattingFinish = false;
    public bool IsBattingFinish { get => _isBattingFinish; }

    private List<UserPokerData> _playerUiOrders = new List<UserPokerData>();
    public List<UserPokerData> GetPlayerUiOrders { get => _playerUiOrders; }

    public float TurnTime { get => _turnTiem; }
    public int NowTurnUserId;
    private bool _isDistributed = false;
    public bool IsDistributed { get => _isDistributed; }
    private int _dieNum = 0;
    private bool _pokerFinish = false;
    public bool PokerFinish { get => _pokerFinish; }
    [SerializeField] UIPokerPlayer _uiPokerPlayer;
    private int _myOrder;
    private int _startPoint;
    private int _callNum = 0;

    public bool ReceiveSocketFlag = false;

    public ReceivedBattingSocketData receivedBattingInfo = new ReceivedBattingSocketData();
    public PokerGameState _pokerGameState;
    public int UiPos;
    public int ResultUserUiPos;
    public bool IsFirstTurn = false;
    public PokerResultPlayerSocketData[] ResultPlayerDatas;
    [SerializeField] GameObject _inactiveBattingButtonView;
    private bool _isSetting = true;


    private void Update()
    {
        if (ReceiveSocketFlag)
        {
            switch (_pokerGameState)
            {
                case PokerGameState.FOCUS:


                    Debug.Log("NOW TURN : " + NowTurnUserId);
                    SetUserInfo();
                    if (NowTurnUserId == UserInfo.Instance.UserID)
                    {
                        //Batting.Instance.SetRoundBatting(PokerGameSocket.Instance.TotalBetAmount, 0);
                        Batting.Instance.InActiveButtonView.SetActive(false);
                        if (_playerUiOrders[0].currentChip < Batting.Instance.CallBattingChip)
                        {
                            Batting.Instance.CallorDieState(true);
                        }

                        if (IsFirstTurn)
                        {
                            Batting.Instance.CanCallState(false);
                        }
                        else
                        {
                            Batting.Instance.CanCallState(true);
                        }
                        IsFirstTurn = false;
                    }
                    else
                    {
                        IsFirstTurn = false;
                        Batting.Instance.InActiveButtonView.SetActive(true);
                    }
                    _turn.StartTurn();



                    break;

                case PokerGameState.BET:
                    //turn 돌아가는거 중단.
                    _turn.FinishTurn();
                    //BET정보 화면에 반영

                    ShowBattingResult();
                    break;

                case PokerGameState.CALL:
                    UpdateUserInfo();
                    _turn.FinishTurn();
                    Batting.Instance.SetCallBattingChip(receivedBattingInfo.betAmout);
                    ShowCallResult();
                    break;

                case PokerGameState.RAISE:
                    UpdateUserInfo();
                    _turn.FinishTurn();
                    Batting.Instance.SetCallBattingChip(receivedBattingInfo.betAmout);
                    ShowRaiseResult();
                    break;
                case PokerGameState.ALLIN:
                    UpdateUserInfo();
                    _turn.FinishTurn();

                    break;


                case PokerGameState.DIE:
                    //turn 돌아가는거 중단.
                    _turn.FinishTurn();
                    ShowDieResult(ResultUserUiPos);
                    break;

                case PokerGameState.LOBBY:
                    PokerGameSocket.Instance.DisconnectSever();
                    GameManager.Instance.ChangeScene(Scenes.LobbyScene);
                    break;

                    case PokerGameState.RESULT:
                    FinishPokerGame();
                    ShowGameResult();
                    break;

            }

            ReceiveSocketFlag = false;
            _pokerGameState = PokerGameState.NONE;
        }
    }


    public void FinishDistribute()
    {
        _isDistributed = true;
    }


    private void Awake()
    {
        Init();

    }

    public void StartPokerGame()
    {
        _peopleNum = PokerGameSocket.Instance.GetPokerGamePeopleNum;
        SettingGame();
    }

    public void Init()
    {
        if (s_instance == null)
            s_instance = this;
    }


    public void ShowGameResult()
    {
        for (int i = 0; i < _peopleNum; i++)
        {
            int _findIndex = _playerUiOrders.FindIndex(x => x.id == PokerGameSocket.Instance.GetGamePlayersList[i].id);

            _playerUiOrders[_findIndex].currentChip = PokerGameSocket.Instance.GetGamePlayersList[i].currentChip;
            _playerUiOrders[_findIndex].result = PokerGameSocket.Instance.GetGamePlayersList[i].result;
            _uiPokerPlayer.SetUserChip(_findIndex, _playerUiOrders[_findIndex].currentChip);

            if (_findIndex == 0)
            {
                Batting.Instance.SetMyBattingChip(_playerUiOrders[_findIndex].currentChip);
            }

            if (_playerUiOrders[_findIndex].result == true)
            {
                SendWinnerMessage(_playerUiOrders[_findIndex].id);
            }
        }

    }


    public void SetUserInfo()
    {
        for (int i = 0; i < _peopleNum; i++)
        {
            int _findIndex = _playerUiOrders.FindIndex(x => x.id == PokerGameSocket.Instance.GetGamePlayersList[i].id);
            _playerUiOrders[_findIndex].currentChip = PokerGameSocket.Instance.GetGamePlayersList[i].currentChip;
            _playerUiOrders[_findIndex].result = PokerGameSocket.Instance.GetGamePlayersList[i].result;
            _uiPokerPlayer.SetUserChip(_findIndex, _playerUiOrders[_findIndex].currentChip);
        }
    }

    public void UpdateUserInfo()
    {
        _playerUiOrders[ResultUserUiPos].currentChip = receivedBattingInfo.currentAmount;

        if (_peopleNum == 2)
        {
            _uiPokerPlayer.SetUserChip(1, _playerUiOrders[ResultUserUiPos].currentChip);
        }
        else
        {
            _uiPokerPlayer.SetUserChip(ResultUserUiPos, _playerUiOrders[ResultUserUiPos].currentChip);
        }
    }

    public UserPokerData GetUserInfo()
    {
        return _playerUiOrders[ResultUserUiPos];
    }


    public void SettingGame()
    {
        IsFirstTurn = true;
        SetPlayerPosition();
        _isDistributed = false;

        Batting.Instance.SettingRoundBatting(_peopleNum * 10);
        SetUserInfo();

        _card.DistributeCard();
        _turn.SettingPeople();

        _pokerGameState = PokerGameState.FOCUS;
        ReceiveSocketFlag = true;

    }


    public void FinishTurn()
    {
        _isBattingFinish = true;
        _turn.ClearTurnUI();
    }

    public void StartTurn()
    {
        _isBattingFinish = false;
    }
    public void FinishPokerGame()
    {
        _pokerFinish = true;
        _turn.StopAllTurn();
        _card.OpenMyCard();
    }

    public void SendWinnerMessage(int winnerID)
    {
        int _findIndex = _playerUiOrders.FindIndex(x => x.id == winnerID);
        Batting.Instance.Win(_findIndex);
        _turn.ShowWinnerUI(_findIndex);
    }

    public void SetPlayerPosition()
    {
        _playerUiOrders.Clear();
        FindMyName();


        if (_myOrder != _peopleNum - 1)
        {
            for (int i = _myOrder + 1; i < _peopleNum; i++)
            {
                _playerUiOrders.Add(new UserPokerData(
                    PokerGameSocket.Instance.GetGamePlayersList[i].id,
                    PokerGameSocket.Instance.GetGamePlayersList[i].nickname,
                    PokerGameSocket.Instance.GetGamePlayersList[i].card));

            }
        }
        for (int i = 0; i < _myOrder; i++)
        {
            if (i == 0)
            {
                if (_myOrder != 0)
                {
                    _playerUiOrders.Add(new UserPokerData(
                    PokerGameSocket.Instance.GetGamePlayersList[i].id,
                    PokerGameSocket.Instance.GetGamePlayersList[i].nickname,
                    PokerGameSocket.Instance.GetGamePlayersList[i].card));
                }

                _startPoint = _playerUiOrders.Count;
            }
            else
            {
                _playerUiOrders.Add(new UserPokerData(
                PokerGameSocket.Instance.GetGamePlayersList[i].id,
                PokerGameSocket.Instance.GetGamePlayersList[i].nickname,
                PokerGameSocket.Instance.GetGamePlayersList[i].card));
            }

        }

        if (_peopleNum == 2)
        {
            _uiPokerPlayer.SetUserName(0, _playerUiOrders[0].nickname);
            _uiPokerPlayer.SetUserName(2, _playerUiOrders[1].nickname);
        }
        else if (_peopleNum == 3)
        {
            _uiPokerPlayer.SetUserName(0, _playerUiOrders[0].nickname);
            _uiPokerPlayer.SetUserName(1, _playerUiOrders[1].nickname);
            _uiPokerPlayer.SetUserName(2, _playerUiOrders[2].nickname);
        }
        else if (_peopleNum == 4)
        {
            _uiPokerPlayer.SetUserName(0, _playerUiOrders[0].nickname);
            _uiPokerPlayer.SetUserName(1, _playerUiOrders[1].nickname);
            _uiPokerPlayer.SetUserName(2, _playerUiOrders[2].nickname);
            _uiPokerPlayer.SetUserName(2, _playerUiOrders[3].nickname);
        }

    }

    public void FindMyName()
    {
        for (int i = 0; i < _peopleNum; i++)
        {
            if (PokerGameSocket.Instance.GetGamePlayersList[i].id == UserInfo.Instance.UserID)
            {
                _myOrder = i;
                _playerUiOrders.Add(new UserPokerData(
                PokerGameSocket.Instance.GetGamePlayersList[i].id,
                PokerGameSocket.Instance.GetGamePlayersList[i].nickname,
                PokerGameSocket.Instance.GetGamePlayersList[i].card));
                return;
            }
        }
    }

    public void Change3DGame()
    {

    }

    public void ShowCallResult()
    {
        Debug.Log("상대방이 call을 했네요");
        Batting.Instance.OtherCall();
        Batting.Instance.ChangeRoundBatting(receivedBattingInfo.totalAmount, receivedBattingInfo.betAmout);
        _uiPokerPlayer.SetUserChip(ResultUserUiPos, receivedBattingInfo.currentAmount);
    }
    public void ShowRaiseResult()
    {
        Debug.Log("상대방이 Raise을 했네요");
        Batting.Instance.OtherRaise(receivedBattingInfo.betAmout);
        Batting.Instance.ChangeRoundBatting(receivedBattingInfo.totalAmount, receivedBattingInfo.betAmout);
        _uiPokerPlayer.SetUserChip(ResultUserUiPos, receivedBattingInfo.currentAmount);
    }
    public void ShowAllinResult()
    {
        Debug.Log("상대방이 All-IN을 했네요");
        Batting.Instance.OtherRaise(receivedBattingInfo.betAmout);
        Batting.Instance.ChangeRoundBatting(receivedBattingInfo.totalAmount, receivedBattingInfo.betAmout);
        _uiPokerPlayer.SetUserChip(ResultUserUiPos, receivedBattingInfo.currentAmount);
    }

    public void ShowBattingResult()
    {
        if (receivedBattingInfo.betAmout == Batting.Instance.CallBattingChip) //콜을 한 경우.
        {
            Debug.Log("상대방이 call을 했네요");
            Batting.Instance.OtherCall();
            Batting.Instance.ChangeRoundBatting(receivedBattingInfo.totalAmount, receivedBattingInfo.betAmout);
            _uiPokerPlayer.SetUserChip(ResultUserUiPos, receivedBattingInfo.currentAmount);
        }
        else
        {
            Debug.Log("상대방이 RAISE을 했네요");
            Batting.Instance.OtherRaise(receivedBattingInfo.betAmout);
            Batting.Instance.ChangeRoundBatting(receivedBattingInfo.totalAmount, receivedBattingInfo.betAmout);
            _uiPokerPlayer.SetUserChip(ResultUserUiPos, receivedBattingInfo.currentAmount);

        }
    }
    public void ShowDieResult(int userID)
    {
        Batting.Instance.OtherDie(userID);
    }
}
