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
        _playerNames.Add("냠냠냠냠냠냠");
        _playerNames.Add("얌얌");
        _playerNames.Add("쩝쩝");
        _playerNames.Add("뇸뇸");

        for (int i = 0; i < PeopleNum; i++)
        {
            PlayerOrder.Add(new PokerPlayer(_playerNames[i],90));
        }         
        Batting.Instance.SettingRoundBatting(_peopleNum * 10);

        _card.DistributeCard();
    }

    /*[0] 은 무조건 자기자신. [1] 은 자기 바로 다음차례. [2]는 그다다음 [3]은 그다다다음
    public void SettingPlayerOrder()
    {

    }*/

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
            if ((_order + 1) % _peopleNum == 0)
            {
                _gameProcessNum++;
                Debug.Log("횟수 늘어남. 현재 : " + _gameProcessNum);
            }
            _order++;


            Debug.Log("In else" );
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



    public void Change3DGame()
    {
        //3D게임전환.
        Debug.Log("3D게임으로 전환됩니다.");
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
