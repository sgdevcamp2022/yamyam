using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIRecycleViewControllerSample : UIRecycleViewController<UICellData>
    {
        [SerializeField] RectTransform _scroll;
        [SerializeField] ScrollRect _scrollRect;

        [SerializeField]
        float _preContentHeight;
        float _currentContentPos;
        private float _changedHeight;
        private void LoadData()
        {
            TableData = new List<UICellData>()
            {
                new UICellData { Name="" , Chat = "�ѱ��ѱ��ѱ��ѱ��ѱ��ѱ��ѱ��ѱ��ѱ��ѱ��ѱ��ѱ��ѱ��ѱ��ѱ��ѱ��ѱ�"},
                new UICellData { Name="���߾�" , Chat = "CONTENTCONTENTCONTENTCONTENTCONTENTCONTENTC"},
                /*new UICellData { Name="���" , Chat = "��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!" +
                "��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!" +
                "��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!"},*/
                new UICellData { Name="�����ϰ���Ʈ" , Chat = "���͵���ķ���� ���Ű� ȯ���մϴ�!"},
                new UICellData { Name="�̻��" , Chat = "PMP ��ǥ�� �¾ƿ�? �װ�??"},
                new UICellData { Name="���" , Chat = "���ü�� ���� ȭ��Ǯ���� ������ ���ܿ�"},
               new UICellData { Name="NoPOKER" , Chat = "NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!"},
               // new UICellData { Name="�ȳȳȳȳȳȳȳȳȳȳ�" , Chat = "�� �ȳ��� �ƴϰ� ����ΰ���?"},
               // new UICellData { Name="����" , Chat = "�׷���ġ�� ������ �Ҹ� �ִµ��� �׷����ʳ���"},
               // new UICellData { Name="���" , Chat = "����� ���Ⱑ ���ϰ� �������ݾƿ�, �Ϳ���;;"}

            };

            InitializeTableView();
            _preContentHeight = CachedScrollRect.content.sizeDelta.y;
            _scroll.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
        }

        public void AddData(UICellData data)
        {
            TableData.Add(data);
        }
        public void UpdataMyData() //������ �� ä���� ������Ʈ �ǵ���.
        {
          
            InitializeTableView();        
            _scrollRect.verticalNormalizedPosition = 0.0f;
            OnScrollPoschanged(new Vector2(0f, -0.01f));
        }

        public void UpdataData() //���� ä�ÿ��� ������
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
            LoadData();
        }


        protected override float GetCellHeightAtIndex(int index)
        {
            int _countContent = CheckEnglishByte(TableData[index].Chat);
            _countContent += CheckEnglishByte(TableData[index].Name);

            float _heightContent = 50f * (_countContent / 110 + 1f);
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
