using UnityEngine;
using TMPro;
public enum AlertMessage
{
    Blank,
    IncorrectPW,
    NotFound,
    EmailLink,
    FindID,
    FindPW,
    IncorrectEmail,
    Duplicate
}

public class Alert : MonoBehaviour
{
    [SerializeField] private TMP_Text _alertTitle;
    [SerializeField] private TMP_Text _alertContent;

    private string[] _alertMessage = {"빈칸이 없는지 다시 확인해주세요", "비밀번호가 서로 일치하지않습니다.", "찾을 수 없는 사용자입니다."
                                        ,"이메일로 인증링크를 보냈습니다.\n확인해주세요", "이메일로 아이디를 보냈습니다.\n확인해주세요",
                                        "이메일로 재설정 링크를 보냈습니다.\n30분내로 확인해주세요", "이메일 형식을 다시 확인해주세요",
                                        "아이디 혹은 닉네임이 중복됩니다."};

    public void SetAlertContent(AlertMessage message)
    {
        _alertContent.text = _alertMessage[(int)message];
    }
}
