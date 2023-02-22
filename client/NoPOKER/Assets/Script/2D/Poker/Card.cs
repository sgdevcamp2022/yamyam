using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField]private UICard _uiCard;
    private int[] _playerCards = new int[4];

    public void  DistributeCard()//나중에 서버로부터 플레이어 카드정보들을 받아오는 역할을 함.
    {
        for(int i=0;i<PokerGameManager.Instance.PeopleNum; i++)
        {
            _playerCards[i] = PokerGameManager.Instance.GetPlayerUiOrders[i].card;
           // _playerCards[i] = PokerGameSocket.Instance.GetGamePlayersList[i].card;
        }      
          _uiCard.SettingCard(_playerCards);
    }

    public void OpenMyCard()
    {
        _uiCard.ShowMyCard();
    }
}
