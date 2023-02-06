using UnityEngine;
using TMPro;

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

        _isCorrect = true;
    }

    public void RequestSignup()
    {
        CheckInfo();
  
        if (_isCorrect)
        {
            //서버통신 : 해당 사용자에게 인증메일 보내라는 신호보냄
            WindowController.Instance.SendAlertMessage(AlertMessage.EmailLink);
            gameObject.SetActive(false);
        }
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
    }
}
