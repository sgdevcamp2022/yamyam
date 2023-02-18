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
    HttpResponseMessage _response;
    string _destroyUrl = "http://127.0.0.1:8000/accounts/";
    StringBuilder _destroyUrlBuilder = new StringBuilder();

    void Start()
    {
        _destroyButton.onClick.AddListener(() => LobbyWindowController.Instance.InActiveDestroyUserWindow());
        _destroyButton.onClick.AddListener(() => Logout.LogOutWebRequest());
        _destroyButton.onClick.AddListener(() => DestroyUserWebRequest());
    }


    public async Task DestroyUserWebRequest()
    {

    _httpClient = new HttpClient();
        // _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Refresh-Token", Crypto.AESDecrypt128(CryptoType.RefreshToken));

        _destroyUrlBuilder.Clear();
        _destroyUrlBuilder.Append(_destroyUrl);
        _destroyUrlBuilder.Append(UserInfo.Instance.UserID);

        _response = await _httpClient.DeleteAsync(_destroyUrlBuilder.ToString());

        Debug.Log("resposne : " + _response);
        Debug.Log("resposne Content: " + _response.Content);


    }
}
