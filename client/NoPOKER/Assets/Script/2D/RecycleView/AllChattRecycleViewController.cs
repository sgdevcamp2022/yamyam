using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AllChattRecycleViewController : UIRecycleViewController<UICellData>
    {
        [SerializeField] RectTransform _scroll;
        [SerializeField] ScrollRect _scrollRect;

        [SerializeField]
        float _preContentHeight;
        float _currentContentPos;
        private float _changedHeight;
        public void LoadAllChattingData()
        {
            TableData = new List<UICellData>()
            {
                new UICellData { Name="" , Chat = "«—±€«—±€«—±€«—±€«—±€«—±€«—±€«—±€«—±€«—±€«—±€«—±€«—±€«—±€«—±€«—±€«—±€"},
                new UICellData { Name="∞◊¿ﬂæÀ" , Chat = "CONTENTCONTENTCONTENTCONTENTCONTENTCONTENTC"},
                /*new UICellData { Name="æ‰æ‰" , Chat = "æ‰æ‰ ∆¿ »≠¿Ã∆√!æ‰æ‰ ∆¿ »≠¿Ã∆√!æ‰æ‰ ∆¿ »≠¿Ã∆√!æ‰æ‰ ∆¿ »≠¿Ã∆√!æ‰æ‰ ∆¿ »≠¿Ã∆√!" +
                "æ‰æ‰ ∆¿ »≠¿Ã∆√!æ‰æ‰ ∆¿ »≠¿Ã∆√!æ‰æ‰ ∆¿ »≠¿Ã∆√!æ‰æ‰ ∆¿ »≠¿Ã∆√!æ‰æ‰ ∆¿ »≠¿Ã∆√!æ‰æ‰ ∆¿ »≠¿Ã∆√!æ‰æ‰ ∆¿ »≠¿Ã∆√!" +
                "æ‰æ‰ ∆¿ »≠¿Ã∆√!æ‰æ‰ ∆¿ »≠¿Ã∆√!æ‰æ‰ ∆¿ »≠¿Ã∆√!æ‰æ‰ ∆¿ »≠¿Ã∆√!"},*/
                new UICellData { Name="Ω∫∏∂¿œ∞‘¿Ã∆Æ" , Chat = "¿©≈Õµ•∫Íƒ∑«¡ø° ø¿Ω≈∞… »Øøµ«’¥œ¥Ÿ!"},
                new UICellData { Name="¿ÃªÁ¥‘" , Chat = "PMP ∏Ò«•ø° ∏¬æ∆ø‰? ±◊∞‘??"},
                new UICellData { Name="øÎøÎ" , Chat = "øÎøÎ√º∏¶ æ≤∏È »≠∞°«Æ∏Æ¥¬ ∏∂π˝¿Ã ª˝∞‹ø‰"},
               new UICellData { Name="NoPOKER" , Chat = "NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!"},
               // new UICellData { Name="≥»≥»≥»≥»≥»≥»≥»≥»≥»≥»≥»" , Chat = "ø÷ ≥»≥»¿Ã æ∆¥œ∞Ì æ‰æ‰¿Œ∞≈¡“?"},
               // new UICellData { Name="¬¡¬¡" , Chat = "±◊∑∏∞‘ƒ°∏È ¬¡¬¡µµ «“∏ª ¿÷¥¬µ•ø‰ ±◊∑∏¡ˆæ ≥™ø‰"},
               // new UICellData { Name="æ‰æ‰" , Chat = "øµæÓ∑Œ æ≤±‚∞° ∆Ì«œ∞Ì ∞£¥‹«œ¿›æ∆ø‰, ±Õø±∞Ì;;"}

            };

            InitializeTableView();
            _preContentHeight = CachedScrollRect.content.sizeDelta.y;
            _scroll.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
        }

        public void AddData(UICellData data)
        {
            TableData.Add(data);
        }
        public void UpdateMyData() //∫ª¿Œ¿Ã æ¥ √§∆√¿Ã æ˜µ•¿Ã∆Æ µ«µµ∑œ.
        {
          
            InitializeTableView();        
            _scrollRect.verticalNormalizedPosition = 0.0f;
            OnScrollPoschanged(new Vector2(0f, -0.01f));
        }

        public void UpdateData() //≥≤¿Ã √§∆√ø°º≠ õß¿ª∂ß
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
