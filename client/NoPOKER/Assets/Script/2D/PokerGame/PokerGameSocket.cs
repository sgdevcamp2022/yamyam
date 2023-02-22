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
    FOCUS,
    BET,
    DIE,
    RESULT,
    OPEN,
}

public class PokerSocketIntData {
    public string type ;
    public Dictionary<string,int> content = new Dictionary<string, int>();
}

public class PokerGameStartSocketData {
    public string type;
    public Dictionary<string, PokerStartPlayerSocketData[]> content = new Dictionary<string, PokerStartPlayerSocketData[]>();
}
public class PokerStartPlayerSocketData {
    public int id;
    public int currentChip;
    public int card;
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

public class PokerSocketStringData {
  
    public string type;
    public Dictionary<string, string> content = new Dictionary<string, string>();
}
public class PokerSocketType {

    public string type;
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

    private bool IsChangeScene = false;

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

    private void Update()
    {
        if(IsChangeScene)
        {
            StartCoroutine(FocusCoroutine());
  
            IsChangeScene = false;
        }

        if(Input.GetKeyDown(KeyCode.D))
        {
           
            DisconnectSever();
        }
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
    public void DisconnectSever()
    { 
                SendUnSubScribe();
                _socket.Close();
        Debug.Log("게임서버 강제종료");

    }
    private void ws_OnOpen(object sender, EventArgs e)
    {
        Debug.Log("open");
        SendStompConnect();
    }
    PokerSocketStringData _messageData;
    PokerSocketIntData _messageIntData;
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
                PokerSocketType _socketType = JsonConvert.DeserializeObject<PokerSocketType>(message.Body);
                // _messageData = JsonConvert.DeserializeObject<PokerSocketData>(message.Body);
                switch (Enum.Parse(typeof(PokerMessageType), _socketType.type)) //TYPE 비교
                {
                    case PokerMessageType.JOIN:
                        _messageData = JsonConvert.DeserializeObject<PokerSocketStringData>(message.Body);
                        userSocketDataList.Add(new PokerUserSocketData(
                            Int32.Parse(_messageData.content["userId"]),
                            _messageData.content["nickname"],
                            Int32.Parse(_messageData.content["order"]))
                            );

                        if(Match.Instance.GetMatchType.Equals("2P"))
                        {
                            if (userSocketDataList.Count == 2)
                            {
                             
                                userSocketDataList = userSocketDataList.OrderBy(x => x.order).ToList();
                                _pokerGamePeopleNum = 2;
                                SendReadyRequest(); //준비되었다고 알림보냄
                                                         
                            }
                        }
                        else //4P
                        {
                            if (userSocketDataList.Count == 4)
                            {
                               
                            }
                        }
                        break;
                    case PokerMessageType.GAME_START:
                        try
                        {
                            PokerGameStartSocketData _startMessageData = JsonConvert.DeserializeObject<PokerGameStartSocketData>(message.Body);
                            PokerStartPlayerSocketData[] _startPlayerDatas = _startMessageData.content["playerInfos"];
                            for (int i = 0; i < _pokerGamePeopleNum; i++)
                            {
                                int _findIndex = userSocketDataList.FindIndex(x => x.id == _startPlayerDatas[i].id);
                                userSocketDataList[_findIndex].SetPokerStartData(_startPlayerDatas[i].currentChip, _startPlayerDatas[i].card);
                            }
                            IsChangeScene = true;
                        }
                        catch (Exception ex)
                        {
                            Debug.Log("ERROR : " + ex);
                        }
                      
                        break;

                    case PokerMessageType.FOCUS:
                        try
                        {
                            _messageData = JsonConvert.DeserializeObject<PokerSocketStringData>(message.Body);
                            if (GameManager.Instance.CheckNowScene() == Scenes.PokerGameScene)
                            {
                                PokerGameManager.Instance.NowTurnUserId = Int32.Parse(_messageData.content["id"]);
                                PokerGameManager.Instance._pokerGameState = PokerGameState.FOCUS;
                                PokerGameManager.Instance.ReceiveSocketFlag = true;
                            }
                        }
                        catch(Exception ex)
                        {
                            Debug.Log("ERROR : " + ex);
                        }
                        break;
                    case PokerMessageType.BET:
                        try
                        {                       
                            _messageIntData = JsonConvert.DeserializeObject<PokerSocketIntData>(message.Body);
                            if (Int32.Parse(_messageIntData.content["playerId"].ToString()) != UserInfo.Instance.UserID)
                            {
                                PokerGameManager.Instance.receivedBattingInfo.id = _messageIntData.content["playerId"];
                            PokerGameManager.Instance.receivedBattingInfo.betAmout = _messageIntData.content["betAmount"];
                            PokerGameManager.Instance.receivedBattingInfo.currentAmount = _messageIntData.content["currentAmount"];
                            PokerGameManager.Instance.receivedBattingInfo.totalAmount = _messageIntData.content["totalAmount"];

                  
                                PokerGameManager.Instance._pokerGameState = PokerGameState.BET;
                                PokerGameManager.Instance.ReceiveSocketFlag = true;
                          }
                        }
                        catch (Exception ex)
                        {
                            Debug.Log("ERROR : " + ex);
                        }
                        break;

                    case PokerMessageType.RESULT:
                        try
                        {
                            Debug.Log("RECEIVED RESULT");

                        }
                        catch (Exception ex)
                        {
                            Debug.Log("ERROR : " + ex);
                        }
                        break;
                }

                break;
        }
   
    }

