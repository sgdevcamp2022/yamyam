using UnityEngine;
using TMPro;

public class UIPokerPlayer : MonoBehaviour
{
    [SerializeField] TMP_Text[] _uiUserNames = new TMP_Text[4];
    [SerializeField] TMP_Text[] _uiUserChip = new TMP_Text[4];

    public void SetUserName(int pos, string name)
    {
        _uiUserNames[pos].text = name;
    }

    public void SetUsetChip(int pos, int chip)
    {
        _uiUserChip[pos].text = chip.ToString();
    }
}

