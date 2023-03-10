using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using System;

public class Logout : MonoBehaviour
{
    [SerializeField] Button _logoutButton;
    static HttpClient _httpClient;
    HttpContent _httpContent;
    static HttpResponseMessage _response;
    static string _logoutUrl = "http://127.0.0.1:8000/accounts/logout/";


    void Start()
    {
        _logoutButton.onClick.AddListener(() => LobbyWindowController.Instance.ActiveAlertWindow(LobbyAlertMessage.Logout));
        _logoutButton.onClick.AddListener(() => LobbyWindowController.Instance.InActiveSettingWindow());
        _logoutButton.onClick.AddListener(() => LogOutWebRequest());
    }


    public static async Task LogOutWebRequest()
    {
        if (LobbyConnect.Instance._lobbySocket.IsAlive)
        {
            WebSocket ws = LobbyConnect.Instance._lobbySocket;
            ws.CloseAsync(CloseStatusCode.Normal);
            Task delayTask = Task.Delay(TimeSpan.FromSeconds(10));

            Task completedTask = await Task.WhenAny(delayTask);

            if (completedTask == null)
            {
                Debug.Log("closing...");
                Debug.Log(completedTask);
                LobbyWindowController.Instance.ActiveAlertWindow(LobbyAlertMessage.Logout);
            }

            else if (completedTask == delayTask)
            {
                Debug.Log("retry...");
                ws.CloseAsync(CloseStatusCode.Normal);
            }
        }
        else
        {
            Debug.Log("WebSocket is not alive");
        }

        //if (await Auth.AuthToken()) 
        //{
        //    Debug.Log("로그아웃 이전 : " + Crypto.AESDecrypt128(CryptoType.RefreshToken));

        //    _httpClient = new HttpClient();
        //    _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Refresh-Token", Crypto.AESDecrypt128(CryptoType.RefreshToken));
        //    _response = await _httpClient.GetAsync(_logoutUrl);

        //    Debug.Log("resposne : " + _response);
        //    Debug.Log("resposne Content: " + _response.Content);


        //    FileIO.ResetKey();

        //}
        //else 
        //{
        //    LobbyWindowController.Instance.ActiveAlertWindow(LobbyAlertMessage.FailAuth);
        //    FileIO.ResetKey();
        //}


    }
}
