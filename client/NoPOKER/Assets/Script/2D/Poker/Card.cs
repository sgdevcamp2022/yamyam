using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField]private UICard _uiCard;
    private int[] _playerCards = new int[4];



    public void  DistributeCard()//���߿� �����κ��� �÷��̾� ī���������� �޾ƿ��� ������ ��.
    {

        for(int i=0;i<PokerGameManager.Instance.PeopleNum; i++)
        {
            _playerCards[i] = Random.Range(0, 10);
        }
        switch(PokerGameManager.Instance.PeopleNum)
        {
            case 2:
                _uiCard.SettingCard2(_playerCards);
                break;
            case 4:
                _uiCard.SettingCard4(_playerCards);
                break;
        }    
    }

    public void OpenMyCard()
    {
        _uiCard.ShowMyCard();
    }
}
