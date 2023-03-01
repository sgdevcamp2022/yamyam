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
using Newtonsoft.Json.Linq;

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

public class PokerGameStartSocketData
{
    public string type;
    public Hashtable content = new Hashtable();
}

public class PokerGameEndData
{
    public string type;
    public Dictionary<string, PokerStartPlayerSocketData> content = new Dictionary<string, PokerStartPlayerSocketData>();
}

public class PokerGameStartContentData
{
    public int totalBetAmount;
    public PokerStartPlayerSocketData[] playerInfos;
}

public class PokerGameResultSocketData {
    public string type;
    public Dictionary<string, PokerResultPlayerSocketData[]> content = new Dictionary<string, PokerResultPlayerSocketData[]>();
}

public class PokerStartPlayerSocketData {
    public int id;
    public int currentChip;
    public int card;

    public PokerStartPlayerSocketData(int id, int currentChip, int card)
    {
        this.id = id;
        this.currentChip = currentChip;
        this.card = card;
    }
}

public class PokerResultPlayerSocketData {
    public int id;
    public bool result;
    public int currentChip;
}


public class PokerUserSocketData {
    public int id;
    public string nickname;
    public int order;
    public int currentChip;
    public int card;
    public bool result;
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

    public void SetPokerResultData(bool result, int currentChip)
    {
        this.currentChip = currentChip;
        this.result = result;
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
    public long TotalBetAmount;
    private bool IsStart = false;
    private bool IsChange3DScene = false;
    private bool IsFirstTurn = false;
    private int _nowTurnUserId;
    private bool IsFocus = false;
    public int GetNowTurnUserID { get => _nowTurnUserId; }
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
        if(IsStart)
        {
            StartCoroutine(StartCoroutine());
            IsStart = false;
        }
        if(IsChange3DScene)
        {
            GameManager.Instance.ChangeScene(Scenes.ActionGameScene);
            IsChange3DScene = false;
        }
if(IsFocus)
        {
            StartCoroutine(WaitSetting());
            IsFocus = false;
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

        string _unsubscribeMessage = "UNSUBSCRIBE\n" +
            "id:sub0\n" +
            "destination:/topic/match\n" +
            "\n" +
            "\u0000";

        _socket.Send(_unsubscribeMessage);
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
    PokerGameStartSocketData _startMessageData = new PokerGameStartSocketData();
    PokerGameResultSocketData _resultMessageData;
    PokerGameEndData _endMessageData = new PokerGameEndData();
    private void ws_OnMessage(object sender, MessageEventArgs e)
    {
        Debug.Log(e.Data);

        StompMessage message = messageParser.Deserialize(e.Data);
        Debug.Log("command: " + message.Command);
        Debug.Log("headers: " + message.Headers);
        Debug.Log("body: " + message.Body);

        try
        { 


        switch (message.Command)
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
                switch (_socketType.type) //TYPE 비교
                {
                    case "JOIN":
                            PokerSocketStringData  _messageData = JsonConvert.DeserializeObject<PokerSocketStringData>(message.Body);
                            userSocketDataList.Add(new PokerUserSocketData(
                            Int32.Parse(_messageData.content["userId"]),
                            _messageData.content["nickname"],
                            Int32.Parse(_messageData.content["order"]))
                            );                   

                                userSocketDataList = userSocketDataList.OrderBy(x => x.order).ToList();
                                _pokerGamePeopleNum = userSocketDataList.Count;
                                SendReadyRequest(); //준비되었다고 알림보냄

                        break;
                    case "GAME_START":                                           
                                 _startMessageData = JsonConvert.DeserializeObject<PokerGameStartSocketData>(message.Body);
                                JArray _startPlayerDataJArray = (JArray)_startMessageData.content["playerInfos"];
                                foreach (JObject jobject in _startPlayerDataJArray)
                                {
                                    int _findIndex = userSocketDataList.FindIndex(x => x.id == (int)jobject["id"]);
                                    if ((int)jobject["currentChip"] == 0) //초기자금이 0인경우
                                    {
                                        if ((int)jobject["id"] == UserInfo.Instance.UserID) //그게 나라면
                                        {
                                            PokerWindowController.Instance.ActiveLooseWindow();
                                            DisconnectSever();
                                        }
                                        else
                                        {
                                            userSocketDataList.RemoveAt(_findIndex);
                                        }
                                    }
                                    else
                                    {
                                      userSocketDataList[_findIndex].SetPokerStartData((int)jobject["currentChip"], (int)jobject["card"]);      
                                    }
                                                             
                                }
                           
                            _pokerGamePeopleNum = userSocketDataList.Count;
            
                 //화면에 유저들 렌더링하기.
                            IsStart = true; 
                            
                

                        break;

                    case "FOCUS":
                       

                                _startMessageData.type = "FOCUS";
                            _messageIntData = JsonConvert.DeserializeObject<PokerSocketIntData>(message.Body);
                            if (GameManager.Instance.CheckNowScene() == Scenes.PokerGameScene)
                            {
                                    Debug.Log("안들어왔나?");
                                IsFocus = true;
                            }      

                        break;

                        case "CALL":
                        case "RAISE":
                        case "ALLIN":                           
                        case "BET":   
                                _messageIntData = JsonConvert.DeserializeObject<PokerSocketIntData>(message.Body);

                            string type = _socketType.type;

                            PokerGameManager.Instance.ResultUserUiPos =
                                         PokerGameManager.Instance.GetPlayerUiOrders.FindIndex
                                         (x => x.id == _messageIntData.content["playerId"]);

                            if (_messageIntData.content["playerId"] != UserInfo.Instance.UserID)
                            {
                                PokerGameManager.Instance.receivedBattingInfo.id = _messageIntData.content["playerId"];
                                PokerGameManager.Instance.receivedBattingInfo.betAmout = _messageIntData.content["betAmount"];
                                PokerGameManager.Instance.receivedBattingInfo.currentAmount = _messageIntData.content["currentAmount"];
                                PokerGameManager.Instance.receivedBattingInfo.totalAmount = _messageIntData.content["totalAmount"];

                                Debug.Log("receivedBattingInfo.id " + PokerGameManager.Instance.receivedBattingInfo.id);
                                Debug.Log("receivedBattingInfo.betAmout " + PokerGameManager.Instance.receivedBattingInfo.betAmout);
                                Debug.Log("receivedBattingInfo.currentAmount " + PokerGameManager.Instance.receivedBattingInfo.currentAmount);
                                Debug.Log("receivedBattingInfo.totalAmount " + PokerGameManager.Instance.receivedBattingInfo.totalAmount);


                                PokerGameManager.Instance.GetUserInfo().SetNewData(PokerGameManager.Instance.receivedBattingInfo.currentAmount);
                                if(type.Equals("CALL"))
                                {
                                    PokerGameManager.Instance._pokerGameState = PokerGameState.CALL;
                                }
                                else if(type.Equals("RAISE"))
                                {
                                    PokerGameManager.Instance._pokerGameState = PokerGameState.RAISE;
                                }
                                else if (type.Equals("ALLIN"))
                                {
                                    PokerGameManager.Instance._pokerGameState = PokerGameState.ALLIN;
                                }

                                PokerGameManager.Instance.ReceiveSocketFlag = true;                                   
                             }
                        break;

                    case "DIE":
                        try
                        {
                            _messageIntData = JsonConvert.DeserializeObject<PokerSocketIntData>(message.Body);
                            if (_messageIntData.content["playerId"] != UserInfo.Instance.UserID)//남이 죽은거
                            {


                                PokerGameManager.Instance.ResultUserUiPos =
                                    PokerGameManager.Instance.GetPlayerUiOrders.FindIndex
                                    (x => x.id == _messageIntData.content["playerId"]);


                                PokerGameManager.Instance._pokerGameState = PokerGameState.DIE;
                                PokerGameManager.Instance.ReceiveSocketFlag = true;
                                Debug.Log("RECEIVED RESULT");
                            }


                        }
                        catch (Exception ex)
                        {
                            Debug.Log("ERROR : " + ex);
                        }
                        break;
                    case "RESULT":
                        try
                        {//RESULT보여주기.
                                _resultMessageData = JsonConvert.DeserializeObject<PokerGameResultSocketData>(message.Body);
                                PokerResultPlayerSocketData[] _resultPlayerDatas = _resultMessageData.content["playerInfos"];
                                for (int i = 0; i < _pokerGamePeopleNum; i++)
                                {
                                    int _findIndex = userSocketDataList.FindIndex(x => x.id == _resultPlayerDatas[i].id);
                                    userSocketDataList[_findIndex].SetPokerResultData(_resultPlayerDatas[i].result, _resultPlayerDatas[i].currentChip);
                                }
                                Debug.Log("RECEIVED RESULTRECEIVED RESULTRECEIVED RESULTRECEIVED RESULTRECEIVED RESULT");
                                PokerGameManager.Instance.ResultPlayerDatas = _resultPlayerDatas;
                                //
                        }
                        catch (Exception ex)
                        {
                            Debug.Log("ERROR : " + ex);
                        }
                        break;
                        case  "OPEN":
                            try
                            {
                                Debug.Log("OPEN OPENOPEN OPENOPEN OPENOPEN OPENOPEN OPENOPEN OPEN");
                                PokerGameManager.Instance._pokerGameState = PokerGameState.OPEN;
                            }
                            catch (Exception ex)
                            {
                                Debug.Log("ERROR : " + ex);
                            }
                            break;
                        case "GAME_END":
                           // _endMessageData = JsonConvert.DeserializeObject<PokerGameEndData>(message.Body);
                            //PokerStartPlayerSocketData _startPlayerDatas = _endMessageData.content["playerInfos"];
                            break;
                    }

                break;
        }



    }

        catch(Exception ex)
        {
            Debug.Log(ex);
        }

    }


    IEnumerator WaitSetting()
    {
        yield return new WaitUntil(() => PokerGameManager.Instance._pokerGameState == PokerGameState.NONE); //NONE이 될때까지 기다리기.

        PokerGameManager.Instance.NowTurnUserId = _messageIntData.content["id"];
        PokerGameManager.Instance._pokerGameState = PokerGameState.FOCUS;
        PokerGameManager.Instance.ReceiveSocketFlag = true;
    }

    IEnumerator StartCoroutine()
    {
        yield return new WaitUntil(() => _startMessageData.type.Equals("FOCUS")); //start하고나서 처음 FOCUS가 들어왔을 때
       // StartCoroutine( FirstFocus());

        if (GameManager.Instance.CheckNowScene() == Scenes.PokerGameScene) //게임을 다시 시작하는거라면?
        {
            PokerGameManager.Instance.IsFirstTurn = true;
            PokerGameManager.Instance.StartPokerGame();       
        }
         
        else if (GameManager.Instance.CheckNowScene() == Scenes.LobbyScene) //로비에서 포커게임으로 넘어오는거라면?
        {
            GameManager.Instance.ChangeScene(Scenes.PokerGameScene);
            StartCoroutine(FirstFocus());       
        }
           
    }

    public IEnumerator FirstFocus()
    {
        Debug.Log("FirstFocus");

        yield return new WaitUntil(() => GameManager.Instance.CheckNowScene() == Scenes.PokerGameScene); //포커게임씬으로 바뀔때까지 기다리기.

        PokerGameManager.Instance.NowTurnUserId = _messageIntData.content["id"];
        Debug.Log("NOW TURN : " + _messageIntData.content["id"]);

        PokerGameManager.Instance.IsFirstTurn = true;
        PokerGameManager.Instance._pokerGameState = PokerGameState.NONE;
        PokerGameManager.Instance.StartPokerGame();       
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

    public void SendCallRequest(int battingChipNum)
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("destination", "/pub/action");

        PokerSocketIntData data = new PokerSocketIntData();
        data.type = "CALL";
        data.content.Add("userId", UserInfo.Instance.UserID);
        data.content.Add("betAmount", battingChipNum);
        data.content.Add("gameId", GameRoomID);
        StompMessage message = new StompMessage(StompCommand.MESSAGE,
            JsonConvert.SerializeObject(data).ToString(), headers);

        _socket.Send(messageParser.Serialize(message));
    }

    public void SendRaiseRequest(int battingChipNum)
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("destination", "/pub/action");

        PokerSocketIntData data = new PokerSocketIntData();
        data.type = "RAISE";
        data.content.Add("userId", UserInfo.Instance.UserID);
        data.content.Add("betAmount", battingChipNum);
        data.content.Add("gameId", GameRoomID);
        StompMessage message = new StompMessage(StompCommand.MESSAGE,
            JsonConvert.SerializeObject(data).ToString(), headers);

        _socket.Send(messageParser.Serialize(message));
    }

    public void SendAllInRequest(int battingChipNum)
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("destination", "/pub/action");

        PokerSocketIntData data = new PokerSocketIntData();
        data.type = "ALLIN";
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
