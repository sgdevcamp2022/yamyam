using UnityEngine;

public class WindowController : MonoBehaviour
{
    private static WindowController s_instance = null;
    public static WindowController Instance
    { get => s_instance; }

    [SerializeField] private GameObject _signupWindow;
    [SerializeField] private GameObject _findWindow;
    [SerializeField] private GameObject _alertWindow;
    [SerializeField] private SignUp _signUp;
    [SerializeField] private Alert _alert;
    [SerializeField] private Find _find;

    private void Awake()
    {
        _init();
    }

    private void _init()
    {
        if (s_instance == null)
            s_instance = this;
    }

    public void ActiveSignUpWindow()
    {
        _signUp.InitSetting();
        _signupWindow.SetActive(true);
    }

    public void InActiveSignUpWindow()
    {
        _signupWindow.SetActive(false);
    }

    public void ActiveFindIdWindow()
    {
        _find.InitSetting(FindNum.ID);
        _findWindow.SetActive(true);
    }

    public void ActiveFindPwWindow()
    {
        _find.InitSetting(FindNum.PW);
        _findWindow.SetActive(true);
    }

    public void InActiveFindWindow()
    {
        _findWindow.SetActive(false);
    }

    public void InActiveAlertWindow()
    {
        _alertWindow.SetActive(false);
    }

    public void SendAlertMessage(AlertMessage alert)
    {
        _alert.SetAlertContent(alert);
        _alertWindow.SetActive(true);
    }
}
