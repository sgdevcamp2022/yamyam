using UnityEngine;
using TMPro;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json.Linq;
public class SignUpData
{
    public string username;
    public string nickname;
    public string email;
    public string password;
}

public class CheckData
{
    public string username;
    public string nickname;
    public string email;
}

public class SignUp : MonoBehaviour
{
    [SerializeField] private TMP_InputField _id;
    [SerializeField] private TMP_InputField _pw;
    [SerializeField] private TMP_InputField _checkPw;
    [SerializeField] private TMP_InputField _name;
    [SerializeField] private TMP_InputField _email;

    private string _blank = "";
    
    public void InitSetting()
    {
        _id.text = _blank;
        _pw.text = _blank;
        _checkPw.text = _blank;
        _name.text = _blank;
        _email.text = _blank;
    }

    public void RequestSignup()
    {
        CheckInfo();
    }

    private void CheckInfo()
    {
        if (_id.text.Equals(_blank) || _pw.text.Equals(_blank) || _checkPw.text.Equals(_blank) || _name.text.Equals(_blank) || _email.text.Equals(_blank))
        {
            WindowController.Instance.SendAlertMessage(LoginAlertMessage.Blank);
            return;
        }
        //아이디 중복 확인
        if (!(_pw.text.Equals(_checkPw.text)))
        {
            WindowController.Instance.SendAlertMessage(LoginAlertMessage.IncorrectPW);
            return;
        }
        if(!IsValidEmail(_email.text))
        {
            WindowController.Instance.SendAlertMessage(LoginAlertMessage.IncorrectEmail);
            return;
        }

        ResetPwWebRequest();
    }
    async Task ResetPwWebRequest()
    {
        SignUpData data = new SignUpData();
        data.username = _id.text;
        data.nickname = _name.text;
        data.email = _email.text;
        data.password = _pw.text;

        HttpClient _httpClient = new HttpClient();
        HttpContent _httpContent = new StringContent(JsonUtility.ToJson(data), Encoding.UTF8, "application/json");
        string _url = "http://127.0.0.1:8000/accounts/";
        using HttpResponseMessage response = await _httpClient.PostAsync(_url, _httpContent);

       // Debug.Log("result = " + response.Content.ReadAsStringAsync().Result);
       // Debug.Log("response = " + response);
        switch ((int)response.StatusCode)
        {
            case 201:
                SucceedSignUpWebRequest();
                break;
            case 400:
                JObject _obj = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                if (_obj["username"] != null)
                {
                    WindowController.Instance.SendAlertMessage(LoginAlertMessage.DuplicateID);
                    return;
                }
                else if (_obj["nickname"] != null)
                {
                    WindowController.Instance.SendAlertMessage(LoginAlertMessage.DuplicateNickName);
                    return;
                }
                else if (_obj["email"] != null)
                {
                    WindowController.Instance.SendAlertMessage(LoginAlertMessage.DuplicateEmail);
                    return;
                }
                break;
        }
    }

    public void SucceedSignUpWebRequest()
    {
            WindowController.Instance.SendAlertMessage(LoginAlertMessage.EmailLink);
            gameObject.SetActive(false);
    }

    public bool IsValidEmail(string email)
    {
        try
        {
            var _addr = new System.Net.Mail.MailAddress(email);
            return _addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
