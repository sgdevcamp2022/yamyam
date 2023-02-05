using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Player
{
    string _name;
    int _chip;

    public void SettingPlayer(string name, int chip)
    {
        _name = name;
        _chip = chip;
    }
}

public class PokerGameManager : MonoBehaviour
{
    private static PokerGameManager s_instance = null;
    public static PokerGameManager Instance { get => s_instance; }

    [SerializeField] float _turnTiem = 10f;
    [SerializeField] private UITurn _turn;
    private int _peopleNum = 4;
    public int PeopleNum { get => _peopleNum; }
    private bool _isBattingFinish = false;
    public bool IsBattingFinish { get => _isBattingFinish; }
    private List<Player> _playerOrder = new List<Player>();
    public float TurnTime { get => _turnTiem; }
    private int _order = 4;
    private int _gameProcessNum = 0;



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

        for (int i = 0; i < PeopleNum; i++)
        {
            _playerOrder.Add(new Player());
        }         
        _playerOrder[0].SettingPlayer("�ȳȳȳȳȳ�", 90);
        _playerOrder[1].SettingPlayer("���", 90);
       // _playerOrder[2].SettingPlayer("����", 90);
       // _playerOrder[3].SettingPlayer("����", 90);

        Batting.Instance.SettingRoundBatting(_peopleNum * 10);
    }

    /*[0] �� ������ �ڱ��ڽ�. [1] �� �ڱ� �ٷ� ��������. [2]�� �״ٴ��� [3]�� �״ٴٴ���
    public void SettingPlayerOrder()
    {

    }*/

    public void FinishTurn()
    {
        _isBattingFinish = true;
        _turn.ClearTurnUI();
        if ((_order + 1) % _peopleNum == 0)
        {
            _gameProcessNum++;
            Debug.Log("Ƚ�� �þ. ���� : " + _gameProcessNum);
        }
        _order++;

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
            NextTurn();
        }
        else
        {            
            Change3DGame();        
        }
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
}
