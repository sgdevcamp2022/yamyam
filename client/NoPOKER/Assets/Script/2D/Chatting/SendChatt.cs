using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using TMPro;
public class SendChatt : MonoBehaviour
{
    [SerializeField] TMP_InputField _chattingField;
    [SerializeField] TMP_Text _sendChattContent;
    UIChattData _sendChattData = new UIChattData();
    LobbyMessageSocketData _sendMessageData = new LobbyMessageSocketData();
    private void Start()
    {
        _chattingField.onSubmit.AddListener(delegate { SendChattContent(); });
        _sendChattData.Name = UserInfo.Instance.NickName;
    }
    public void SendChattContent()
    {
        _sendChattData.ClearChatData();
        _sendChattData.Chat = _sendChattContent.text;
        Chatting.Instance.SendChatting(_sendChattData);

        _sendMessageData.LobbyMessageSetting(_sendChattContent.text);

        LobbyConnect.Instance.SendAllChattMessage(_sendMessageData);
        _chattingField.text = "";
    }
}
