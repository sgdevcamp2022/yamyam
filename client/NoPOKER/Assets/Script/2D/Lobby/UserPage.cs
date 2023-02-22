using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using System.Text;

public class JsonUserPageData {
    public string nickname;
    int victory;
    int loose;
    public string date_joined;

    public string GetNickName()
    {
        return nickname;
    }
    public string GetJoinDate()
    {
        return date_joined;
    }
    public int GetVictory()
    {
        return victory;
    }
    public int GetLoose()
    {
        return loose;
    }
}

public class UserPage : MonoBehaviour
{
    static UserPage s_instance = null;
    public static UserPage Instance { get => s_instance; }

    [SerializeField] UIUserPage _uiUserPage;
    JsonUserPageData _pageData;
    HttpClient _httpClient;
    HttpContent _httpContent;
    HttpResponseMessage _response;
    JsonUserPageData _userPageData;
    StringBuilder _userPageUrl = new StringBuilder();
    UserType _pageType;

    private void Awake()
    {
        Init();
    }
    public void Init()
    {
        if (s_instance == null)
            s_instance = this;
    }


    public async Task UserPageWebRequest(int id)
    {
        if(await Auth.AuthToken()) //Token 인증성공!
        {

            _response = null;
            _pageData = new JsonUserPageData();

            _httpClient = new HttpClient();
            _userPageUrl.Clear();
            _userPageUrl.Append("http://127.0.0.1:8000/accounts/");
            _userPageUrl.Append(id);
            _userPageUrl.Append("/");
            _response = await _httpClient.GetAsync(_userPageUrl.ToString());
            _userPageData = JsonUtility.FromJson<JsonUserPageData>(_response.Content.ReadAsStringAsync().Result);

            StartCoroutine(SettingUserPageUI());
        }
        else //인증실패!
        {
            FileIO.ResetKey();
            LobbyWindowController.Instance.ActiveAlertWindow(LobbyAlertMessage.FailAuth);
        }

 
    }

    IEnumerator SettingUserPageUI()
    {
        yield return new WaitUntil(() => _response!=null);
        _uiUserPage.SetUserPage(_userPageData);
       LobbyWindowController.Instance.ActiveMyPageWindow();
            
    }
}
