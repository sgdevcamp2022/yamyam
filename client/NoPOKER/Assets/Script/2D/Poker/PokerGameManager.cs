using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattingState
{
    die,
    batting,
    call,
}

public enum PokerGameState {

    FOCUS,
    RRESULT,
    BET,
    DIE,
    OPEN
}

public class PokerPlayer
{
    public int _id;
    public string _name;
    int _chip;
    public int _card;
    BattingState _state = BattingState.batting;

    public PokerPlayer(int id, string name, int chip)
    {
        _id = id;
        _name = name;
        _chip = chip;
    }

    public PokerPlayer(int id, string name, int chip, int card)
    {
        _id = id;
        _name = name;
        _chip = chip;
        _card = card;
    }

    public void SetState(BattingState state)
    {
        _state = state;
    }
    public BattingState GetState()
    {
        return _state;
    }

    public void AddChip(int chip)
    {
        _chip += chip;
    }
    public void SubChip(int chip)
    {
        _chip -= chip;
    }

}


public class ReceivedBattingSocketData {
    public int gameId;
    public int id;
    public int betAmout;
    public int totalAmount;
    public int currentAmount;
   
}


public class PokerGameManager : MonoBehaviour
{
    private static PokerGameManager s_instance = null;
    public static PokerGameManager Instance { get => s_instance; }

    [SerializeField] float _turnTiem = 10f;
    [SerializeField] private UITurn _turn;
    [SerializeField] private Card _card;
    private int _peopleNum = 4;
    public int PeopleNum { get => _peopleNum; }
    private bool _isBattingFinish = false;
    public bool IsBattingFinish { get => _isBattingFinish; }
    public List<PokerPlayer> PlayerOrder = new List<PokerPlayer>();
    private List<UserPokertData> _playerUiOrders = new List<UserPokertData>();
    public List<UserPokertData> GetPlayerUiOrders { get => _playerUiOrders; }

    public float TurnTime { get => _turnTiem; }
    public int NowTurnUserId;
    private int _distributeNum = 0;
    public int DistributeNum { get => _distributeNum; }
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
   public  PokerResultPlayerSocketData[] ResultPlayerDatas;
    [SerializeField] GameObject _inactiveBattingButtonView;
    private void Update()
    {
        if(ReceiveSocketFlag)
        {
            switch(_pokerGameState)
            {
                case PokerGameState.FOCUS:
                    Debug.Log("NOW TURN : " + NowTurnUserId);
                    if (NowTurnUserId == UserInfo.Instance.UserID)
                    {
                        Batting.Instance.InActiveButtonView.SetActive(false);
                        if(_playerUiOrders[0].currentChip < Batting.Instance.CallBattingChip)
                        {
                            Batting.Instance.CallorDieState(true);
                        }
                        else
                        {
                            Batting.Instance.CallorDieState(false);
                        }
                     }
                    _turn.StartTurn(NowTurnUserId);
                    break;
                case PokerGameState.BET:
                    //turn 돌아가는거 중단.
                    //BET정보 화면에 반영
                    _turn.FinishTurn();
                    ShowBattingResult();
                    break;
                case PokerGameState.DIE:
                    //turn 돌아가는거 중단.
                    _turn.FinishTurn();
                    ShowDieResult(ResultUserUiPos);
                    break;
                case PokerGameState.OPEN:
                    FinishPokerGame();
                    ShowGameResult();
                    break;
            }

            ReceiveSocketFlag = false;
        }
    }

 
    public void UpDistributeNum()
    {
        _distributeNum++;
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
      //  DontDestroyOnLoad(this);
    }


    public void ShowGameResult()
    {
        for(int i=0;i<_peopleNum;i++)
        {
            int _findIndex = _playerUiOrders.FindIndex(x => x.id == PokerGameSocket.Instance.GetGamePlayersList[i].id);
            _playerUiOrders[_findIndex].currentChip = PokerGameSocket.Instance.GetGamePlayersList[i].currentChip;
            _playerUiOrders[_findIndex].result = PokerGameSocket.Instance.GetGamePlayersList[i].result;
            _uiPokerPlayer.SetUsetChip(_findIndex, _playerUiOrders[_findIndex].currentChip);

            if(_playerUiOrders[_findIndex].result == true)
            {
                SendWinnerMessage(_playerUiOrders[_findIndex].id);
            }
        }
    }

