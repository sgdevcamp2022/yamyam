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
            _uiBatting.SetPlayerBattingResult(0, "��");
            _payChip(_callBattingChip, true);
        }      
    }
    public void Die()
    {
        // ������� : �ش� �÷��̾ �����ߴٴ°� �˸���.
        // ���� �ѱ⵵����.
        _uiBatting.SetPlayerBattingResult(0,"����");
        PokerGameManager.Instance.FinishTurn();
    }
    private void _payChip(int batting, bool call)
    {
       
        _roundBattingChip += batting;
        _myBattingChip -= batting;
        _callBattingChip = batting;
        Debug.Log("���� ���� Ĩ �ݾ� : " + _myBattingChip);
        // (���� �������)������� : �ش� �÷��̾ ���ߴٴ°� �˸���.
        //if(call) 
        // ������� : Ĩ�� ������, ��ü ���ñݾ� ���� �˸�
        // ���� �ѱ⵵����.
        PokerGameManager.Instance.FinishTurn();
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

    public void SettingRoundBatting(int roundBatting)
    {
        _roundBattingChip = roundBatting;
    }


}
