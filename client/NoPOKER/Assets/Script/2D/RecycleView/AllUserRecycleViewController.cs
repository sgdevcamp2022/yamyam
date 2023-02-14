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
                new UICellUserData { Name="뇸뇸쓰" ,Invite = true},
                new UICellUserData { Name="겜잘알" , Invite = true},
                new UICellUserData { Name="스마일게이트" ,Invite = true },
                new UICellUserData { Name="이사님" , Invite = true},
                new UICellUserData { Name="용용" ,Invite = true },
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

        public void UpdateData() //새로운 유저가 로비에 추가되었을 때
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
