using UnityEngine;

public class Batting : MonoBehaviour
{
    [SerializeField] UIBatting _uiBatting;
    public UIBatting GetUIBatting { get => _uiBatting; }
    static private Batting s_instance = null;
    static public Batting Instance { get => s_instance; }

    private long _roundBattingChip = 20;
    public long RoundBattingChip { get => _roundBattingChip; }
    private int _myBattingChip = 90;
    public int MyBattingChip { get => _myBattingChip; }
    private int _callBattingChip = 0;
    public int CallBattingChip { get => _callBattingChip; }
    public int MinBattingChip { get => _callBattingChip +1; }
    private bool _canPayChip;
    public GameObject InActiveButtonView;


    public void SetMyBattingChip(int chip)
    {
        _myBattingChip = chip;
    }

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        if (s_instance == null)
            s_instance = this;
    }
    public void SetCallBattingChip(int callChip)
    {
        _callBattingChip = callChip;
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

                _payChip(raiseChip, false);
        }
    }
    public void OtherRaise(int raiseChip)
    {  
            _uiBatting.SetPlayerBattingResult(PokerGameManager.Instance.ResultUserUiPos, raiseChip.ToString());
            PersonSound.Instance.PlayRaiseSound();
    }
    public void OtherAllin()
    {
        _uiBatting.SetPlayerBattingResult(PokerGameManager.Instance.ResultUserUiPos, "올인");
        PersonSound.Instance.PlayCallSound();
    }
    public void Call()
    {       
        _canPayChip = _checkMyChip(_callBattingChip);
        if (_canPayChip)
        {
            _uiBatting.SetPlayerBattingResult(0, "콜");
            PersonSound.Instance.PlayCallSound();

            _payChip(_callBattingChip, true);
        }      
    }
    public void OtherCall()
    {
       
            _uiBatting.SetPlayerBattingResult(PokerGameManager.Instance.ResultUserUiPos, "콜");
            PersonSound.Instance.PlayCallSound();
           // _payChip(_callBattingChip, true);
    }
    public void Die()
    {
        // 서버통신 : 해당 플레이어가 다이했다는걸 알리기.
        // 턴을 넘기도록함.
        _uiBatting.SetPlayerBattingResult(0, "다이");
        _uiBatting.ActiveDieView(0);
        PersonSound.Instance.PlayDieSound();
 
            PokerGameManager.Instance.FinishTurn();

            PokerGameSocket.Instance.SendDieRequest();
            InActiveButtonView.SetActive(true);

    }

    public void OtherDie(int userId)
    {
        // 서버통신 : 해당 플레이어가 다이했다는걸 알리기.
        // 턴을 넘기도록함.
        _uiBatting.SetPlayerBattingResult(PokerGameManager.Instance.ResultUserUiPos, "다이");
        _uiBatting.ActiveDieView(PokerGameManager.Instance.UiPos);
        PersonSound.Instance.PlayDieSound();

    }


    

    private void _payChip(int batting, bool call)
    {
        _roundBattingChip += batting;
        
        _callBattingChip = batting;

        // (콜을 했을경우)서버통신 : 해당 플레이어가 콜했다는걸 알리기.
        // 서버통신 : 칩을 지불함, 전체 베팅금액 증가 알림
        // 턴을 넘기도록함.

          Debug.Log("Who is Now UserID : " + PokerGameManager.Instance.NowTurnUserId);

          
            _myBattingChip -= batting;
            PokerGameManager.Instance.FinishTurn();
        
            _uiBatting.SettingCanBattingChip();
            _uiBatting.ChangeBattingChip();

            _uiBatting.ShowBattingChipMoveCenter(batting);
           InActiveButtonView.SetActive(true);

            if(call)
            {
                if(_myBattingChip < CallBattingChip)
                {
                    PokerGameSocket.Instance.SendAllInRequest(_myBattingChip);
                }
                else
                {
                    PokerGameSocket.Instance.SendCallRequest(batting);
                }
             
            }
            else
            {
                PokerGameSocket.Instance.SendRaiseRequest(batting);
            }
               

    
    }



    public void SetRoundBatting(long total, int raise)
    {
        _roundBattingChip = total;
        _callBattingChip = raise;

        _uiBatting.ChangeBattingChip();
    }

    public void SettingRoundBatting(int roundBatting)
    {
        _roundBattingChip = roundBatting;
    }

    public void Win(int who) //모두 다이를 했을 경우 마지막 한 사람에게 돈을 몰아줌.
    {
      
                //Reset 베팅 칩
                ResetBattingChip();
                //베팅칩 그 사람한테 가는거 UI적으로 보여주기
                _uiBatting.ShowBattingChipMovePlayer(who);

                Debug.Log("winner찾음~!");
            
        
        //다시 포커게임 시작.
    }
    public void ResetBattingChip()
    {
        _roundBattingChip = 0;
        _callBattingChip = 0;
    }

 

    public void CallorDieState(bool state)
    {
        if(state)
        {
            _uiBatting.DonAccessBattingButton();
        }        
        else
        {
            _uiBatting.AccessBattingButton();
        }
    }
    public void CanCallState(bool state)
    {
        if (state)
        {
            _uiBatting.AccessCallButton();
        }
        else
        {
            _uiBatting.DonAccessCallButton();
        }
    }
}
