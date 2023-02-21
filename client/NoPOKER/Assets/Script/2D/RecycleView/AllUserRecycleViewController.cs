using System;
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
        public bool IsChangedUserList = false;       

        public void LoadAllChattingData()
        {
            //SetDatas(UserList.Instance..users);

            TableData = new List<UICellUserData>()
            {
              

            };

            InitializeTableView();
            _preContentHeight = CachedScrollRect.content.sizeDelta.y;
            _scroll.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
        }


        public void SetDatas(UserSocketData[] users)
        {/*
            for(int i=0;i<users.Length;i++)
            {
                TableData.Add(new UICellUserData {Id=users[i].id , Name = users[i].nickname, Invite = true});
            }
            UpdateData();*/
        }

        public void AddData(UserSocketData user)
        {
            Debug.Log("AddData userNickName = " +user.nickname);
            if (user.id != UserInfo.Instance.UserID)
            {
                TableData.Add(new UICellUserData { Id = user.id, Name = user.nickname, Invite = true });
            }
            UpdateData();
        }

        public void DeleteData(UserSocketData user)
        {
            Debug.Log("Now TableData Count = " + TableData.Count);
            for(int i=0;i< TableData.Count;i++)
            {
                if (TableData[i].Id == user.id)
                    TableData.RemoveAt(i);
                   // TableData.Remove(new UICellUserData { Id = user.id, Name = user.nickname, Invite = true });
            }
          

            UpdateAfterLeaveData();
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

        public void UpdateAfterLeaveData()
        {
            _currentContentPos = CachedScrollRect.content.anchoredPosition.y;
            _changedHeight = CachedScrollRect.content.sizeDelta.y - _preContentHeight;
            _preContentHeight = CachedScrollRect.content.sizeDelta.y;
            _currentContentPos -= _changedHeight;
            CachedScrollRect.content.anchoredPosition = new Vector2(0f, _currentContentPos);
            InitializeTableView();
            OnScrollPoschanged(new Vector2(0f, 0.01f));
        }

        protected override void Start()
        {
            base.Start();
            LoadAllChattingData();

        }
    }
}
