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
                new UICellSampleData { Name="WISE" , Chat = "¾È³çÇÏ¼¼¿ë"},
                new UICellSampleData { Name="°×Àß¾Ë" , Chat = "ABCDE"},
                new UICellSampleData { Name="¾ä¾ä" , Chat = "¾ä¾ä ÆÀ È­ÀÌÆÃ!¾ä¾ä ÆÀ È­ÀÌÆÃ!¾ä¾ä ÆÀ È­ÀÌÆÃ!¾ä¾ä ÆÀ È­ÀÌÆÃ!¾ä¾ä ÆÀ È­ÀÌÆÃ!" +
                "¾ä¾ä ÆÀ È­ÀÌÆÃ!¾ä¾ä ÆÀ È­ÀÌÆÃ!¾ä¾ä ÆÀ È­ÀÌÆÃ!¾ä¾ä ÆÀ È­ÀÌÆÃ!¾ä¾ä ÆÀ È­ÀÌÆÃ!¾ä¾ä ÆÀ È­ÀÌÆÃ!¾ä¾ä ÆÀ È­ÀÌÆÃ!" +
                "¾ä¾ä ÆÀ È­ÀÌÆÃ!¾ä¾ä ÆÀ È­ÀÌÆÃ!¾ä¾ä ÆÀ È­ÀÌÆÃ!¾ä¾ä ÆÀ È­ÀÌÆÃ!"},
                new UICellSampleData { Name="½º¸¶ÀÏ°ÔÀÌÆ®" , Chat = "À©ÅÍµ¥ºêÄ·ÇÁ¿¡ ¿À½Å°É È¯¿µÇÕ´Ï´Ù!"},
                new UICellSampleData { Name="ÀÌ»ç´Ô" , Chat = "PMP ¸ñÇ¥¿¡ ¸Â¾Æ¿ä? ±×°Ô??"},
                new UICellSampleData { Name="¿ë¿ë" , Chat = "¿ë¿ëÃ¼¸¦ ¾²¸é È­°¡Ç®¸®´Â ¸¶¹ýÀÌ »ý°Ü¿ä"},
                new UICellSampleData { Name="NoPOKER" , Chat = "NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!"},
                new UICellSampleData { Name="³È³È³È³È³È³È³È³È³È³È³È" , Chat = "¿Ö ³È³ÈÀÌ ¾Æ´Ï°í ¾ä¾äÀÎ°ÅÁÒ?"},
                new UICellSampleData { Name="ÂÁÂÁ" , Chat = "±×·¸°ÔÄ¡¸é ÂÁÂÁµµ ÇÒ¸» ÀÖ´Âµ¥¿ä ±×·¸Áö¾Ê³ª¿ä"},
                new UICellSampleData { Name="¾ä¾ä" , Chat = "¿µ¾î·Î ¾²±â°¡ ÆíÇÏ°í °£´ÜÇÏÀÝ¾Æ¿ä, ±Í¿±°í;;"}

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
                // if (char.GetUnicodeCategory(tableData[index].chat[i]) == System.Globalization.UnicodeCategory.eng) //ÇÑ±ÛÀÏ °æ¿ì
                if ('a' <= text[i] && text[i] <= 'z' ||
                    'A' <= text[i] && text[i] <= 'Z')
                    byteCount++;
            }

            return byteCount;
        }
    }

 
}
