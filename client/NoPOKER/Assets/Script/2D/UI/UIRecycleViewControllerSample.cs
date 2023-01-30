using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace UI
{
    public class UIRecycleViewControllerSample : UIRecycleViewController<UICellSampleData>
    {
        private void LoadData()
        {
            tableData = new List<UICellSampleData>()
            {
                new UICellSampleData { name="WISE" , chat = "�ȳ��ϼ���"},
                new UICellSampleData { name="���߾�" , chat = "ABCDE"},
                new UICellSampleData { name="���" , chat = "��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!" +
                "��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!" +
                "��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!��� �� ȭ����!"},
                new UICellSampleData { name="�����ϰ���Ʈ" , chat = "���͵���ķ���� ���Ű� ȯ���մϴ�!"},
                new UICellSampleData { name="�̻��" , chat = "PMP ��ǥ�� �¾ƿ�? �װ�??"},
                new UICellSampleData { name="���" , chat = "���ü�� ���� ȭ��Ǯ���� ������ ���ܿ�"},
                new UICellSampleData { name="NoPOKER" , chat = "NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!"},
                new UICellSampleData { name="�ȳȳȳȳȳȳȳȳȳȳ�" , chat = "�� �ȳ��� �ƴϰ� ����ΰ���?"},
                new UICellSampleData { name="����" , chat = "�׷���ġ�� ������ �Ҹ� �ִµ��� �׷����ʳ���"},
                new UICellSampleData { name="���" , chat = "����� ���Ⱑ ���ϰ� �������ݾƿ�, �Ϳ���;;"}

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
            int countContent= CheckEnglishByte(tableData[index].chat);
            int countName =  CheckEnglishByte(tableData[index].name);
     
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
