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
    string _name;
    int _chip;
    PokerState _state = PokerState.batting;

    public PokerPlayer(string name, int chip)
    {
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
    public int NowTurn { get => _order % PeopleNum; }
    private int _gameProcessNum = 0;
    private int _distributeNum = 0;
    public int DistributeNum { get => _distributeNum; }
    private int _dieNum = 0;
    private bool _pokerFinish = false;
    public bool PokerFinish { get => _pokerFinish; }


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
        _turn.StartTurn(_order% _peopleNum);
    }


    public void Init()
    {
        if (s_instance == null)
            s_instance = this;
    }

    public void SettingGame()
    {
        _playerNames.Add("�ȳȳȳȳȳ�");
        _playerNames.Add("���");
        _playerNames.Add("����");
        _playerNames.Add("����");

        for (int i = 0; i < PeopleNum; i++)
        {
            PlayerOrder.Add(new PokerPlayer(_playerNames[i],90));
        }         
        Batting.Instance.SettingRoundBatting(_peopleNum * 10);

        _card.DistributeCard();
    }

    /*[0] �� ������ �ڱ��ڽ�. [1] �� �ڱ� �ٷ� ��������. [2]�� �״ٴ��� [3]�� �״ٴٴ���
    public void SettingPlayerOrder()
    {

    }*/

    public void FinishTurn()
    {
       
        if (_dieNum == _peopleNum-1) // �Ѹ��� ��ΰ� ���̸� ���� ���,
        {
            Debug.Log("die num = " + _dieNum);
            FinishPokerGame();
            AllDie();
            //���� ī�� ���� �Ǹ鼭, ���õ�Ĩ �� �Ѹ��Է� ����           
        }
        else
        {
            _isBattingFinish = true;
            _turn.ClearTurnUI();
            Debug.Log("Finish Turn");
            if ((_order + 1) % _peopleNum == 0)
            {
                _gameProcessNum++;
                Debug.Log("Ƚ�� �þ. ���� : " + _gameProcessNum);
            }
            _order++;


            Debug.Log("In else" );
            if (_gameProcessNum < 3)
            {
                //������� : ���� �����ٰ� �˸�. 
                /*������� : �����κ��� �޾ƿ� ������ 
                 * ��� ���� ���� ��� ȭ����ȯ
                 * ��� ���̸� ������� ��� ������
                 * 
                Debug.Log("���� order: " + _order);
                if () //��� �ο��� Call�� �� ��� 
                {
                    Change3DGame();
                }
                else if() //1���� ������ ������� Die�� �� ���
                {
                    //��������� ���� �� �����°ɷ� !
                }
                */
                _isBattingFinish = false;
                //���� ����Ǵ°� ���ٸ� ���� ������ ������ �� �ֵ�����.
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



    public void Change3DGame()
    {
        //3D������ȯ.
        Debug.Log("3D�������� ��ȯ�˴ϴ�.");
    }

    public void NextTurn()
    { 
        _turn.StartTurn(_order % _peopleNum);
    }

    public void ChangePlayerState(PokerState state)
    {
        PlayerOrder[NowTurn].SetState(state);
    }


}
