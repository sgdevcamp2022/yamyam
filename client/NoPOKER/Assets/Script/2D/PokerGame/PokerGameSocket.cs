using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Threading;
using System;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using StompHelper;
using Unity.VisualScripting;

public enum PokerMessageType {
    JOIN,
    READY,
    GAME_START,
    FOCUS
}

public class ReadySocketData {
    public string type ="READY";
    public Dictionary<string,int> content = new Dictionary<string, int>();
}

public class PokerUserSocketData {
    public int id;
    public string nickname;
    public int order;
    public int currentChip;
    public int card;
    public PokerUserSocketData(int id, string nickname,int order)
    {
        this.id = id;
        this.nickname = nickname;
        this.order = order;
    }

    public void SetPokerStartData(int currentChip, int card)
    {
        this.currentChip = currentChip;
        this.card = card;
    }
}

public class PokerSocketData {
  
    public string type;
    public Dictionary<string, string> content = new Dictionary<string, string>();
}



public class PokerGameSocket : MonoBehaviour
{
    private static PokerGameSocket s_instance = null;
    public static PokerGameSocket Instance { get => s_instance; }
    WebSocket _socket;
    private StompMessageParser messageParser = new StompMessageParser();
    public int GameRoomID;
    List<PokerUserSocketData> userSocketDataList = new List<PokerUserSocketData>();
    public List<PokerUserSocketData> GetGamePlayersList { get => userSocketDataList; }
    int _pokerGamePeopleNum;
    public int GetPokerGamePeopleNum { get => _pokerGamePeopleNum; }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        if (s_instance == null)
            s_instance = this;

        DontDestroyOnLoad(s_instance);
    }

    public void PokerGameSocketConnect()
    {
        _socket = new WebSocket("ws://15.165.8.102:8004/poker-ws");
        _socket.OnMessage += ws_OnMessage; 
        _socket.OnOpen += ws_OnOpen;
        _socket.OnClose += ws_OnClose;
        _socket.Connect();
    }
    private void SendStompConnect()
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("accept-version", "1.2");
        headers.Add("host", "localhost");

        StompMessage message = new StompMessage(StompCommand.CONNECT, "", headers);

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
    private void ws_OnClose(object sender, CloseEventArgs e)
    {
        Debug.Log("close");

    }

    private void ws_OnOpen(object sender, EventArgs e)
    {
        Debug.Log("open");
        SendStompConnect();
    }

    int receiveStartNum = 0;
    private void ws_OnMessage(object sender, MessageEventArgs e)
    {
        Debug.Log(e.Data);

        StompMessage message = messageParser.Deserialize(e.Data);
        Debug.Log("command: " + message.Command);
        Debug.Log("headers: " + message.Headers);
        Debug.Log("body: " + message.Body);

        switch(message.Command)
        {
            case StompCommand.CONNECTED:
                receiveStartNum = 0;
                Debug.Log("게임서버 연결되었습니다~");
                Match.Instance.DisconnectSever();
                SendStompSubscribe();
                break;
            case StompCommand.MESSAGE:
                PokerSocketData _messagehData = JsonConvert.DeserializeObject<PokerSocketData>(message.Body);

                switch (Enum.Parse(typeof(PokerMessageType), _messagehData.type)) //TYPE 비교
                {
                    case PokerMessageType.JOIN:
                        userSocketDataList.Add(new PokerUserSocketData(
                            Int32.Parse(_messagehData.content["userId"]),
                            _messagehData.content["nickname"],
                            Int32.Parse(_messagehData.content["order"]))
                            );

                        if(Match.Instance.GetMatchType.Equals("2P"))
                        {
                            if (userSocketDataList.Count == 2)
                            {
                                //화면전환.
                                userSocketDataList = userSocketDataList.OrderBy(x => x.order).ToList();
                                _pokerGamePeopleNum = 2;
                                SendReadyRequest(); //준비되었다고 알림보냄
                                                         
                            }
                        }
                        else //4P
                        {
                            if (userSocketDataList.Count == 4)
                            {
                                //화면전환
                            }
                        }
                        break;
                    case PokerMessageType.GAME_START:
                        // try
                        // {

                        PokerUserSocketData _findPokerUserSocketData = userSocketDataList.Find(x => x.id == Int32.Parse(_messagehData.content["id"]));
                        _findPokerUserSocketData.SetPokerStartData(Int32.Parse(_messagehData.content["currentChip"]), Int32.Parse(_messagehData.content["card"]));
                        receiveStartNum++;
                        if (receiveStartNum == _pokerGamePeopleNum) //화면전환
                        {
                            try
                            {
                                GameManager.Instance.ChangeScene(Scenes.PokerGameScene);
                            }
                            catch(Exception ex)
                            {
                                Debug.Log("ERRPR : " + ex);
                            }
                        }

                        break;

                    case PokerMessageType.FOCUS:
                        // body: { "type":"FOCUS","content":{ "id":1},"sender":null,"userId":1,"gameId":0}
                        try
                        {
                            PokerGameManager.Instance.NowTurn = Int32.Parse(_messagehData.content["id"]);
                        Debug.Log("1");
                        PokerGameManager.Instance._pokerGameState = PokerGameState.FOCUS;
                        Debug.Log("2");
             
                        GameManager.Instance.ChangeScene(Scenes.PokerGameScene);
                        Debug.Log("3");
                        PokerGameManager.Instance.ReceiveSocketFlag = true;
                        Debug.Log("4");
                        }
                        catch (Exception ex)
                        {
                            Debug.Log("ERRPR : " + ex);
                        }
                
                break;
                }

                break;
        }
   
    }
    private void SendStompSubscribe()
    {
        Debug.Log("gameRoom ID = " + GameRoomID);
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("destination", "/topic/game/" + GameRoomID);
        headers.Add("id", UserInfo.Instance.UserID.ToString());
        headers.Add("userId", UserInfo.Instance.UserID.ToString());
        headers.Add("nickName", UserInfo.Instance.NickName.ToString());
        StompMessage message = new StompMessage(StompCommand.SUBSCRIBE, "", headers);

        _socket.Send(messageParser.Serialize(message));
    }




    private void SendReadyRequest()
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("destination", "/pub/action");

        ReadySocketData data = new ReadySocketData();
        data.content.Add("userId", UserInfo.Instance.UserID);
        data.content.Add("gameId", GameRoomID);
        Debug.Log("GameRoomID :" + GameRoomID.ToString());
        StompMessage message = new StompMessage( StompCommand.MESSAGE,
            JsonConvert.SerializeObject(data).ToString(), headers);

        _socket.Send(messageParser.Serialize(message));
    }
}
