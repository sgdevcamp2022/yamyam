using UnityEngine;
using TMPro;
using System.Text;
namespace UI
{
    public class UICellData
    {
        public string Name;
        public string Chat;
    }

    public class UIRecycleViewCellChatting : UIRecycleViewCell<UICellData>
    {
        [SerializeField] private TMP_Text _txtContent;

       StringBuilder _stringBuilder = new StringBuilder();
        UICellData _data;
        string _frontColor = "<color=#CAB75B>";
        string _backColor = "</color>";
        public override void UpdateContent(UICellData itemData)
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
