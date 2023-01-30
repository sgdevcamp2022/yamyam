using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UIRecycleViewControllerSample : UIRecycleViewController<UICellSampleData>
    {
        private void LoadData()
        {
            tableData = new List<UICellSampleData>()
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
            int countContent= CheckEnglishByte(tableData[index].Chat);
            int countName =  CheckEnglishByte(tableData[index].Name);
     
            float heightContent = 50f * (countContent / 65 + 1f) ;
            float heightName = 50f * (countName / 15 + 1f);

            float height = (heightContent > heightName) ? heightContent : heightName;

            return height;
        }
        private int CheckEnglishByte(string text)
        {
            int byteCount = System.Text.Encoding.Default.GetByteCount(text); ;
            for (int i = 0; i < text.Length; i++)
            {
                // if (char.GetUnicodeCategory(tableData[index].chat[i]) == System.Globalization.UnicodeCategory.eng) //�ѱ��� ���
                if ('a' <= text[i] && text[i] <= 'z' ||
                    'A' <= text[i] && text[i] <= 'Z')
                    byteCount++;
            }

            return byteCount;
        }
    }

 
}
