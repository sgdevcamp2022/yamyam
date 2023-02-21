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
    RRESULT
}

public class PokerPlayer
{
    public int _id;
    public string _name;
    int _chip;
    BattingState _state = BattingState.batting;

    public PokerPlayer(int id, string name, int chip)
    {
        _id = id;
        _name = name;
        _chip = chip;
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
    private List<string> _playerNames = new List<string>();
    
    public float TurnTime { get => _turnTiem; }
    private int _order = 4;
    public int NowTurn;
    private int _gameProcessNum = 0;
    private int _distributeNum = 0;
    public int DistributeNum { get => _distributeNum; }
    private int _dieNum = 0;
    private bool _pokerFinish = false;
    public bool PokerFinish { get => _pokerFinish; }
    [SerializeField] UIPokerPlayer _uiPokerPlayer;
    private int _myOrder;
    private int _myID = 3;
    private int _startPoint;
    private int _callNum = 0;
    private int _alivePeople;
    public bool ReceiveSocketFlag = false;
    
    public PokerGameState _pokerGameState;


    private void Update()
    {
        if(ReceiveSocketFlag)
        {
            switch(_pokerGameState)
            {
                case PokerGameState.FOCUS:
                    _turn.StartTurn(NowTurn);
                    break;

            }

            ReceiveSocketFlag = false;
        }
    }

    public void UpCallNum()
    {
        _callNum++;
    }
    
    public void ResetCallNum()
    {
        _callNum = 0;
    }

    public void UpDieNum()
    {
        _dieNum++;
        _alivePeople--;
    }

    public void UpDistributeNum()
    {
        _distributeNum++;
    }

    private void Awake()
    {
        Init();
   
    }
    private void Start()
    {
        SettingGame();
       // _turn.StartTurn(NowTurn);
       // _turn.StartTurn(_order% _peopleNum);

    }


    public void Init()
    {
        if (s_instance == null)
            s_instance = this;

        _peopleNum = PokerGameSocket.Instance.GetPokerGamePeopleNum;
        DontDestroyOnLoad(this);
    }

    public void SettingGame()
    {
        _alivePeople = _peopleNum;
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
                PokerGameSocket.Instance.GetGamePlayersList[i].currentChip)
                );
        }
    }



    public void FinishTurn()
    {
       
        if (_dieNum == _peopleNum-1) // 한명빼고 모두가 다이를 했을 경우,
        {
            Debug.Log("die num = " + _dieNum);
            FinishPokerGame();
            AllDie();
            //본인 카드 공개 되면서, 베팅된칩 그 한명에게로 몰빵           
        }
        else if(_callNum == _alivePeople) //��� �ִ� ��ΰ� ��� ��� ��� 
        {
            FinishPokerGame();
            Change3DGame();
        }
        else
        {
            _isBattingFinish = true;
            _turn.ClearTurnUI();
            Debug.Log("Finish Turn");
          //  _order++;
            if (NowTurn == _startPoint-1)
            {
                _gameProcessNum++;
                Debug.Log("횟수 늘어남. 현재 : " + _gameProcessNum);
            }

            if (_gameProcessNum < 3)
            {
                //서버통신 : 턴이 끝났다고 알림. 
                /*서버통신 : 서버로부터 받아온 정보로 
                 * 모두 콜을 했을 경우 화면전환
                 * 모두 다이를 했을경우 결과 보여줌
                 * 
                Debug.Log("현재 order: " + _order);
                if () //모든 인원이 Call을 할 경우 
                {
                    Change3DGame();
                }
                else if() //1명을 제외한 모든사람이 Die를 할 경우
                {
                    //다음사람이 돈을 다 가지는걸로 !
                }
                */
                _isBattingFinish = false;
                //따로 진행되는게 없다면 다음 턴으로 진행할 수 있도록함.
                if (PlayerOrder[NowTurn].GetState() == BattingState.die)
                {
                    FinishTurn();
                }
                else
                {
                    NextTurn();
                }

            }
            else
            {
                Change3DGame();
            }
        }

    }

    public void FinishPokerGame()
    {
        _pokerFinish = true;
        _turn.StopAllTurn();
        _card.OpenMyCard();
    }

    public void AllDie()
    {
        _card.OpenMyCard();
        SendWinnerMessage();
    }

    public void SendWinnerMessage()
    {
        Batting.Instance.Win();
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
                _playerNames.Add(PlayerOrder[i]._name);
            }
        }
        for (int i =0; i < _myOrder; i++)
        {
            if(i==0)
            {
                if(_myOrder != 0)
                _playerNames.Add(PlayerOrder[i]._name);

                _startPoint = _playerNames.Count;
            }
            else
            {
                _playerNames.Add(PlayerOrder[i]._name);
            }
            
        }
      //  _order = (_peopleNum + _startPoint - 1);


        for (int i = 0; i < _playerNames.Count; i++)
        {
            Debug.Log("order " + i + " : " + _playerNames[i]);
            _uiPokerPlayer.SetUserName(i, _playerNames[i]);
        }

    }

    public void FindMyName()
    {
        for(int i=0;i<_peopleNum;i++)
        {
            if(PlayerOrder[i]._id == _myID)
            {
                Debug.Log("my name is " + PlayerOrder[i]._name);
                _myOrder = i;
                _playerNames.Add(PlayerOrder[i]._name);
                return;
            }
        }
    }

    public void Change3DGame()
    {
        //3D게임전환.
        Debug.Log("3D게임으로 전환됩니다.");
    }

    public void NextTurn()
    { 
        _turn.StartTurn(NowTurn);
    }

    public void ChangePlayerState(BattingState state)
    {
        PlayerOrder[NowTurn].SetState(state);
    }


}
