using UnityEngine;
using TMPro;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json.Linq;


public class JsonLoginData
{
    public string username;
    public string password;
}

public class JsonUserData 
{
    public int id;
    public string nickname;
}

public class Login : MonoBehaviour
{
    [SerializeField] private TMP_InputField _id;
    [SerializeField] private TMP_InputField _pw;

    string _AccessToken = "";
    string _RefreshToken = "";

    HttpClient _httpClient;
    HttpContent _httpContent;
    JsonLoginData _loginData;
    HttpResponseMessage _response;
    JsonUserData _userJson;

    string _loginUrl = "http://127.0.0.1:8000/accounts/login/";

    public void RequestLogin()
    {
        if (_id.text.Equals("") || _pw.text.Equals(""))
        {
            WindowController.Instance.SendAlertMessage(LoginAlertMessage.Blank);
            return;
        }
        LoginWebRequest();
    }

    

    async Task LoginWebRequest()
    {
        _loginData = new JsonLoginData();
        _loginData.username = _id.text;
        _loginData.password = _pw.text;
        _httpClient = new HttpClient();
        _httpContent = new StringContent(JsonUtility.ToJson(_loginData), Encoding.UTF8, "application/json");
        
        _response = await _httpClient.PostAsync(_loginUrl, _httpContent);


      

        switch ((int)_response.StatusCode)
        {
            case 200:
                _userJson = JsonUtility.FromJson<JsonUserData>(_response.Content.ReadAsStringAsync().Result);
                SucceedLoginWebRequest();
                break;
            case 404:
                WindowController.Instance.SendAlertMessage(LoginAlertMessage.NotFound);
                FileIO.ResetKey();
                break;
        }
    }


    
    public void SucceedLoginWebRequest()
    {
        foreach (var i in _response.Headers.GetValues("Access-Token"))
        {
            _AccessToken = i;
        }
        foreach (var i in _response.Headers.GetValues("Refresh-Token"))
        {
            _RefreshToken = i;
        }

        Debug.Log("refresh-token = " + _RefreshToken);


        //_token은 서버로부터 받아온값을 바로 넣어주는걸로.
        _AccessToken = Crypto.AESEncrypt128(_AccessToken, CryptoType.AccessToken);
        _RefreshToken = Crypto.AESEncrypt128(_RefreshToken, CryptoType.RefreshToken);

        UserInfo.Instance.SetUserInfo(_userJson.id, _userJson.nickname);

        /*복호화테스트용.
        _AccessToken = Crypto.AESDecrypt128(CryptoType.AccessToken);
        _RefreshToken = Crypto.AESDecrypt128(CryptoType.RefreshToken);
        Debug.Log("AES 복호화 : " + _AccessToken);
        Debug.Log("AES 복호화 : " + _RefreshToken);*/
        GameManager.Instance.ChangeScene(Scenes.LobbyScene);
    }
}
