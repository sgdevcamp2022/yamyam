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

public class LobbyMessageSocketData {

    public string type;
    public int id;
    public string nickname;
    public string message;
    
    public void LobbyMessageSetting(string message)
    {
        type = "lobby_message";
        id = UserInfo.Instance.UserID;
        nickname = UserInfo.Instance.NickName;
        this.type = type;
        this.message = message;
    }
}
public class LobbyConnect : MonoBehaviour
{
    private static LobbyConnect s_instance = null;
    public static LobbyConnect Instance { get => s_instance; }
    WebSocket _lobbySocket;
    StringBuilder _urlBuilder = new StringBuilder();
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (s_instance == null)
            s_instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        LobbyServerConnect();
    }
    public void LobbyServerConnect()
    {
        _urlBuilder.Append("ws://127.0.0.1:8001/ws/lobby/");
        _urlBuilder.Append(UserInfo.Instance.UserID + "/");
        _urlBuilder.Append(UserInfo.Instance.NickName);
        _lobbySocket = new WebSocket(_urlBuilder.ToString());

        _lobbySocket.OnOpen += ws_OnOpen;
        _lobbySocket.OnMessage += ws_OnMessage;      
        _lobbySocket.OnClose += ws_OnClose;
        _lobbySocket.Connect();

    }

    private void ws_OnClose(object sender, CloseEventArgs e)
    {
        Debug.Log("close");
    }

    private void ws_OnOpen(object sender, EventArgs e)
    {
            Debug.Log("open");
    }


    private void ws_OnMessage(object sender, MessageEventArgs e)
    {
        Debug.Log(e.Data);

    }


    public void SendAllChattMessage(LobbyMessageSocketData sendMessage)
    {
        Debug.Log("send JSOn Message check : " + sendMessage);
        _lobbySocket.Send(JsonConvert.SerializeObject(sendMessage));
    }
}
