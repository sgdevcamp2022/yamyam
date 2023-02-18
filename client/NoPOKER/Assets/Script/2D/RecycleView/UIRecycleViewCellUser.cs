using UnityEngine;
using TMPro;
using System.Text;
namespace UI
{
    public class UICellUserData
    {
        public string Name;
        public bool Invite;
    }

    public class UIRecycleViewCellUser : UIRecycleViewCell<UICellUserData>
    {
        [SerializeField] private TMP_Text _txtContent;
        [SerializeField] private GameObject _inviteButton;
        UICellUserData _data;

        public override void UpdateContent(UICellUserData itemData)
        {
            _data = itemData;
            _txtContent.text = _data.Name;
            _inviteButton.SetActive(_data.Invite);
        }

        public void OnClickedButton()
        {
            Debug.Log("name : " + _data.Name);
            Team.Instance.Invite(_data.Name);
        }
    }
}
