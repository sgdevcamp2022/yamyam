using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField]private UICard _uiCard;
    private int _myCardNum;
    
    private void Awake()
    {
        DistributeCard();
    }
    private void DistributeCard()
    {
        _myCardNum = Random.Range(0, 10);
        _uiCard.SettingMyCard(_myCardNum);
    }

}
