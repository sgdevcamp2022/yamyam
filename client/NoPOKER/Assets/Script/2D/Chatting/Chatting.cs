using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
public class Chatting : MonoBehaviour
{
    private static Chatting s_instance = null;
    public static Chatting Instance { get => s_instance; }
    [SerializeField] UIRecycleViewControllerSample uIRecycleViewControllerSample;
    private string _userName = "‡œ‡œ¾²";
    public string UserName { get => _userName; }

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        if (s_instance == null)
            s_instance = this;
    }

   public void SendChatting(UICellData chattingData)
    {
        uIRecycleViewControllerSample.AddData(chattingData);
        uIRecycleViewControllerSample.UpdataData();

    }
}
