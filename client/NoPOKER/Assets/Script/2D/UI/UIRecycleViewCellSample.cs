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
        [SerializeField] private TMP_Text _txtName;
        [SerializeField] private TMP_Text _txtChat;

        public override void UpdateContent(UICellSampleData itemData)
        {
            _txtName.text = itemData.Name;
            _txtChat.text = itemData.Chat;
        }

        public void OnClickedButton()
        {
              Debug.Log(_txtName.text);
        }
    }
}
