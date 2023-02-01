using UnityEngine;
using TMPro;
using System.Text;

public class FindPW : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputID;
    [SerializeField] private TMP_InputField _inputEmail;

    private string _blank = "";
    private bool _isCorrect;


    private void OnEnable()
    {
        InitSetting();
    }

    public void InitSetting()
    {
        _isCorrect = false;
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
        //서버통신 : ID정보 및 이메일정보 확인, 결과값 수신
        if (_isCorrect)
        {
            gameObject.SetActive(false);
            WindowController.Instance.SendAlertMessage(AlertMessage.FindPW);
        }
    }
}
