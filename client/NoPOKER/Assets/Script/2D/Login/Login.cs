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



    private bool _isCorrect= true;
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
        LoginData data = new LoginData();
        data.username = _id.text;
        data.password = _pw.text;
        HttpClient httpClient = new HttpClient();
        HttpContent httpContent = new StringContent(JsonUtility.ToJson(data), Encoding.UTF8, "application/json");
        string url = "http://127.0.0.1:8000/accounts/login/";
        using HttpResponseMessage response = await httpClient.PostAsync(url, httpContent);
        switch ((int)response.StatusCode)
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
        Debug.Log("����: " + _token);
        //_token�� �����κ��� �޾ƿ°��� �ٷ� �־��ִ°ɷ�.
        _token = Crypto.AESEncrypt128(_token);
        Debug.Log("AES ��ȣȭ : " + _token);

        _token = Crypto.AESDecrypt128();
        Debug.Log("AES ��ȣȭ : " + _token);
        GameManager.Instance.ChangeScene(Scenes.LobbyScene);
    }
}
