using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UICard : MonoBehaviour
{
    [SerializeField] private List<Sprite> _cardImage = new List<Sprite>();
    [SerializeField] private Image _myCardUI;

    public void SettingMyCard(int num)
    {
        _myCardUI.sprite = _cardImage[num];
    }

}
