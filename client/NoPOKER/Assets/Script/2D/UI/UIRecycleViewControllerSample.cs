using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UIRecycleViewControllerSample : UIRecycleViewController<UICellSampleData>
    {
        private void LoadData()
        {
            TableData = new List<UICellSampleData>()
            {
                new UICellSampleData { Name="WISE" , Chat = "�ȳ��ϼ���"},
                new UICellSampleData { Name="���߾�" , Chat = "ABCDE"},
                new UICellSampleData { Name="���" , Chat = "��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!" +
                "��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!" +
                "��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!"},
                new UICellSampleData { Name="�����ϰ���Ʈ" , Chat = "���͵���ķ���� ���Ű� ȯ���մϴ�!"},
                new UICellSampleData { Name="�̻��" , Chat = "PMP ��ǥ�� �¾ƿ�? �װ�??"},
                new UICellSampleData { Name="���" , Chat = "���ü�� ���� ȭ��Ǯ���� ������ ���ܿ�"},
                new UICellSampleData { Name="NoPOKER" , Chat = "NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!"},
                new UICellSampleData { Name="�ȳȳȳȳȳȳȳȳȳȳ�" , Chat = "�� �ȳ��� �ƴϰ� ����ΰ���?"},
                new UICellSampleData { Name="����" , Chat = "�׷���ġ�� ������ �Ҹ� �ִµ��� �׷����ʳ���"},
                new UICellSampleData { Name="���" , Chat = "����� ���Ⱑ ���ϰ� �������ݾƿ�, �Ϳ���;;"}

            };
            InitializedTableView();
        }

        protected override void Start()
        {
            base.Start();
            LoadData();
        }

        public void OnPressCell(UIRecycleViewControllerSample cell)
        {
            Debug.Log("Cell Click");
        }

        protected override float GetCellHeightAtIndex(int index)
        { 
            int _countContent= CheckEnglishByte(TableData[index].Chat);
            int _countName =  CheckEnglishByte(TableData[index].Name);
     
            float _heightContent = 50f * (_countContent / 65 + 1f) ;
            float _heightName = 50f * (_countName / 15 + 1f);

            float _height = (_heightContent > _heightName) ? _heightContent : _heightName;

            return _height;
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
