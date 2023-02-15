using UnityEngine;
using TMPro;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Collections;

public class LoginData
{
    public string username;
    public string password;
}

public class Login : MonoBehaviour
{
    [SerializeField] private TMP_InputField _id;
    [SerializeField] private TMP_InputField _pw;

     public string _AccessToken = "";
    public string _RefreshToken = "";

    public void RequestLogin()
    {
        if (_id.text.Equals("") || _pw.text.Equals(""))
        {
            WindowController.Instance.SendAlertMessage(LoginAlertMessage.Blank);
            return;
        }
        ResetPwWebRequest();
    }

    async Task ResetPwWebRequest()
    {
        LoginData _data = new LoginData();
        _data.username = _id.text;
        _data.password = _pw.text;
        HttpClient _httpClient = new HttpClient();
        HttpContent _httpContent = new StringContent(JsonUtility.ToJson(_data), Encoding.UTF8, "application/json");
        string _url = "http://127.0.0.1:8000/accounts/login/";
         HttpResponseMessage _response = await _httpClient.PostAsync(_url, _httpContent);

        //Debug.Log("ResponseHeader : " + _response.Headers);

        //IEnumerator enumerator = _response.Headers.GetEnumerator();

        //Debug.Log("AccessToken : " + _response.Headers.GetValues("Access-Token").ToString());
        //Test용
        string AccessToken = "";
        string RefreshToken = "";

        foreach( var i in _response.Headers.GetValues("Access-Token"))
        {
            _AccessToken = i;
        }
        foreach (var i in _response.Headers.GetValues("Refresh-Token"))
        {
            _RefreshToken = i;
        }
        Debug.Log("response RequestMessage : " + _response.RequestMessage);
        Debug.Log("response Result : " + _response.Content.ReadAsStringAsync().Result);
   
        switch ((int)_response.StatusCode)
        {
            case 200:
                SucceedLoginWebRequest();
                break;
            case 404:
                WindowController.Instance.SendAlertMessage(LoginAlertMessage.NotFound);
                break;
        }
    }

    public void SucceedLoginWebRequest()
    {

        //_token은 서버로부터 받아온값을 바로 넣어주는걸로.
        _AccessToken = Crypto.AESEncrypt128(_AccessToken, CryptoType.AccessToken);
        _RefreshToken = Crypto.AESEncrypt128(_RefreshToken, CryptoType.RefreshToken);

        /*복호화테스트용.
        _AccessToken = Crypto.AESDecrypt128(CryptoType.AccessToken);
        _RefreshToken = Crypto.AESDecrypt128(CryptoType.RefreshToken);
        Debug.Log("AES 복호화 : " + _AccessToken);
        Debug.Log("AES 복호화 : " + _RefreshToken);*/
        GameManager.Instance.ChangeScene(Scenes.LobbyScene);
    }
}
