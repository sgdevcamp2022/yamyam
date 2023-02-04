using UnityEngine;

public enum BattingMessage
{
    Batting,
    Call,
    Die
}

public class Batting : MonoBehaviour
{
    static private Batting s_instance = null;
    static public Batting Instance { get => s_instance; }

    private int _roundBattingChip = 20;
    public int RoundBattingChip { get => _roundBattingChip; }

    private int _myBattingChip = 150;
    public int MyBattingChip { get => _myBattingChip; }

    private int _callBattingChip = 10;
    public int MinBattingChip { get => _callBattingChip + _unitBattingChip; }

    private int _unitBattingChip = 5;
    public int UnitBattingChip { get => _unitBattingChip; }

    private bool _canPayChip;

    [SerializeField] UIBatting _uiBatting;

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
            _uiBatting.SetPlayerBattingResult(0, raiseChip.ToString());
            _payChip(raiseChip, false);
        }
    }
    public void Call()
    {
        

        _canPayChip = _checkMyChip(_callBattingChip);
        if (_canPayChip)
        {
            _uiBatting.SetPlayerBattingResult(0, "콜");
            _payChip(_callBattingChip, true);
        }      
    }
    public void Die()
    {
        // 서버통신 : 해당 플레이어가 다이했다는걸 알리기.
        // 턴을 넘기도록함.
        _uiBatting.SetPlayerBattingResult(0,"다이");
        PokerGameManager.Instance.FinishTurn();
    }
    private void _payChip(int batting, bool call)
    {
       
        _roundBattingChip += batting;
        _myBattingChip -= batting;
        _callBattingChip = batting;
        Debug.Log("현재 나의 칩 금액 : " + _myBattingChip);
        // (콜을 했을경우)서버통신 : 해당 플레이어가 콜했다는걸 알리기.
        //if(call) 
        // 서버통신 : 칩을 지불함, 전체 베팅금액 증가 알림
        // 턴을 넘기도록함.
        PokerGameManager.Instance.FinishTurn();
    }

    ////////////////////////////////////////서버를 통해 받을부분..../////////////////////////////////////
    /// <summary>
    /// 서버 수신을 통해 라운드 베팅 금액이 올라갔을 경우.
    /// </summary>
    public void RaiseRoundBatting(int raise) 
    {
        //raise : 올라간 금액
        _roundBattingChip += raise;
        _callBattingChip += raise;
        //UI쪽에도 변경사항을 반영해야함..
    }

    public void SettingRoundBatting(int roundBatting)
    {
        _roundBattingChip = roundBatting;
    }


}
