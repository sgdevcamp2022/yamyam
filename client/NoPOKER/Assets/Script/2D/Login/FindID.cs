using UnityEngine;
using TMPro;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
public class FindIdData
{
    public string email;
}
public class FindID : MonoBehaviour
{
    [SerializeField] private TMP_InputField _email;

    private string _blank = "";

    private void OnEnable()
    {
        InitSetting();
    }

    public void InitSetting()
    {
        _email.text = _blank;
    }

    public void FindUserID()
    {
        if (_email.text.Equals(_blank))
        {
            WindowController.Instance.SendAlertMessage(LoginAlertMessage.Blank);
            return;
        }

        if (!IsValidEmail(_email.text))
        {
            WindowController.Instance.SendAlertMessage(LoginAlertMessage.IncorrectEmail);
            return;
        }
        ResetPwWebRequest();
    }

    async Task ResetPwWebRequest()
    {
        FindIdData _data = new FindIdData();
        _data.email = _email.text;
        HttpClient _httpClient = new HttpClient();
        HttpContent _httpContent = new StringContent(JsonUtility.ToJson(_data), Encoding.UTF8, "application/json");
        string _url = "http://127.0.0.1:8000/accounts/find_username/";
        using HttpResponseMessage _response = await _httpClient.PostAsync(_url, _httpContent);

        switch ((int)_response.StatusCode)
        {
            case 200:
                SucceedFindIDWebRequest();
                break;
            case 404:
                WindowController.Instance.SendAlertMessage(LoginAlertMessage.NotFound);
                break;
        }
    }

    public void SucceedFindIDWebRequest()
    {
        gameObject.SetActive(false);
        WindowController.Instance.SendAlertMessage(LoginAlertMessage.FindID);
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
