using UnityEngine;
using TMPro;

namespace UI
{
    public class UICellSampleData
    {
        public string Name;
        public string Chat;
    }

    public class UIRecycleViewCellSample : UIRecycleViewCell<UICellSampleData>
    {
        [SerializeField] private TMP_Text txtName;
        [SerializeField] private TMP_Text txtChat;

        public override void UpdateContent(UICellSampleData itemData)
        {
            txtName.text = itemData.Name;
            txtChat.text = itemData.Chat;
        }

        public void OnClickedButton()
        {
              Debug.Log(txtChat.text + ":" + System.Text.Encoding.Default.GetByteCount(txtName.text));
           // Debug.Log(txtChat.text.Length);
        }

    }
}
