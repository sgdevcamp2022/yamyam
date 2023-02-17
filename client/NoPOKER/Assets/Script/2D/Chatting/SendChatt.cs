using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using TMPro;
public class SendChatt : MonoBehaviour
{
    [SerializeField] TMP_InputField _chattingField;
    [SerializeField] TMP_Text _sendChattContent;

    private void Start()
    {
        _chattingField.onSubmit.AddListener(delegate { SendChattContent(); });
    }
    public void SendChattContent()
    {
        Chatting.Instance.SendChatting(new UICellData {Name= UserInfo.Instance.NickName,Chat= _sendChattContent.text});
        _chattingField.text = "";
    }
}
