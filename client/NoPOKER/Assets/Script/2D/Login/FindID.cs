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
            WindowController.Instance.SendAlertMessage(AlertMessage.Blank);
            return;
        }

        if (!IsValidEmail(_email.text))
        {
            WindowController.Instance.SendAlertMessage(AlertMessage.IncorrectEmail);
            return;
        }

        ResetPwWebRequest();
    }

    async Task ResetPwWebRequest()
    {
        FindIdData data = new FindIdData();
        data.email = _email.text;
        HttpClient httpClient = new HttpClient();
        HttpContent httpContent = new StringContent(JsonUtility.ToJson(data), Encoding.UTF8, "application/json");
        string url = "http://127.0.0.1:8000/accounts/find_username/";
        using HttpResponseMessage response = await httpClient.PostAsync(url, httpContent);
        
        switch ((int)response.StatusCode)
        {
            case 200:
                SucceedFindIDWebRequest();
                break;
            case 404:
                WindowController.Instance.SendAlertMessage(AlertMessage.NotFound);
                break;
        }
    }

    public void SucceedFindIDWebRequest()
    {
        gameObject.SetActive(false);
        WindowController.Instance.SendAlertMessage(AlertMessage.FindID);
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
