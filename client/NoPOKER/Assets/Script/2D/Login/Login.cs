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
        //서버통신 : DB에 해당 사용자 존재 확인하기
        if (_isCorrect)
        {
            GameManager.Instance.ChangeScene(Scenes.LobbyScene);
        }
    }
  
}
