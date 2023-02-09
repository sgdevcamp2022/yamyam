using UnityEngine;
using TMPro;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
public class SignUpData
{
    public string username;
    public string nickname;
    public string email;
    public string password;
}
public class SignUp : MonoBehaviour
{
    [SerializeField] private TMP_InputField _id;
    [SerializeField] private TMP_InputField _pw;
    [SerializeField] private TMP_InputField _checkPw;
    [SerializeField] private TMP_InputField _name;
    [SerializeField] private TMP_InputField _email;

    private bool _isCorrect;
    private string _blank = "";
    
    public void InitSetting()
    {
        _id.text = _blank;
        _pw.text = _blank;
        _checkPw.text = _blank;
        _name.text = _blank;
        _email.text = _blank;

        _isCorrect = false;
    }

    public void RequestSignup()
    {
        CheckInfo();
  

    }

    private void CheckInfo()
    {
        if (_id.text.Equals(_blank) || _pw.text.Equals(_blank) || _checkPw.text.Equals(_blank) || _name.text.Equals(_blank) || _email.text.Equals(_blank))
        {
            WindowController.Instance.SendAlertMessage(AlertMessage.Blank);
            return;
        }
        //아이디 중복 확인
        if (!(_pw.text.Equals(_checkPw.text)))
        {
            WindowController.Instance.SendAlertMessage(AlertMessage.IncorrectPW);
            return;
        }
        //서버통신 : 닉네임 중복 확인
        //서버통신 : 이메일 중복 확인
        if(!IsValidEmail(_email.text))
        {
            WindowController.Instance.SendAlertMessage(AlertMessage.IncorrectEmail);
            return;
        }
        //StartCoroutine(SignUpWebRequestPOST());
        ResetPwWebRequest();
    }
    async Task ResetPwWebRequest()
    {
        SignUpData data = new SignUpData();
        data.username = _id.text;
        data.nickname = _name.text;
        data.email = _email.text;
        data.password = _pw.text;
        HttpClient httpClient = new HttpClient();
        HttpContent httpContent = new StringContent(JsonUtility.ToJson(data), Encoding.UTF8, "application/json");
        string url = "http://127.0.0.1:8000/accounts/";
        using HttpResponseMessage response = await httpClient.PostAsync(url, httpContent);
        Debug.Log((int)response.StatusCode);
        Debug.Log(response);
        
        switch ((int)response.StatusCode)
        {
            case 201:
                SucceedSignUpWebRequest();
                break;
            case 400:
                WindowController.Instance.SendAlertMessage(AlertMessage.Duplicate);
                break;
        }
    }

    public void SucceedSignUpWebRequest()
    {
            WindowController.Instance.SendAlertMessage(AlertMessage.EmailLink);
            gameObject.SetActive(false);
    }

    public bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
