using UnityEngine;
using TMPro;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;

public class LoginData
{
    public string username;
    public string password;
}

public class Login : MonoBehaviour
{
    [SerializeField] private TMP_InputField _id;
    [SerializeField] private TMP_InputField _pw;

     public string _token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJ2ZWxvcGVydC5jb20iLCJleHAiOiIxNDg1MjcwMDAwMDAwIiwiaHR0cHM6Ly92ZWxvcGVydC5jb20vand0X2NsYWltcy9pc19hZG1pbiI6dHJ1ZSwidXNlcklkIjoiMTEwMjgzNzM3MjcxMDIiLCJ1c2VybmFtZSI6InZlbG9wZXJ0In0.WE5fMufM0NDSVGJ8cAolXGkyB5RmYwCto1pQwDIqo2w";


    public void RequestLogin()
    {
        if (_id.text.Equals("") || _pw.text.Equals(""))
        {
            WindowController.Instance.SendAlertMessage(AlertMessage.Blank);
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
        using HttpResponseMessage _response = await _httpClient.PostAsync(_url, _httpContent);

        switch ((int)_response.StatusCode)
        {
            case 200:
                SucceedLoginWebRequest();
                break;
            case 404:
                WindowController.Instance.SendAlertMessage(AlertMessage.NotFound);
                break;
        }
    }

    public void SucceedLoginWebRequest()
    {
        Debug.Log("원본: " + _token);
        //_token은 서버로부터 받아온값을 바로 넣어주는걸로.
        _token = Crypto.AESEncrypt128(_token);
        Debug.Log("AES 암호화 : " + _token);

        _token = Crypto.AESDecrypt128();
        Debug.Log("AES 복호화 : " + _token);
        GameManager.Instance.ChangeScene(Scenes.LobbyScene);
    }
}
