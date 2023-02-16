using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PokerState
{
    die,
    batting,
    call
}

public class PokerPlayer
{
    public int _id;
    public string _name;
    int _chip;
    PokerState _state = PokerState.batting;

    public PokerPlayer(int id, string name, int chip)
    {
        _id = id;
        _name = name;
        _chip = chip;
    }

    public void SetState(PokerState state)
    {
        _state = state;
    }
    public PokerState GetState()
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

public class PokerOrder {
    public int PokerUserID;
    public string PokerUserNickName;

    public PokerOrder(int id, string name)
    {
        PokerUserID = id;
        PokerUserNickName = name;
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
    public int NowTurn { get => _order % _peopleNum; }
    private int _gameProcessNum = 0;
    private int _distributeNum = 0;
    public int DistributeNum { get => _distributeNum; }
    private int _dieNum = 0;
    private bool _pokerFinish = false;
    public bool PokerFinish { get => _pokerFinish; }
    [SerializeField] UIPokerPlayer _uiPokerPlayer;
    private List<PokerOrder> _pokerOrder = new List<PokerOrder>();
    private int _myOrder;
    private int _myID = 3;
    private int _startPoint;

    public void UpDieNum()
    {
        _dieNum++;
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
        _turn.StartTurn(NowTurn);
    }


    public void Init()
    {
        if (s_instance == null)
            s_instance = this;
    }

    public void SettingGame()
    {
        /*
        _playerNames.Add("냠냠냠냠냠냠");
        _playerNames.Add("얌얌");
        _playerNames.Add("스구식탁");
        _playerNames.Add("뇸뇸");

        for (int i = 0; i < PeopleNum; i++)
        {
            PlayerOrder.Add(new PokerPlayer(_playerNames[i],90));
        }    */
        GetPlayerOrder();
        SetPlayerPosition();

        Batting.Instance.SettingRoundBatting(_peopleNum * 10);

        _card.DistributeCard();
    }

    /*[0] 은 무조건 자기자신. [1] 은 자기 바로 다음차례. [2]는 그다다음 [3]은 그다다다음
    public void SettingPlayerOrder()
    {

    }*/

       public void GetPlayerOrder() //플레이어 순서 가져오기
    {
        //test : 4명이라는 가정
        PlayerOrder.Add(new PokerPlayer(1, "냠냠냠냠",90));
        PlayerOrder.Add(new PokerPlayer(2, "뇸뇸",90));
        PlayerOrder.Add(new PokerPlayer(3, "욤욤쓰",90));
        PlayerOrder.Add(new PokerPlayer(4, "얌얌얌얌",90));
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
        else
        {
            _isBattingFinish = true;
            _turn.ClearTurnUI();
            Debug.Log("Finish Turn");
            _order++;
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
                if (PlayerOrder[NowTurn].GetState() == PokerState.die)
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
        /*  로직
         *  1. 받아온 순서에서 내 이름이 어디있는지 찾고, 순서(ex. 0)를 기억하기. , _playerNames.Add(내이름)
            2. 내 숫자+1부터 (1~) 사람수까지 도는  for문을 시작.
            3. 그 사람들을 _playerNames.Add 함.
            4. 순서 0부터 내 숫자까지 for문을 돌림.
            5. 그 사람들을 _playerNames.Add 함.
            그럼, 나는0부터 자리를 잡고, 나머지 사람들은 그뒤로 쭉쭉.
            이렇게 자리배치를 하도록함.
            그러면 총 2개의 리스트가 생김.
            한개는, 게임 플레이어 순서  PokerOrder
            다른 한개는 , 나를 중심으로 한 배치순서. playerNames
         */
        FindMyName();
        if (_myOrder != _peopleNum - 1) //내가 마지막 순서가 아니라면
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
        _order = (_peopleNum + _startPoint - 1);


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

    public void ChangePlayerState(PokerState state)
    {
        PlayerOrder[NowTurn].SetState(state);
    }


}
