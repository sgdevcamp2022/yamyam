using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AllUserRecycleViewController : UIRecycleViewController<UICellUserData>
    {
        [SerializeField] RectTransform _scroll;
        [SerializeField] ScrollRect _scrollRect;

        [SerializeField]
        float _preContentHeight;
        float _currentContentPos;
        private float _changedHeight;
        public void LoadAllChattingData()
        {
            TableData = new List<UICellUserData>()
            {
                new UICellUserData { Name="������" ,Invite = true},
                new UICellUserData { Name="���߾�" , Invite = true},
                new UICellUserData { Name="�����ϰ���Ʈ" ,Invite = true },
                new UICellUserData { Name="�̻��" , Invite = true},
                new UICellUserData { Name="���" ,Invite = true },
               new UICellUserData { Name="NoPOKER" , Invite = true},

            };

            InitializeTableView();
            _preContentHeight = CachedScrollRect.content.sizeDelta.y;
            _scroll.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
        }

        public void AddData(UICellUserData data)
        {
            TableData.Add(data);
        }

        public void UpdateData() //���ο� ������ �κ� �߰��Ǿ��� ��
        {
            _currentContentPos = CachedScrollRect.content.anchoredPosition.y;
            _changedHeight = CachedScrollRect.content.sizeDelta.y - _preContentHeight;
            _preContentHeight = CachedScrollRect.content.sizeDelta.y;
            _currentContentPos += _changedHeight;
            CachedScrollRect.content.anchoredPosition = new Vector2(0f, _currentContentPos);
            InitializeTableView();
            OnScrollPoschanged(new Vector2(0f, -0.01f));
        }

        protected override void Start()
        {
            base.Start();
            LoadAllChattingData();
        }
    }
}
