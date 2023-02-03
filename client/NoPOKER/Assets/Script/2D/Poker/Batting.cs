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
            //Alert �˸�ȭ������ �� Ĩ�� �׸�ŭ ���ٰ� �ϱ�.
            //��.. ���ӿ� ���ؼ� �ٽ� ������� ����غ���.
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
        // ������� : �ش� �÷��̾ �����ߴٴ°� �˸���.
        // ���� �ѱ⵵����.
    }
    private void _payChip(int batting)
    {
       
        _roundBattingChip += batting;
        _myBattingChip -= batting;
        _callBattingChip = batting;
        Debug.Log("���� ���� Ĩ �ݾ� : " + _myBattingChip);
        // ������� : Ĩ�� ������, ��ü ���ñݾ� ���� �˸�
        // ���� �ѱ⵵����.
    }

    ////////////////////////////////////////������ ���� �����κ�..../////////////////////////////////////
    /// <summary>
    /// ���� ������ ���� ���� ���� �ݾ��� �ö��� ���.
    /// </summary>
    public void RaiseRoundBatting(int raise) 
    {
        //raise : �ö� �ݾ�
        _roundBattingChip += raise;
        _callBattingChip += raise;
        //UI�ʿ��� ��������� �ݿ��ؾ���..
    }


}
