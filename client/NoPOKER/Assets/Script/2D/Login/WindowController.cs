using UnityEngine;

public class WindowController : MonoBehaviour
{
    private static WindowController s_instance = null;
    public static WindowController Instance
    { get => s_instance; }

    [SerializeField] private GameObject _signupWindow;
    [SerializeField] private GameObject _findIDWindow;
    [SerializeField] private GameObject _findPWWindow;
    [SerializeField] private GameObject _alertWindow;
    [SerializeField] private SignUp _signUp;
    [SerializeField] private Alert _alert;


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
        _findIDWindow.SetActive(true);
    }

    public void ActiveFindPwWindow()
    {
        _findPWWindow.SetActive(true);
    }

    public void InActiveFindIDWindow()
    {
        _findIDWindow.SetActive(false);
    }
    public void InActiveFindPWWindow()
    {
        _findPWWindow.SetActive(false);
    }

    public void InActiveAlertWindow()
    {
        _alertWindow.SetActive(false);
    }

    public void SendAlertMessage(LoginAlertMessage alert)
    {
        _alert.SetAlertContent(alert);
        _alertWindow.SetActive(true);
    }
}
