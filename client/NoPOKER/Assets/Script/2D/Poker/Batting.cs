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
    public int MinBattingChip { get => _callBattingChip + _unitBattingChip; }
    private int _unitBattingChip = 5;
    public int UnitBattingChip { get => _unitBattingChip; }
    private bool _canPayChip;

    

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
            _uiBatting.SetPlayerBattingResult(PokerGameManager.Instance.NowTurn, raiseChip.ToString());
            PersonSound.Instance.PlayRaiseSound();
            _payChip(raiseChip, false);
        }
    }

    public void Call()
    {       
        _canPayChip = _checkMyChip(_callBattingChip);
        if (_canPayChip)
        {
            _uiBatting.SetPlayerBattingResult(PokerGameManager.Instance.NowTurn, "��");
            PersonSound.Instance.PlayCallSound();
            _payChip(_callBattingChip, true);
        }      
    }

    public void Die()
    {
        // ������� : �ش� �÷��̾ �����ߴٴ°� �˸���.
        // ���� �ѱ⵵����.
        _uiBatting.SetPlayerBattingResult(PokerGameManager.Instance.NowTurn, "����");
        _uiBatting.ActiveDieView(PokerGameManager.Instance.NowTurn);
        PokerGameManager.Instance.UpDieNum();
        PersonSound.Instance.PlayDieSound();
        PokerGameManager.Instance.ChangePlayerState(PokerState.die);
        PokerGameManager.Instance.FinishTurn();
    }

    private void _payChip(int batting, bool call)
    {
        _roundBattingChip += batting;
        _myBattingChip -= batting;
        _callBattingChip = batting;
        _uiBatting.SettingCanBattingChip();
        _uiBatting.ChangeBattingChip();

        _uiBatting.ShowBattingChipMoveCenter(batting);
        // (���� �������)������� : �ش� �÷��̾ ���ߴٴ°� �˸���.
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
        _roundBattingChip += raise;
        _callBattingChip += raise;
        //UI�ʿ��� ��������� �ݿ��ؾ���..
    }

    public void SettingRoundBatting(int roundBatting)
    {
        _roundBattingChip = roundBatting;
    }

    public void Win() //��� ���̸� ���� ��� ������ �� ������� ���� ������.
    {
        for (int i = 0; i < PokerGameManager.Instance.PeopleNum; i++)
        {
            if (PokerGameManager.Instance.PlayerOrder[i].GetState() != PokerState.die)
            {
                PokerGameManager.Instance.PlayerOrder[i].AddChip(_roundBattingChip);
                //Reset ���� Ĩ
                ResetBattingChip();
                //����Ĩ �� ������� ���°� UI������ �����ֱ�
                _uiBatting.ShowBattingChipMovePlayer(i);
                Debug.Log("winnerã��~!");
            }
        }
        //�ٽ� ��Ŀ���� ����.
    }
    public void ResetBattingChip()
    {
        _roundBattingChip = 0;
        _callBattingChip = 10;
    }


}
