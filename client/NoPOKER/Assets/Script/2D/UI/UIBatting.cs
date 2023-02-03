using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class UIBatting : MonoBehaviour
{
    [SerializeField] Button _battingButton;
    [SerializeField] Button _callButton;
    [SerializeField] Button _dieButton;

    [SerializeField] TMP_Text _roundBattingChip;
    [SerializeField] TMP_Text _battingChipNum;
    [SerializeField] TMP_Text _myBattingChip;

    [SerializeField] GameObject _dieView;
    private int _canBatting;

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
        _dieView.SetActive(false);

        _settingButton();
    }

    private void _settingButton()
    {
        _battingButton.onClick.AddListener(() => Batting.Instance.Raise(_canBatting));
        _battingButton.onClick.AddListener(() => ChangeBattingChip());

        _callButton.onClick.AddListener(() => Batting.Instance.Call());
        _callButton.onClick.AddListener(() => ChangeBattingChip());

        _dieButton.onClick.AddListener(() => Batting.Instance.Die());
        _dieButton.onClick.AddListener(() => ActiveDieView());
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

    public void ActiveDieView()
    {       
        _dieView.SetActive(true);
    }

}
