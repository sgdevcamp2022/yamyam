using UnityEngine;

public class Batting : MonoBehaviour
{
    [SerializeField] UIBatting _uiBatting;
    public UIBatting GetUIBatting { get => _uiBatting; }
    static private Batting s_instance = null;
    static public Batting Instance { get => s_instance; }

    private int _roundBattingChip = 20;
    public int RoundBattingChip { get => _roundBattingChip; }
    private int _myBattingChip = 150;
    public int MyBattingChip { get => _myBattingChip; }
    private int _callBattingChip = 10;
    public int CallBattingChip { get => _callBattingChip; }
    public int MinBattingChip { get => _callBattingChip + _unitBattingChip; }
    private int _unitBattingChip = 5;
    public int UnitBattingChip { get => _unitBattingChip; }
    private bool _canPayChip;
    public GameObject InActiveButtonView;


    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        if (s_instance == null)
            s_instance = this;
    }

    private bool _checkMyChip(int batting) 
    {
        if(_myBattingChip < batting)
        {
            return false;
        }
            return _myBattingChip < batting ? false : true;
    }

    public void Raise(int raiseChip)
    {
        _canPayChip = _checkMyChip(raiseChip);

        if (_canPayChip)
        {
            _uiBatting.SetPlayerBattingResult(PokerGameManager.Instance.UiPos, raiseChip.ToString());
            PersonSound.Instance.PlayRaiseSound();
            PokerGameManager.Instance.ResetCallNum();

                _payChip(raiseChip, false);
        }
    }
    public void OtherRaise(int raiseChip)
    {
   
            _uiBatting.SetPlayerBattingResult(PokerGameManager.Instance.UiPos, raiseChip.ToString());
            PersonSound.Instance.PlayRaiseSound();
            PokerGameManager.Instance.ResetCallNum();

            _payChip(raiseChip, false);

    }
    public void Call()
    {       
        _canPayChip = _checkMyChip(_callBattingChip);
        if (_canPayChip)
        {
            _uiBatting.SetPlayerBattingResult(PokerGameManager.Instance.NowTurnUserId, "콜");
            PersonSound.Instance.PlayCallSound();

            _payChip(_callBattingChip, true);
        }      
    }
    public void OtherCall()
    {
       
            _uiBatting.SetPlayerBattingResult(PokerGameManager.Instance.NowTurnUserId, "콜");
            PersonSound.Instance.PlayCallSound();
         //   PokerGameManager.Instance.UpCallNum();
            _payChip(_callBattingChip, true);
    }
    public void Die()
    {
        // 서버통신 : 해당 플레이어가 다이했다는걸 알리기.
        // 턴을 넘기도록함.
        _uiBatting.SetPlayerBattingResult(PokerGameManager.Instance.UiPos, "다이");
        _uiBatting.ActiveDieView(PokerGameManager.Instance.UiPos);
        PokerGameManager.Instance.UpDieNum();
        PersonSound.Instance.PlayDieSound();
        PokerGameManager.Instance.ChangePlayerState(BattingState.die);
    
    
        if(PokerGameManager.Instance.NowTurnUserId == UserInfo.Instance.UserID)
        {
            PokerGameManager.Instance.FinishTurn();

            PokerGameSocket.Instance.SendDieRequest();
            InActiveButtonView.SetActive(true);
        }
    }



    private void _payChip(int batting, bool call)
    {
        _roundBattingChip += batting;
        
        _callBattingChip = batting;

        // (콜을 했을경우)서버통신 : 해당 플레이어가 콜했다는걸 알리기.
        // 서버통신 : 칩을 지불함, 전체 베팅금액 증가 알림
        // 턴을 넘기도록함.


        if (PokerGameManager.Instance.NowTurnUserId == UserInfo.Instance.UserID)
        {
            _myBattingChip -= batting;
            PokerGameManager.Instance.FinishTurn();
            PokerGameSocket.Instance.SendBettingRequest(batting);

            _uiBatting.SettingCanBattingChip();
            _uiBatting.ChangeBattingChip();

            _uiBatting.ShowBattingChipMoveCenter(batting);
           InActiveButtonView.SetActive(true);
        }
        else
        {
            _uiBatting.SettingCanBattingChip();
            _uiBatting.ChangeBattingChip();

            _uiBatting.ShowBattingChipMoveCenter(batting);
            PokerGameManager.Instance.FinishTurn();
        }
    }



    ////////////////////////////////////////서버를 통해 받을부분..../////////////////////////////////////
    /// <summary>
    /// 서버 수신을 통해 라운드 베팅 금액이 올라갔을 경우.
    /// </summary>
    public void RaiseRoundBatting(int raise) 
    {
        _roundBattingChip += raise;
        _callBattingChip += raise;
        //UI쪽에도 변경사항을 반영해야함..
    }

    public void SetRoundBatting(int total, int raise)
    {
        _roundBattingChip = total;
        _callBattingChip = raise;

        _uiBatting.ChangeBattingChip();
    }

    public void SettingRoundBatting(int roundBatting)
    {
        _roundBattingChip = roundBatting;
    }

    public void Win() //모두 다이를 했을 경우 마지막 한 사람에게 돈을 몰아줌.
    {
        for (int i = 0; i < PokerGameManager.Instance.PeopleNum; i++)
        {
            if (PokerGameManager.Instance.PlayerOrder[i].GetState() != BattingState.die)
            {
                PokerGameManager.Instance.PlayerOrder[i].AddChip(_roundBattingChip);
                //Reset 베팅 칩
                ResetBattingChip();
                //베팅칩 그 사람한테 가는거 UI적으로 보여주기
                _uiBatting.ShowBattingChipMovePlayer(i);
                Debug.Log("winner찾음~!");
            }
        }
        //다시 포커게임 시작.
    }
    public void ResetBattingChip()
    {
        _roundBattingChip = 0;
        _callBattingChip = 10;
    }


}
