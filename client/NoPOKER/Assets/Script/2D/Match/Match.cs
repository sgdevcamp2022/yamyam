using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Threading;
using System;



public class Match : MonoBehaviour
{
    [SerializeField] private List<GameObject> _loadingObject = new List<GameObject>();
    private IEnumerator _loadingCoroutine;
    private IEnumerator _loadingUICoroutine;

    public WebSocket _socket = null;

    private void Start()
    {
       // var wssv = new WebSocketServer("3.36.131.0", 8080);
        _socket = new WebSocket("ws://3.36.131.0:8080/match-ws");
        _socket.OnMessage += ws_OnMessage; //서버에서 유니티 쪽으로 메세지가 올 경우 실행할 함수를 등록한다.
        _socket.OnOpen += ws_OnOpen;//서버가 연결된 경우 실행할 함수를 등록한다
        _socket.OnClose += ws_OnClose;//서버가 닫힌 경우 실행할 함수를 등록한다.
        _socket.Connect();//서버에 연결한다.
        _socket.Send("hello");//서버에게 "hello"라는 메세지를 보낸다.
    
    }
    void ws_OnMessage(object sender, MessageEventArgs e)
    {
        Debug.Log(e.Data);//받은 메세지를 디버그 콘솔에 출력한다.
    }
    void ws_OnOpen(object sender, System.EventArgs e)
    {
        Debug.Log("open"); //디버그 콘솔에 "open"이라고 찍는다.
    }
    void ws_OnClose(object sender, CloseEventArgs e)
    {
        Debug.Log("close"); //디버그 콘솔에 "close"이라고 찍는다.
    }



    public void MatchLoading()
    {
        for (int i = 0; i < _loadingObject.Count; i++)
        {
            _loadingObject[i].SetActive(true);
        }
        _loadingCoroutine = Loading();
        _loadingUICoroutine = LoadingUI();
        //Connect();
        _socket.Send("hello");
        StartCoroutine(_loadingCoroutine);
       
    }
    public void DisconnectSever()
    {
        try
        {
            if (_socket == null)
                return;

            if (_socket.IsAlive)
                _socket.Close();
        }
        catch(Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void Connect()
    {
        try
        {
            if (_socket == null )
            {
                _socket.Connect();
                Debug.Log("connect!");
            }
            else
            {
                if (_socket == null)
                    Debug.Log("it is null");

            }
                
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void CloseConnect(object sender, CloseEventArgs e)
    {
        DisconnectSever();
    }
    public void Recv(object sender, MessageEventArgs e)
    {
        Debug.Log(e.Data);
        Debug.Log(e.RawData);
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
        StopCoroutine(_loadingUICoroutine);
        StopCoroutine(_loadingCoroutine);  
    }

    public void MatchingExit()
    {
        StopLoading();
        LobbyWindowController.Instance.ActiveAlertWindow();
    }

    public void MatchingSucceed()
    {
        StopLoading();
        LobbyWindowController.Instance.InActiveMatchingWindow();
    }

    //수락누르면 다음씬 가도록.
    /*
      public void ClickedAcceptMatching()
    {

    }
     */

}
