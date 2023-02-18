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

public class PokerGameSocket : MonoBehaviour
{
    private static PokerGameSocket s_instance = null;
    public static PokerGameSocket Instance { get => s_instance; }
    WebSocket _socket;
    private StompMessageParser messageParser = new StompMessageParser();

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
        _socket.OnMessage += ws_OnMessage; //서버에서 유니티 쪽으로 메세지가 올 경우 실행할 함수를 등록한다.
        _socket.OnOpen += ws_OnOpen;//서버가 연결된 경우 실행할 함수를 등록한다
        _socket.OnClose += ws_OnClose;//서버가 닫힌 경우 실행할 함수를 등록한다.
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

    private void ws_OnMessage(object sender, MessageEventArgs e)
    {
        StompMessage message = messageParser.Deserialize(e.Data);

        if (message.Command == StompCommand.CONNECTED)
        {
            Debug.Log("게임서버 연결되었습니다~");

            if (message.Command == StompCommand.CONNECTED)
            {/*
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("destination", "/topic/match/" + message.Headers.GetValueOrDefault("user-name"));
                headers.Add("id", "sub0");
                StompMessage subscribeMessage = new StompMessage(StompCommand.SUBSCRIBE, "", headers);
                _socket.Send(messageParser.Serialize(subscribeMessage));*/

            }

        }
    }
}
