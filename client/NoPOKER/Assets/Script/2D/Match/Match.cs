using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Threading;
using System;
using System.Text;
using Newtonsoft.Json;

public class MatchSendData {
    public string sender = "aaa";
    public string type = "MATCH";
    public Dictionary<string, string> content = new Dictionary<string, string>();
    public int userId = UserInfo.Instance.UserID;
}


public class Match : MonoBehaviour
{
    [SerializeField] private List<GameObject> _loadingObject = new List<GameObject>();
    private IEnumerator _loadingCoroutine;
    private IEnumerator _loadingUICoroutine;

    public WebSocket _socket = null;
    private StringBuilder _matchType = new StringBuilder();
    public void SetMatch2()
    {
        _matchType.Clear();
        _matchType.Append("2P");
    }

    public void SetMatch4()
    {
        _matchType.Clear();
        _matchType.Append("4P");
    }


    void ws_OnMessage(object sender, MessageEventArgs e)
    {
        Debug.Log(e.Data);//받은 메세지를 디버그 콘솔에 출력한다.
    }
    void ws_OnOpen(object sender, System.EventArgs e)
    {
        Debug.Log("open"); //디버그 콘솔에 "open"이라고 찍는다.
        SendStompConnect();
        SendSubScribe();
        SendMatchRequest();
    }
    void ws_OnClose(object sender, CloseEventArgs e)
    {
        Debug.Log("close"); //디버그 콘솔에 "close"이라고 찍는다.
    }

    private void OnDestroy()
    {
        DisconnectSever();
    }

    private void SendStompConnect()
    {
        string _connectMessage = "STOMP\n" +
            "accept-version:1.2\n" +
            "host:localhost\n" +
            "\n" +
            "\0";

        _socket.Send(_connectMessage);
    }

    private void SendMatchRequest()
    {
        MatchSendData data = new MatchSendData();
        data.content.Add("match_type", _matchType.ToString());
        StringBuilder strBuilder = new StringBuilder();
        strBuilder.Append("SEND\n" +
            "destination:/pub/v1/match\n" +
            "\n");
        strBuilder.Append(JsonConvert.SerializeObject(data));
        strBuilder.Append("\n\u0000");

       _socket.Send(strBuilder.ToString());
    }

    private void SendSubScribe()
    {
        
        string _subscribeMessage = "SUBSCRIBE\n" +  
            "id:sub0\n"+
            "destination:/topic/match\n" +
            "\n" +
            "\u0000";

        _socket.Send(_subscribeMessage);
    }

    private void SendUnSubScribe()
    {

        string _subscribeMessage = "UNSUBSCRIBE\n" +
            "id:sub0\n" +
            "destination:/topic/match\n" +
            "\n" +
            "\u0000";

        _socket.Send(_subscribeMessage);
    }

    public void MatchLoading()
    {
     
        _socket = new WebSocket("ws://3.36.64.92:8003/match-ws");
        _socket.OnMessage += ws_OnMessage; //서버에서 유니티 쪽으로 메세지가 올 경우 실행할 함수를 등록한다.
        _socket.OnOpen += ws_OnOpen;//서버가 연결된 경우 실행할 함수를 등록한다
        _socket.OnClose += ws_OnClose;//서버가 닫힌 경우 실행할 함수를 등록한다.
        _socket.Connect();

        for (int i = 0; i < _loadingObject.Count; i++)
        {
            _loadingObject[i].SetActive(true);
        }
        _loadingCoroutine = Loading();
        _loadingUICoroutine = LoadingUI();
        StartCoroutine(_loadingCoroutine);
       
    }
    public void DisconnectSever()
    {
        try
        {
            if (_socket == null)
                return;

            if (_socket.IsAlive)
            {
                SendUnSubScribe();
                _socket.Close();
            }
                
        }
        catch(Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public IEnumerator LoadingUI()
    {
        while(true)
        {           
            for(int i=0;i<_loadingObject.Count;i++)
            {
                _loadingObject[i].SetActive(false);
                yield return new WaitForSeconds(0.5f);
                _loadingObject[i].SetActive(true);
            }               
        }
    }

    public IEnumerator Loading()
    {
        StartCoroutine(_loadingUICoroutine);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.A)); //나중에 A키 입력대신 서버통신으로
        MatchingSucceed();
    }

    public void StopLoading()
    {
        DisconnectSever();

        StopCoroutine(_loadingUICoroutine);
        StopCoroutine(_loadingCoroutine);  
    }

    public void MatchingExit()
    {
        StopLoading();
    }

    public void MatchingSucceed()
    {
        StopLoading();
        LobbyWindowController.Instance.InActiveMatchingWindow();
    }

  

}
