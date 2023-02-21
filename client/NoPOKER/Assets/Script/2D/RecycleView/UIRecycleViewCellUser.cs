using UnityEngine;
using TMPro;
using System.Text;
namespace UI
{
    public class UICellUserData
    {
        public int Id;
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
            if(UserInfo.Instance.IsLeader)
            Team.Instance.SendInviteRequest(new UserSocketData(_data.Id, _data.Name));
        }
    }
}
