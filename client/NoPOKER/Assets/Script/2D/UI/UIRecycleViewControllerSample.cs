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
                new UICellSampleData { name="WISE" , chat = "¾È³çÇÏ¼¼¿ë"},
                new UICellSampleData { name="°×Àß¾Ë" , chat = "ABCDE"},
                new UICellSampleData { name="¾ä¾ä" , chat = "¾ä¾ä ÆÀ È­ÀÌÆÃ!¾ä¾ä ÆÀ È­ÀÌÆÃ!¾ä¾ä ÆÀ È­ÀÌÆÃ!¾ä¾ä ÆÀ È­ÀÌÆÃ!¾ä¾ä ÆÀ È­ÀÌÆÃ!" +
                "¾ä¾ä ÆÀ È­ÀÌÆÃ!¾ä¾ä ÆÀ È­ÀÌÆÃ!¾ä¾ä ÆÀ È­ÀÌÆÃ!¾ä¾ä ÆÀ È­ÀÌÆÃ!¾ä¾ä ÆÀ È­ÀÌÆÃ!¾ä¾ä ÆÀ È­ÀÌÆÃ!¾ä¾ä ÆÀ È­ÀÌÆÃ!" +
                "¾ä¾ä ÆÀ È­ÀÌÆÃ!¾ä¾ä ÆÀ È­ÀÌÆÃ!¾ä¾ä ÆÀ È­ÀÌÆÃ!¾ä¾ä ÆÀ È­ÀÌÆÃ!"},
                new UICellSampleData { name="½º¸¶ÀÏ°ÔÀÌÆ®" , chat = "À©ÅÍµ¥ºêÄ·ÇÁ¿¡ ¿À½Å°É È¯¿µÇÕ´Ï´Ù!"},
                new UICellSampleData { name="ÀÌ»ç´Ô" , chat = "PMP ¸ñÇ¥¿¡ ¸Â¾Æ¿ä? ±×°Ô??"},
                new UICellSampleData { name="¿ë¿ë" , chat = "¿ë¿ëÃ¼¸¦ ¾²¸é È­°¡Ç®¸®´Â ¸¶¹ýÀÌ »ý°Ü¿ä"},
                new UICellSampleData { name="NoPOKER" , chat = "NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!NoPOKER Is Funny!!!"},
                new UICellSampleData { name="³È³È³È³È³È³È³È³È³È³È³È" , chat = "¿Ö ³È³ÈÀÌ ¾Æ´Ï°í ¾ä¾äÀÎ°ÅÁÒ?"},
                new UICellSampleData { name="ÂÁÂÁ" , chat = "±×·¸°ÔÄ¡¸é ÂÁÂÁµµ ÇÒ¸» ÀÖ´Âµ¥¿ä ±×·¸Áö¾Ê³ª¿ä"},
                new UICellSampleData { name="¾ä¾ä" , chat = "¿µ¾î·Î ¾²±â°¡ ÆíÇÏ°í °£´ÜÇÏÀÝ¾Æ¿ä, ±Í¿±°í;;"}

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
                // if (char.GetUnicodeCategory(tableData[index].chat[i]) == System.Globalization.UnicodeCategory.eng) //ÇÑ±ÛÀÏ °æ¿ì
                if ('a' <= text[i] && text[i] <= 'z' ||
                    'A' <= text[i] && text[i] <= 'Z')
                    byteCount++;
            }

            return byteCount;
        }
    }

 
}
