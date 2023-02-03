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

    private int _myBattingChip = 90;
    public int MyBattingChip { get => _myBattingChip; }

    private int _callBattingChip = 10;
    public int MinBattingChip { get => _callBattingChip + _unitBattingChip; }

    private int _unitBattingChip = 5;
    public int UnitBattingChip { get => _unitBattingChip; }

    private int _turnbattingChip;
    private bool _canPayChip;

    private void Awake()
    {
        _init();
    }

    private void _init()
    {
        if (s_instance == null)
            s_instance = this;
    }

    private bool _checkMyChip(int batting) 
    {
        if(_myBattingChip < batting)
        {
            //Alert 알림화면으로 낼 칩이 그만큼 없다고 하기.
            //음.. 게임에 대해서 다시 팀원들과 얘기해보기.
            return false;
        }
            return _myBattingChip < batting ? false : true;
    }
    public void Raise(int raiseChip)
    {
        _canPayChip = _checkMyChip(raiseChip);
        if (_canPayChip)
        {
            _payChip(raiseChip);
        }
    }
    public void Call()
    {
        _canPayChip = _checkMyChip(_callBattingChip);
        if (_canPayChip) 
        {
            _payChip(_callBattingChip);
        }      
    }
    public void Die()
    {
        // 서버통신 : 해당 플레이어가 다이했다는걸 알리기.
        // 턴을 넘기도록함.
    }
    private void _payChip(int batting)
    {
       
        _roundBattingChip += batting;
        _myBattingChip -= batting;
        _callBattingChip = batting;
        Debug.Log("현재 나의 칩 금액 : " + _myBattingChip);
        // 서버통신 : 칩을 지불함, 전체 베팅금액 증가 알림
        // 턴을 넘기도록함.
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


}