    IEnumerator FocusCoroutine()
    {

        // yield return new WaitUntil(() => GameManager.Instance.CheckNowScene() == Scenes.PokerGameScene);
        yield return new WaitUntil(() => _messageData.type.Equals("FOCUS"));
        GameManager.Instance.ChangeScene(Scenes.PokerGameScene);
        StartCoroutine( FirstFocus());
    }

    public IEnumerator FirstFocus()
    {
        Debug.Log("FirstFocus");
        yield return new WaitUntil(() => GameManager.Instance.CheckNowScene() == Scenes.PokerGameScene);
        Debug.Log("화면전환됨!");
        PokerGameManager.Instance.NowTurnUserId = Int32.Parse(_messageData.content["id"]);
        Debug.Log("NOW TURN : " + Int32.Parse(_messageData.content["id"]));

        PokerGameManager.Instance._pokerGameState = PokerGameState.FOCUS;
        PokerGameManager.Instance.ReceiveSocketFlag = true;


       
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

        PokerSocketIntData data = new PokerSocketIntData();
        data.type = "READY";
        data.content.Add("userId", UserInfo.Instance.UserID);
        data.content.Add("gameId", GameRoomID);
        StompMessage message = new StompMessage( StompCommand.MESSAGE,
            JsonConvert.SerializeObject(data).ToString(), headers);

        _socket.Send(messageParser.Serialize(message));
    }

    public void SendBettingRequest(int battingChipNum)
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("destination", "/pub/action");

        PokerSocketIntData data = new PokerSocketIntData();
        data.type = "BET";
        data.content.Add("userId", UserInfo.Instance.UserID);
        data.content.Add("betAmount", battingChipNum);
        data.content.Add("gameId", GameRoomID);
        StompMessage message = new StompMessage(StompCommand.MESSAGE,
            JsonConvert.SerializeObject(data).ToString(), headers);

        _socket.Send(messageParser.Serialize(message));
    }

    public void SendDieRequest()
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("destination", "/pub/action");

        PokerSocketIntData data = new PokerSocketIntData();
        data.type = "DIE";
        data.content.Add("id", UserInfo.Instance.UserID);
        data.content.Add("gameId", GameRoomID);
        StompMessage message = new StompMessage(StompCommand.MESSAGE,
            JsonConvert.SerializeObject(data).ToString(), headers);

        _socket.Send(messageParser.Serialize(message));
    }
}
