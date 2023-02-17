using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class DestroyUser : MonoBehaviour {

    [SerializeField] Button _destroyButton;
    HttpClient _httpClient;
    HttpContent _httpContent;
    HttpResponseMessage _response;
    string _logoutUrl = "http://127.0.0.1:8000/accounts/";
    StringBuilder _userPageUrl = new StringBuilder();

    void Start()
    {
       // _logoutButton.onClick.AddListener(() => LobbyWindowController.Instance.ActiveAlertWindow(LobbyAlertMessage.Logout));
       // _logoutButton.onClick.AddListener(() => LobbyWindowController.Instance.InActiveSettingWindow());
       // _logoutButton.onClick.AddListener(() => LogOutWebRequest());
    }


    public async Task DestroyUserWebRequest()
    {





    _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Refresh-Token", Crypto.AESDecrypt128(CryptoType.RefreshToken));
        _response = await _httpClient.GetAsync(_logoutUrl);

        Debug.Log("resposne : " + _response);
        Debug.Log("resposne Content: " + _response.Content);


        FileIO.ResetKey();
        LobbyWindowController.Instance.ActiveAlertWindow(LobbyAlertMessage.Logout);

    }
}
