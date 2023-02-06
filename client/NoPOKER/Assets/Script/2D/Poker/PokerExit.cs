using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PokerExit : MonoBehaviour
{
    [SerializeField] GameObject _exitAlertWindow;
    [SerializeField] Button _exitButton;

    private void Start()
    {
        _exitButton.onClick.AddListener(()=> ActiveAlertWindow());
        _exitButton.onClick.AddListener(() => Exit());
    }

    public void ActiveAlertWindow()
    {
        _exitAlertWindow.SetActive(true);
    }
    public void InActiveAlertWindow()
    {
        _exitAlertWindow.SetActive(false);
    }

    public void Exit()
    {
        //�ʱ��ڱ� Ȯ���ϰ�,
        //�ڱ��� �ִ»��¿��� �����ٸ� �й�1 ����
        //�ڱ��� ���� ���¿��� �����ٸ� �׳� ������. => �ٵ� ������ �й��ڳ�??
        //�κ�ȭ������ �̵�
       
    }

}
