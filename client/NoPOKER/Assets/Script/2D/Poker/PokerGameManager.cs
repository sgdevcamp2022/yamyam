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
    
    private int _peopleNum = 4;
    private string _myname = "냠냠냠냠냠냠";
    public int PeopleNum { get => _peopleNum; }
    private List<Player> _playerOrder = new List<Player>();
    [SerializeField] float _turnTiem = 10f;
    public float TurnTime { get => _turnTiem; }
    private int _gameProcessNum = 0;
    private int _order = 4;
    private bool _isBattingFinish = false;
    public bool IsBattingFinish { get => _isBattingFinish; }

    [SerializeField] private UITurn _turn; 

    private void Awake()
    {
        Init();
    }
    private void Start()
    {
        SettingGame();
        _turn.StartTurn(_order%4);
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
        _playerOrder[0].SettingPlayer("냠냠냠냠냠냠", 90);
        _playerOrder[1].SettingPlayer("얌얌", 90);
        _playerOrder[2].SettingPlayer("쩝쩝", 90);
        _playerOrder[3].SettingPlayer("뇸뇸", 90);

        Batting.Instance.SettingRoundBatting(_peopleNum * 10);
    }
    /*[0] 은 무조건 자기자신. [1] 은 자기 바로 다음차례. [2]는 그다다음 [3]은 그다다다음
    public void SettingPlayerOrder()
    {

    }*/
    public void FinishTurn()
    {
        _isBattingFinish = true;
        _turn.ClearTurnUI();
        if ((_order + 1) % 4 == 0)
        {
            _gameProcessNum++;
            Debug.Log("횟수 늘어남. 현재 : " + _gameProcessNum);
        }
        _order++;

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
            NextTurn();
        }
        else
        {
            
            Change3DGame();
           
        }
    }

    public void Change3DGame()
    {
        //3D게임전환.
        Debug.Log("3D게임으로 전환됩니다.");
    }

    public void NextTurn()
    {
        _turn.StartTurn(_order % 4);
    }
}
