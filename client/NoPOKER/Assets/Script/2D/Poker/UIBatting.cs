using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIBatting : MonoBehaviour
{
    [SerializeField] Button _battingButton;
    [SerializeField] Button _callButton;
    [SerializeField] Button _dieButton;

    [SerializeField] TMP_Text _roundBattingChip;
    [SerializeField] TMP_Text _battingChipNum;
    [SerializeField] TMP_Text _myBattingChip;

    [SerializeField] GameObject[] _dieView = new GameObject[4];
    [SerializeField] GameObject[] _playerBattingResultObject = new GameObject[4];
    [SerializeField] TMP_Text[] _playerBattingResultText = new TMP_Text[4];

    [SerializeField] GameObject[] _playersPosition = new GameObject[4];
    [SerializeField] RectTransform[] _battingChips = new RectTransform[3];

    private int _canBatting;
    private Vector3 _targetPos;

    private void Start()
    {
        _init();
    }

    private void _init()
    {
        _canBatting = Batting.Instance.MinBattingChip;
        _roundBattingChip.text = Batting.Instance.RoundBattingChip.ToString();
        _myBattingChip.text = Batting.Instance.MyBattingChip.ToString();
        _battingChipNum.text = _canBatting.ToString();
        for(int i=0;i<4;i++)
        _dieView[i].SetActive(false);

        _settingButton();
    }

    private void _settingButton()
    {
        _battingButton.onClick.AddListener(() => Batting.Instance.Raise(_canBatting));
        _callButton.onClick.AddListener(() => Batting.Instance.Call());
        _dieButton.onClick.AddListener(() => Batting.Instance.Die());
    }

    public void SettingCanBattingChip()
    {
        _canBatting = Batting.Instance.MinBattingChip;
    }

    public void BattingUp()
    {
        _canBatting += Batting.Instance.UnitBattingChip;

        if ( _canBatting <= Batting.Instance.MyBattingChip)
        {
            _battingChipNum.text = _canBatting.ToString();
        }
        else
        {
            _canBatting -= Batting.Instance.UnitBattingChip;
        }       
    }

    public void BattingDown()
    {
        _canBatting -= Batting.Instance.UnitBattingChip;

        if ( _canBatting < Batting.Instance.MinBattingChip)
        {
            _canBatting += Batting.Instance.UnitBattingChip;
        }
        else
        {
            _battingChipNum.text = _canBatting.ToString();
        }
    }

    public void ChangeBattingChip()
    {
        ChangeRoundBattingChip();
        ChangeMyBattingChip();
        ChangeRaiseBattingChip();

    }
    public void ChangeRaiseBattingChip()
    {
        _battingChipNum.text = Batting.Instance.MinBattingChip.ToString();
    }
    public void ChangeRoundBattingChip()
    {
        _roundBattingChip.text = Batting.Instance.RoundBattingChip.ToString();
    }

    public void ChangeMyBattingChip()
    {
        _myBattingChip.text = Batting.Instance.MyBattingChip.ToString();
    }

    public void ActiveDieView(int who)
    {       
        _dieView[who].SetActive(true);
        PokerGameManager.Instance.PlayerOrder[who].SetState(PokerState.die);
    }

    public void SetPlayerBattingResult(int who, string text)
    {
        _playerBattingResultObject[who].SetActive(true);
        _playerBattingResultText[who].text = text;
    }
    public void ShowBattingChipMoveCenter(int battingMoney)
    {
        _battingChips[0].position = _playersPosition[PokerGameManager.Instance.NowTurn].transform.position;
        _battingChips[0].gameObject.SetActive(true);
        StartCoroutine(BattingChipMoveCenter(battingMoney));
    }
    
    IEnumerator BattingChipMoveCenter(int battingMoney)
    {
        Sound.Instance.PlayBattinSound();
        _targetPos = transform.position;


        while ((_battingChips[0].position - _targetPos).sqrMagnitude > 0.5)
        {
            _battingChips[0].position = Vector3.Lerp(_battingChips[0].position, _targetPos, 0.5f);
            yield return new WaitForSeconds(0.1f);
        }       
        _battingChips[0].gameObject.SetActive(false);
    }

    public void ShowBattingChipMovePlayer(int winner)
    {

        _battingChips[0].gameObject.SetActive(true);
        _battingChips[0].position = transform.position;
        StartCoroutine(BattingChipMovePlayer(winner));
    }

    IEnumerator BattingChipMovePlayer(int who)
    {
        Debug.Log("who is : " + who);
        _targetPos = _playersPosition[who].transform.position;
        while ((_battingChips[0].position - _targetPos).sqrMagnitude > 0.5)
        {
            _battingChips[0].position = Vector3.Lerp(_battingChips[0].position, _targetPos, 0.5f);
            yield return new WaitForSeconds(0.1f);
        }
       // _battingChips[0].gameObject.SetActive(false);
        
    }

}
