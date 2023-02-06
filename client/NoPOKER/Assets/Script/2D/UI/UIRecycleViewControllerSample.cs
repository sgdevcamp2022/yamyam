using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UIRecycleViewControllerSample : UIRecycleViewController<UICellData>
    {
        [SerializeField] RectTransform _scroll;
        private void LoadData()
        {
            TableData = new List<UICellData>()
            {
                new UICellData { Name="" , Chat = "«—±€«—±€«—±€«—±€«—±€«—±€«—±€«—±€«—±€«—±€«—±€«—±€«—±€«—±€«—±€«—±€«—±€«—±€«—"},
                new UICellData { Name="∞◊¿ﬂæÀ" , Chat = "CONTENTCONTENTCONTENTCONTENTCONTENTCONTENTC"},
                new UICellData { Name="æ‰æ‰" , Chat = "æ‰æ‰ ∆¿ »≠¿Ã∆√!æ‰æ‰ ∆¿ »≠¿Ã∆√!æ‰æ‰ ∆¿ »≠¿Ã∆√!æ‰æ‰ ∆¿ »≠¿Ã∆√!æ‰æ‰ ∆¿ »≠¿Ã∆√!" +
                "æ‰æ‰ ∆¿ »≠¿Ã∆√!æ‰æ‰ ∆¿ »≠¿Ã∆√!æ‰æ‰ ∆¿ »≠¿Ã∆√!æ‰æ‰ ∆¿ »≠¿Ã∆√!æ‰æ‰ ∆¿ »≠¿Ã∆√!æ‰æ‰ ∆¿ »≠¿Ã∆√!æ‰æ‰ ∆¿ »≠¿Ã∆√!" +
                "æ‰æ‰ ∆¿ »≠¿Ã∆√!æ‰æ‰ ∆¿ »≠¿Ã∆√!æ‰æ‰ ∆¿ »≠¿Ã∆√!æ‰æ‰ ∆¿ »≠¿Ã∆√!"},
                new UICellData { Name="Ω∫∏∂¿œ∞‘¿Ã∆Æ" , Chat = "¿©≈Õµ•∫Íƒ∑«¡ø° ø¿Ω≈∞… »Øøµ«’¥œ¥Ÿ!"},
                new UICellData { Name="¿ÃªÁ¥‘" , Chat = "PMP ∏Ò«•ø° ∏¬æ∆ø‰? ±◊∞‘??"},
                new UICellData { Name="øÎøÎ" , Chat = "øÎøÎ√º∏¶ æ≤∏È »≠∞°«Æ∏Æ¥¬ ∏∂π˝¿Ã ª˝∞‹ø‰"},
                new UICellData { Name="NoPOKER" , Chat = "NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!"},
                new UICellData { Name="≥»≥»≥»≥»≥»≥»≥»≥»≥»≥»≥»" , Chat = "ø÷ ≥»≥»¿Ã æ∆¥œ∞Ì æ‰æ‰¿Œ∞≈¡“?"},
                new UICellData { Name="¬¡¬¡" , Chat = "±◊∑∏∞‘ƒ°∏È ¬¡¬¡µµ «“∏ª ¿÷¥¬µ•ø‰ ±◊∑∏¡ˆæ ≥™ø‰"},
                new UICellData { Name="æ‰æ‰" , Chat = "øµæÓ∑Œ æ≤±‚∞° ∆Ì«œ∞Ì ∞£¥‹«œ¿›æ∆ø‰, ±Õø±∞Ì;;"}

            };
            UpdataData();
        }

        public void AddData(UICellData data)
        {
            TableData.Add(data);
        }
        public void UpdataData()
        {
            InitializedTableView();
            _scroll.position = new Vector3(0f, 10000f,0f);
            _scroll.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
        }

        protected override void Start()
        {
            base.Start();
            LoadData();
        }


        protected override float GetCellHeightAtIndex(int index)
        { 
            int _countContent= CheckEnglishByte(TableData[index].Chat);
            _countContent +=  CheckEnglishByte(TableData[index].Name);
     
            float _heightContent = 50f * (_countContent / 100 + 1f) ;
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
