using UnityEngine;
using TMPro;
public enum AlertMessage
{
    Blank,
    IncorrectPW,
    NotFound,
    EmailLink,
    FindID,
    FindPW
}

public class Alert : MonoBehaviour
{
    [SerializeField] private TMP_Text _alertTitle;
    [SerializeField] private TMP_Text _alertContent;

    private string[] _alertMessage = {"��ĭ�� ������ �ٽ� Ȯ�����ּ���", "��й�ȣ�� ���� ��ġ�����ʽ��ϴ�.", "ã�� �� ���� ������Դϴ�."
                                        ,"�̸��Ϸ� ������ũ�� ���½��ϴ�.\nȮ�����ּ���", "�̸��Ϸ� ���̵� ���½��ϴ�.\nȮ�����ּ���",
                                        "�̸��Ϸ� �缳�� ��ũ�� ���½��ϴ�.\n30�г��� Ȯ�����ּ���"};

    public void SetAlertContent(AlertMessage message)
    {
        _alertContent.text = _alertMessage[(int)message];
    }
}
