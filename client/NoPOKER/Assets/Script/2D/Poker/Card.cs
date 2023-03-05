using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] private UICard _uiCard;
    private int[] _playerCards = new int[4];

    public void DistributeCard()
    {
        for (int i = 0; i < PokerGameManager.Instance.PeopleNum; i++)
        {
            _playerCards[i] = PokerGameManager.Instance.GetPlayerUiOrders[i].card;
        }
        _uiCard.SettingCard(_playerCards);
    }

    public void OpenMyCard()
    {
        _uiCard.ShowMyCard();
    }
}
