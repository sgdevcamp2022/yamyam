using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICard : MonoBehaviour
{
    [SerializeField] private List<Sprite> _cardImage = new List<Sprite>();
    [SerializeField] private Image[] _playerCardUI = new Image[4];
    [SerializeField] private Sprite _backImage;
    private int[] _cards;
    public void SettingCard2(int[] cards)
    {
        _cards = cards;
        //_playerCardUI[0].sprite = _cardImage[cards[0]];
        _playerCardUI[0].sprite = _backImage;
        _playerCardUI[2].sprite = _cardImage[_cards[2]];
    }

    public void SettingCard4(int[] cards)
    {
        _cards = cards;
        _playerCardUI[0].sprite = _backImage;
        for (int i=1;i<4;i++)
        {
            _playerCardUI[i].sprite = _cardImage[_cards[i]];
        }
    }

    public void ShowMyCard()
    {
        _playerCardUI[0].sprite = _cardImage[_cards[0]];
    }
}
