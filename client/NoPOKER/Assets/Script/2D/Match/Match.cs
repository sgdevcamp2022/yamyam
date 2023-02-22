using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Threading;
using System;
using System.Text;
using Newtonsoft.Json;
using StompHelper;
using Unity.VisualScripting;


public enum MatchMessagetype {
    MATCH,
    MATCH_DONE,
    ERROR
}

public class MatchSendData
{
    public string sender = "aaa";
    public string type = "MATCH";
    public Dictionary<string, string> content = new Dictionary<string, string>();
    public int userId = UserInfo.Instance.UserID;
}


public class Match : MonoBehaviour
{
    private static Match s_instance = null;
    public static Match Instance { get => s_instance; }

    [SerializeField] private List<GameObject> _loadingObject = new List<GameObject>();
    private IEnumerator _loadingCoroutine;
    private IEnumerator _loadingUICoroutine;

    public WebSocket _socket = null;
    private StringBuilder _matchType = new StringBuilder();
    public StringBuilder GetMatchType { get => _matchType; }
    private string username = "";
    private StompMessageParser messageParser = new StompMessageParser();

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        if (s_instance == null)
            s_instance = this;
    }

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
        StompMessage message = messageParser.Deserialize(e.Data);
        Debug.Log(e.Data);
        Debug.Log("command: " + message.Command);
        Debug.Log("headers: " + message.Headers);
        Debug.Log("body: " + message.Body);

        if (message.Command == StompCommand.CONNECTED)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("destination", "/topic/match/" + message.Headers.GetValueOrDefault("user-name"));
            headers.Add("id", "sub0");
            StompMessage subscribeMessage = new StompMessage(StompCommand.SUBSCRIBE, "", headers);

            _socket.Send(messageParser.Serialize(subscribeMessage));
        }
        else if(message.Command == StompCommand.MESSAGE)
        {
            MatchSendData _matchData = JsonConvert.DeserializeObject<MatchSendData>(message.Body);


            switch(Enum.Parse(typeof(MatchMessagetype) , _matchData.type))
            {
                case MatchMessagetype.MATCH:

                    break;
                case MatchMessagetype.MATCH_DONE:
                    //게임서버 열기.

                    PokerGameSocket.Instance.GameRoomID = Int32.Parse(_matchData.content["gameId"]);
                    PokerGameSocket.Instance.PokerGameSocketConnect();
                    //게임ID
                    //매칭서버 닫기.

                    break;
                case MatchMessagetype.ERROR:

                    break;

            }

        }


        // TODO : e.Data -> user-name 이 존재하는 경우에 추출
    }
    void ws_OnOpen(object sender, System.EventArgs e)
    {
        Debug.Log("open"); //디버그 콘솔에 "open"이라고 찍는다.
        SendStompConnect();
        SendMatchRequest();
    }
    void ws_OnClose(object sender, CloseEventArgs e)
    {
        Debug.Log("close"); //디버그 콘솔에 "close"이라고 찍는다.
    }

    void ws_OnError(object sender, CloseEventArgs e)
    {
        Debug.Log("close"); //디버그 콘솔에 "close"이라고 찍는다.
    }

    private void OnDestroy()
    {
        DisconnectSever();
    }

    private void SendStompConnect()
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("accept-version", "1.2");
        headers.Add("host", "localhost");

        StompMessage message = new StompMessage(StompCommand.CONNECT, "", headers);

        _socket.Send(messageParser.Serialize(message));
    }

    private void SendMatchRequest()
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("destination", "/pub/v1/match");

        MatchSendData data = new MatchSendData();
        data.content.Add("match_type", _matchType.ToString());

        StompMessage message = new StompMessage(StompCommand.SEND,
            JsonConvert.SerializeObject(data).ToString(), headers);

        _socket.Send(messageParser.Serialize(message));
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
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public IEnumerator LoadingUI()
    {
        while (true)
        {
            for (int i = 0; i < _loadingObject.Count; i++)
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