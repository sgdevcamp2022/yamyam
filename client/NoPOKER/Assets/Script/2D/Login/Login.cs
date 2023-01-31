using UnityEngine;
using TMPro;

public class Login : MonoBehaviour
{
    [SerializeField] private TMP_InputField _id;
    [SerializeField] private TMP_InputField _pw;

    private bool _isCorrect= true;
    public void RequestLogin()
    {
        if (_id.text.Equals("") || _pw.text.Equals(""))
        {
            WindowController.Instance.SendAlertMessage(AlertMessage.Blank);
            return;
        }
        //������� : DB�� �ش� ����� ���� Ȯ���ϱ�
        if (_isCorrect)
        {
            GameManager.Instance.ChangeScene(Scenes.LobbyScene);
        }
    }
  
}