    public void SettingGame()
    {
        GetPlayerOrder();
        SetPlayerPosition();                   

        Batting.Instance.SettingRoundBatting(_peopleNum * 10);
        _card.DistributeCard();
    }

    /*[0] 은 무조건 자기자신. [1] 은 자기 바로 다음차례. [2]는 그다다음 [3]은 그다다다음
    public void SettingPlayerOrder()
    {

    }*/

    public void GetPlayerOrder() //�÷��̾� �� ������
    {
        for(int i=0;i<PokerGameSocket.Instance.GetPokerGamePeopleNum;i++)
        {
        
            PlayerOrder.Add(new PokerPlayer(
                PokerGameSocket.Instance.GetGamePlayersList[i].id,
                PokerGameSocket.Instance.GetGamePlayersList[i].nickname,
                PokerGameSocket.Instance.GetGamePlayersList[i].currentChip,
                PokerGameSocket.Instance.GetGamePlayersList[i].card)
                );
        }
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
        Batting.Instance.Win();
        _turn.ShowWinnerUI(winnerID);
    }

    public void ResetPokerGame()
    {

        //Start();
    }

    public void SetPlayerPosition()
    {
       
        FindMyName();

        
        if (_myOrder != _peopleNum - 1) 
        {
            for (int i = _myOrder + 1; i < _peopleNum; i++)
            {
                _playerUiOrders.Add( new UserPokertData(PlayerOrder[i]._id, PlayerOrder[i]._name , PlayerOrder[i]._card));

            }
        }
        for (int i =0; i < _myOrder; i++)
        {
            if(i==0)
            {
                if(_myOrder != 0)
                    _playerUiOrders.Add(new UserPokertData(PlayerOrder[i]._id, PlayerOrder[i]._name , PlayerOrder[i]._card));

                _startPoint = _playerUiOrders.Count;
            }
            else
            {
                _playerUiOrders.Add(new UserPokertData(PlayerOrder[i]._id, PlayerOrder[i]._name, PlayerOrder[i]._card));
            }
            
        }

        for (int i = 0; i < _playerUiOrders.Count; i++)
            Debug.Log("_playerUiOrders :" + _playerUiOrders[i].nickname);
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

        for(int i=0;i< _playerUiOrders.Count;i++)
        {
            Debug.Log("_playerUiOrders[" + i + "] : " + _playerUiOrders[i].nickname);
        }
    }

    public void FindMyName()
    {
        for(int i=0;i<_peopleNum;i++)
        {
            if(PlayerOrder[i]._id == UserInfo.Instance.UserID)
            {
                Debug.Log("my name is " + PlayerOrder[i]._name);
                _myOrder = i;
                _playerUiOrders.Add(new UserPokertData(PlayerOrder[i]._id, PlayerOrder[i]._name, PlayerOrder[i]._card));
                return;
            }
        }
    }

    public void Change3DGame()
    {
        //3D게임전환.
        Debug.Log("3D게임으로 전환됩니다.");
    }

    public void ChangePlayerState(BattingState state)
    {
        PlayerOrder[NowTurnUserId].SetState(state);
    }

    public void ShowBattingResult()
    {
       if(receivedBattingInfo.betAmout == Batting.Instance.CallBattingChip) //콜을 한 경우.
        {
            Debug.Log("상대방이 call을 했네요");
            Batting.Instance.OtherCall();
            Batting.Instance.SetRoundBatting(receivedBattingInfo.totalAmount, receivedBattingInfo.betAmout);
            _uiPokerPlayer.SetUsetChip(ResultUserUiPos, receivedBattingInfo.currentAmount);
        }
       else
        {
            Debug.Log("상대방이 RAISE을 했네요");
            Batting.Instance.OtherRaise(receivedBattingInfo.betAmout);
            Batting.Instance.SetRoundBatting(receivedBattingInfo.totalAmount, receivedBattingInfo.betAmout);
            _uiPokerPlayer.SetUsetChip(ResultUserUiPos, receivedBattingInfo.currentAmount);

        }
    }
    public void ShowDieResult(int userID)
    {
        Batting.Instance.OtherDie(userID);
    }
}
