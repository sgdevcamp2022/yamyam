using UnityEngine;
using TMPro;
using System.Text;

public class FindID : MonoBehaviour
{
    [SerializeField] private TMP_InputField _input;

    private string _blank = "";
    private bool _isCorrect;

    private void OnEnable()
    {
        InitSetting();
    }

    public void InitSetting()
    {
        _isCorrect = false;
        _input.text = _blank;
    }

    public void FindUserID()
    {
        if (_input.text.Equals(_blank))
        {
            WindowController.Instance.SendAlertMessage(AlertMessage.Blank);
            return;
        }
        //������� : DBȮ�� �� ����� ����
        if (_isCorrect)
        {
            gameObject.SetActive(false);
            WindowController.Instance.SendAlertMessage(AlertMessage.FindID);
        }
    }
}
