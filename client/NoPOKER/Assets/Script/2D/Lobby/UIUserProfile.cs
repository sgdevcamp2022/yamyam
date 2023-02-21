using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIUserProfile : MonoBehaviour
{
    [SerializeField] TMP_Text _userName;

    private void Start()
    {
        _userName.text = UserInfo.Instance.NickName;
    }

}
