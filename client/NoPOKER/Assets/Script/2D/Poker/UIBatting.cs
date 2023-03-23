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

    [SerializeField] GameObject[] _2DieView = new GameObject[2];
    [SerializeField] GameObject[] _4DieView = new GameObject[4];

    [SerializeField] GameObject[] _2PlayerBattingResultObject = new GameObject[2];
    [SerializeField] TMP_Text[] _2PlayerBattingResultText = new TMP_Text[2];
    [SerializeField] GameObject[] _4PlayerBattingResultObject = new GameObject[4];
    [SerializeField] TMP_Text[] _4PlayerBattingResultText = new TMP_Text[4];

    [SerializeField] GameObject[] _playersPosition = new GameObject[4];
    [SerializeField] RectTransform[] _battingChips = new RectTransform[3];
    [SerializeField] GameObject _inactiveRaiseButtonView;
    [SerializeField] TMP_Text _allInorCallText;
    [SerializeField] GameObject _inactiveCallButtonView;
    private int _canBatting;
    private Vector3 _targetPos;

    private void Start()
    {
        _init();
    }

    public void InitSetting()
    {
        _init();
    }
    private void _init()
    {
        _canBatting = Batting.Instance.MinBattingChip;
        _roundBattingChip.text = Batting.Instance.RoundBattingChip.ToString();
        _myBattingChip.text = Batting.Instance.MyBattingChip.ToString();
        _battingChipNum.text = _canBatting.ToString();
        for (int i = 0; i < 4; i++)
            _4DieView[i].SetActive(false);
        for (int i = 0; i < 2; i++)
            _2DieView[i].SetActive(false);

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
        _canBatting += 1;

        if (_canBatting <= Batting.Instance.MyBattingChip)
        {
            _battingChipNum.text = _canBatting.ToString();
        }
        else
        {
            _canBatting -= 1;
        }
    }

    public void BattingDown()
    {
        _canBatting -= 1;

        if (_canBatting < Batting.Instance.MinBattingChip)
        {
            _canBatting += 1;
        }
        else
        {
            _battingChipNum.text = _canBatting.ToString();
        }
    }

    public void ChangeToAllinButton()
    {
        _allInorCallText.text = "올인";
        _inactiveRaiseButtonView.SetActive(true);
    }

    public void ChangeToCallButton()
    {
        _allInorCallText.text = "콜";
        _inactiveRaiseButtonView.SetActive(false);
    }

    public void DonAccessCallButton()
    {

        _inactiveCallButtonView.SetActive(true);
    }

    public void AccessCallButton()
    {

        _inactiveCallButtonView.SetActive(false);
    }

    public void ChangeBattingChip()
    {
        SettingCanBattingChip();
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
        if (PokerGameManager.Instance.PeopleNum == 2)
        {
            _2DieView[who].SetActive(true);
        }
        else
        {
            _4DieView[who].SetActive(true);
        }

    }

    public void ResetPlayerBattingResult()
    {
        for(int i=0;i<2;i++)
        {
            _2PlayerBattingResultObject[i].SetActive(false);
        }
        
        for(int i=0;i<4;i++)
        {
            _4PlayerBattingResultObject[i].SetActive(false);
        }
    }


    public void SetPlayerBattingResult(int who, string text)
    {
        if (PokerGameManager.Instance.PeopleNum == 2)
        {
            _2PlayerBattingResultObject[who].SetActive(true);
            _2PlayerBattingResultText[who].text = text;
        }
        else
        {
            _4PlayerBattingResultObject[who].SetActive(true);
            _4PlayerBattingResultText[who].text = text;
        }

    }

    public void ShowBattingChipMoveCenter(int battingMoney)
    {
        _battingChips[0].position = _playersPosition[PokerGameManager.Instance.UiPos].transform.position;
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
        _targetPos = _playersPosition[who].transform.position;
        while ((_battingChips[0].position - _targetPos).sqrMagnitude > 0.5)
        {
            _battingChips[0].position = Vector3.Lerp(_battingChips[0].position, _targetPos, 0.5f);
            yield return new WaitForSeconds(0.1f);
        }
    }

}
