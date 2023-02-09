using UnityEngine;
using TMPro;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;

public class ResetPWData
{
    public string username;
    public string email;
}

public class ResetPW : MonoBehaviour
{
    [SerializeField] private  TMP_InputField _inputID;
    [SerializeField] private  TMP_InputField _inputEmail;

    private string _blank = "";


    private void OnEnable()
    {
        InitSetting();
    }

    public void InitSetting()
    {
        _inputID.text = _blank;
        _inputEmail.text = _blank;
    }

    public void FindUserPW()
    {
        if (_inputID.text.Equals(_blank) || _inputEmail.text.Equals(_blank))
        {
            WindowController.Instance.SendAlertMessage(AlertMessage.Blank);
            return;
        }
        if (!IsValidEmail(_inputEmail.text))
        {
            WindowController.Instance.SendAlertMessage(AlertMessage.IncorrectEmail);
            return;
        }
         ResetPwWebRequest();
    }
     async Task ResetPwWebRequest()
    {
        ResetPWData _data = new ResetPWData();
        _data.email = _inputEmail.text;
        _data.username = _inputID.text;
        HttpClient _httpClient = new HttpClient();
        HttpContent _httpContent = new StringContent(JsonUtility.ToJson(_data), Encoding.UTF8, "application/json");
        string _url = "http://127.0.0.1:8000/accounts/password_reset";
        using HttpResponseMessage _response = await _httpClient.PostAsync(_url, _httpContent);

        switch((int)_response.StatusCode)
        {
            case 200:
                SucceedFindPWWebRequest();
                break;
            case 404:
                WindowController.Instance.SendAlertMessage(AlertMessage.NotFound);
                break;
        }
    }

    public void SucceedFindPWWebRequest()
    {
        gameObject.SetActive(false);
        WindowController.Instance.SendAlertMessage(AlertMessage.FindPW);
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
