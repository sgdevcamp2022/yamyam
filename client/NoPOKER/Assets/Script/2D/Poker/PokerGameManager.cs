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
    private int _myOrder;
    private int _myID = 3;
    private int _startPoint;
    private int _callNum = 0;
    private int _alivePeople;

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
        _turn.StartTurn(NowTurn);
    }


    public void Init()
    {
        if (s_instance == null)
            s_instance = this;

        DontDestroyOnLoad(this);
    }

    public void SettingGame()
    {

        /*
        _playerNames.Add("냠냠냠냠냠냠");
        _playerNames.Add("얌얌");
        _playerNames.Add("쩝쩝");
        _playerNames.Add("뇸뇸");


        for (int i = 0; i < PeopleNum; i++)
        {
            PlayerOrder.Add(new PokerPlayer(_playerNames[i],90));
        }    */
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
        //test : 4���̶�� ���
        PlayerOrder.Add(new PokerPlayer(1, "�ȳȳȳ�",90));
        PlayerOrder.Add(new PokerPlayer(2, "����",90));
        PlayerOrder.Add(new PokerPlayer(3, "��農",90));
        PlayerOrder.Add(new PokerPlayer(4, "�����",90));
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
        /*  ����
         *  1. �޾ƿ� ���� �� �̸��� ����ִ��� ã��, ��(ex. 0)�� ����ϱ�. , _playerNames.Add(���̸�)
            2. �� ����+1���� (1~) ������� ����  for��� ����.
            3. �� ����� _playerNames.Add ��.
            4. �� 0���� �� ���ڱ��� for��� ����.
            5. �� ����� _playerNames.Add ��.
            �׷�, ����0���� �ڸ��� ���, ������ ����� �׵ڷ� ����.
            �̷��� �ڸ���ġ�� �ϵ�����.
            �׷��� �� 2���� ����Ʈ�� ���.
            �Ѱ���, ���� �÷��̾� ��  PokerOrder
            �ٸ� �Ѱ��� , ���� �߽���� �� ��ġ��. playerNames
         */
        FindMyName();
        if (_myOrder != _peopleNum - 1) //���� ���� �� �ƴ϶��
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
