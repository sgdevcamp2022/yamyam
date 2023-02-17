using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Http.Headers;
public class Logout : MonoBehaviour
{
    [SerializeField] Button _logoutButton;
    HttpClient _httpClient;
    HttpContent _httpContent;
    HttpResponseMessage _response;
    string _logoutUrl = "http://127.0.0.1:8000/accounts/logout/";


    void Start()
    {
        _logoutButton.onClick.AddListener(() => LobbyWindowController.Instance.ActiveAlertWindow(LobbyAlertMessage.Logout));
        _logoutButton.onClick.AddListener(() => LobbyWindowController.Instance.InActiveSettingWindow());
        _logoutButton.onClick.AddListener(() => LogOutWebRequest());
    }


    public async Task LogOutWebRequest()
    {


        Debug.Log("로그아웃 이전 : " + Crypto.AESDecrypt128(CryptoType.RefreshToken));


        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Refresh-Token", Crypto.AESDecrypt128(CryptoType.RefreshToken));
        _response = await _httpClient.GetAsync(_logoutUrl);

        Debug.Log("resposne : " + _response);
        Debug.Log("resposne Content: " + _response.Content);


        FileIO.ResetKey();
        LobbyWindowController.Instance.ActiveAlertWindow(LobbyAlertMessage.Logout);

    }

}
