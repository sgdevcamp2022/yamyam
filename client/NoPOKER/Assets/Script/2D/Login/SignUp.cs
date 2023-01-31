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
            //������� : �ش� ����ڿ��� �������� ������� ��ȣ����
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
        //���̵� �ߺ� Ȯ��
        if (!(_pw.text.Equals(_checkPw.text)))
        {
            WindowController.Instance.SendAlertMessage(AlertMessage.IncorrectPW);
            return;
        }
        //������� : �г��� �ߺ� Ȯ��
        //������� : �̸��� �ߺ� Ȯ��
    }
}
