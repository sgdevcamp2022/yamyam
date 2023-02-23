using UnityEngine;
using TMPro;
using System.Text;
namespace UI
{
    public class UIChattData
    {
        public string Name;
        public string Chat;

        public void ClearChatData()
        {
            Chat = "";
        }
    }

    public class UIRecycleViewCellChatting : UIRecycleViewCell<UIChattData>
    {
        [SerializeField] private TMP_Text _txtContent;

       StringBuilder _stringBuilder = new StringBuilder();
        UIChattData _data;
        string _frontColor = "<color=#CAB75B>";
        string _backColor = "</color>";
        public override void UpdateContent(UIChattData itemData)
        {
            _data = itemData;
            _stringBuilder.Clear();
            _stringBuilder.Append(_frontColor);
            _stringBuilder.Append(_data.Name);
            _stringBuilder.Append(" : ");
            _stringBuilder.Append(_backColor);
            _stringBuilder.Append(_data.Chat);

            _txtContent.text = _stringBuilder.ToString();

        }

        public void OnClickedButton()
        {
              Debug.Log(_data.Name);
        }
    }
}
