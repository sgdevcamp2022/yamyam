using UnityEngine;
using UI;
using TMPro;
public class SendChatt : MonoBehaviour
{
    [SerializeField] TMP_InputField _chattingField;
    [SerializeField] TMP_Text _sendChattContent;
    UIChattData _sendChattData = new UIChattData();

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
        _chattingField.text = "";
    }
}
