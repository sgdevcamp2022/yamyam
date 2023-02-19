using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AllChattRecycleViewController : UIRecycleViewController<UIChattData>
    {
        [SerializeField] RectTransform _scroll;
        [SerializeField] ScrollRect _scrollRect;

        [SerializeField]
        float _preContentHeight;
        float _currentContentPos;
        private float _changedHeight;
        public void LoadAllChattingData()
        {
            TableData = new List<UIChattData>()
            {/*
                new UIChattData { Name="" , Chat = "한글한글한글한글한글한글한글한글한글한글한글한글한글한글한글한글한글"},
                new UIChattData { Name="겜잘알" , Chat = "CONTENTCONTENTCONTENTCONTENTCONTENTCONTENTC"},
                new UIChattData { Name="스마일게이트" , Chat = "윈터데브캠프에 오신걸 환영합니다!"},
                new UIChattData { Name="이사님" , Chat = "PMP 목표에 맞아요? 그게??"},
                new UIChattData { Name="용용" , Chat = "용용체를 쓰면 화가풀리는 마법이 생겨요"},
               new UIChattData { Name="NoPOKER" , Chat = "NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!"}*/
            };

            InitializeTableView();
            _preContentHeight = CachedScrollRect.content.sizeDelta.y;
            _scroll.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
        }

        public void AddData(UIChattData data)
        {
            TableData.Add(data);
        }
        public void UpdateMyData() //본인이 쓴 채팅이 업데이트 되도록.
        {
          
            InitializeTableView();        
            _scrollRect.verticalNormalizedPosition = 0.0f;
            OnScrollPoschanged(new Vector2(0f, -0.01f));
        }

        public void UpdateData() //남이 채팅에서 썻을때
        {
            _currentContentPos = CachedScrollRect.content.anchoredPosition.y;
            _changedHeight = CachedScrollRect.content.sizeDelta.y - _preContentHeight;
            _preContentHeight = CachedScrollRect.content.sizeDelta.y;
            _currentContentPos += _changedHeight;
            CachedScrollRect.content.anchoredPosition = new Vector2(0f, _currentContentPos);
            InitializeTableView();
            OnScrollPoschanged(new Vector2(0f, -0.01f));
            //_scroll.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
        }

        protected override void Start()
        {
            base.Start();
            LoadAllChattingData();
        }
        private void OnEnable()
        {
            Chatting.Instance.SetChattingMode(ChattMode.All);
        }


        protected override float GetCellHeightAtIndex(int index)
        {
            int _countContent = CheckEnglishByte(TableData[index].Chat);
            _countContent += CheckEnglishByte(TableData[index].Name);

            float _heightContent = 50f * (_countContent / 95 + 1f);
            return _heightContent;
        }
        private int CheckEnglishByte(string text)
        {
            int _byteCount = System.Text.Encoding.Default.GetByteCount(text); ;
            for (int i = 0; i < text.Length; i++)
            {
                if ('a' <= text[i] && text[i] <= 'z' ||
                    'A' <= text[i] && text[i] <= 'Z')
                    _byteCount++;
            }

            return _byteCount;
        }

    }
}
