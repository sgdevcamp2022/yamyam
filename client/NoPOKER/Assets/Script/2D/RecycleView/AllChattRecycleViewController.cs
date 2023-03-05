using System.Collections;
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
            TableData = new List<UIChattData>();

            InitializeTableView();
            _preContentHeight = CachedScrollRect.content.sizeDelta.y;
            _scroll.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
        }

        public void AddData(UIChattData data)
        {
            TableData.Add(data);
        }
        public void UpdateMyData() 
        {

            InitializeTableView();
            _scrollRect.verticalNormalizedPosition = 0.0f;
            OnScrollPoschanged(new Vector2(0f, -0.01f));
        }

        public void UpdateData() 
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
            int _byteCount = System.Text.Encoding.Default.GetByteCount(text);
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
