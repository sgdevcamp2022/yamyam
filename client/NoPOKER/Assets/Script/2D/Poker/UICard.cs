using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UICard : MonoBehaviour
{
    [SerializeField] private List<Sprite> _cardImage = new List<Sprite>();

    [SerializeField] private Image[] _playerCardUI = new Image[4];

    public void SettingCard2(int[] cards)
    {
        _playerCardUI[0].sprite = _cardImage[cards[0]];
        _playerCardUI[2].sprite = _cardImage[cards[2]];
    }
    public void SettingCard4(int[] cards)
    {
        for(int i=0;i<4;i++)
        {
            _playerCardUI[i].sprite = _cardImage[cards[i]];
        }
    }


}
