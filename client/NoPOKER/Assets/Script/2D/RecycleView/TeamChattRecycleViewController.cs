using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TeamChattRecycleViewController : UIRecycleViewController<UICellData>
    {
        [SerializeField] RectTransform _scroll;
        [SerializeField] ScrollRect _scrollRect;

        [SerializeField]
        float _preContentHeight;
        float _currentContentPos;
        private float _changedHeight;

        public void LoadTeamChattingData()
        {
            TableData = new List<UICellData>()
            {
                new UICellData { Name="Team" , Chat = "여기는 팀채팅입니다. 팀이요 팀!"},
                new UICellData { Name="겜잘알" , Chat = "팀끼리는 어떤말을 하는건가여?"},
            };

            InitializeTableView();
            _preContentHeight = CachedScrollRect.content.sizeDelta.y;
            _scroll.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
        }


        public void AddData(UICellData data)
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
        }

        protected override void Start()
        {
            base.Start();
            LoadTeamChattingData();
        }
        private void OnEnable()
        {
            Chatting.Instance.SetChattingMode(ChattMode.Team);
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
