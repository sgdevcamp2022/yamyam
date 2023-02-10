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
        _socket.OnMessage += ws_OnMessage; //�������� ����Ƽ ������ �޼����� �� ��� ������ �Լ��� ����Ѵ�.
        _socket.OnOpen += ws_OnOpen;//������ ����� ��� ������ �Լ��� ����Ѵ�
        _socket.OnClose += ws_OnClose;//������ ���� ��� ������ �Լ��� ����Ѵ�.
        _socket.Connect();//������ �����Ѵ�.
        _socket.Send("hello");//�������� "hello"��� �޼����� ������.
    
    }
    void ws_OnMessage(object sender, MessageEventArgs e)
    {
        Debug.Log(e.Data);//���� �޼����� ����� �ֿܼ� ����Ѵ�.
    }
    void ws_OnOpen(object sender, System.EventArgs e)
    {
        Debug.Log("open"); //����� �ֿܼ� "open"�̶�� ��´�.
    }
    void ws_OnClose(object sender, CloseEventArgs e)
    {
        Debug.Log("close"); //����� �ֿܼ� "close"�̶�� ��´�.
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
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.A)); //���߿� AŰ �Է´�� �����������
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

    //���������� ������ ������.
    /*
      public void ClickedAcceptMatching()
    {

    }
     */

}
