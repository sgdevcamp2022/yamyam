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

    public void SettingCard(int[] cards) //플레이어 순서대로 있는 카드.
    {
        _cards = cards; //_cards는 게임 플레이어 순서대로 들어가있는 카드 숫자이다.
        int peopleNum = PokerGameSocket.Instance.GetPokerGamePeopleNum;
        _playerCardUI[0].sprite = _backImage; //0은 나 자신.


       // int pos = PokerGameManager.Instance.PlayerOrder.FindIndex(x=>x.)
     //   _playerCardUI[1].sprite =
     if(PokerGameManager.Instance.PeopleNum == 2)
        {
            Debug.Log("_cards[1] : " +_cards[1]);
            _playerCardUI[2].sprite = _cardImage[_cards[1]-1];
        }
     else //2명 이상일 경우
        {
            for (int i = 1; i < peopleNum; i++) //나 이후에.
            {
                _playerCardUI[i].sprite = _cardImage[_cards[i]];
            }
        }
      
        _playerCardUI[0].gameObject.SetActive(true);
        _playerCardUI[2].gameObject.SetActive(true);


        if (peopleNum == 2)
        {
            _playerCardUI[1].gameObject.SetActive(false);
            _playerCardUI[3].gameObject.SetActive(false);
        }
        else if (peopleNum == 3)
        {
            _playerCardUI[1].gameObject.SetActive(true);
            _playerCardUI[3].gameObject.SetActive(false);
        }
        else if (peopleNum ==4)
        {
            _playerCardUI[3].gameObject.SetActive(true);
        }
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
