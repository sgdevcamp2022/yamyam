using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class UICellSampleData
    {
        public string name;
        public string chat;
    }

    public class UIRecycleViewCellSample : UIRecycleViewCell<UICellSampleData>
    {
        [SerializeField] private TMP_Text txtName;
        [SerializeField] private TMP_Text txtChat;

        public override void UpdateContent(UICellSampleData itemData)
        {
            txtName.text = itemData.name;
            txtChat.text = itemData.chat;
        }

        public void OnClickedButton()
        {
              Debug.Log(txtChat.text + ":" + System.Text.Encoding.Default.GetByteCount(txtName.text));
           // Debug.Log(txtChat.text.Length);
        }

    }
}
