using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match : MonoBehaviour
{
    [SerializeField] private List<GameObject> _loadingObject = new List<GameObject>();
    private IEnumerator _loadingCoroutine;
    private IEnumerator _loadingUICoroutine;

    public void MatchLoading()
    {
        for (int i = 0; i < _loadingObject.Count; i++)
        {
            _loadingObject[i].SetActive(true);
        }
        _loadingCoroutine = Loading();
        _loadingUICoroutine = LoadingUI();

        StartCoroutine(_loadingCoroutine);
    }

    public IEnumerator LoadingUI()
    {
        while(true)
        {           
            for(int i=0;i<_loadingObject.Count;i++)
            {
                _loadingObject[i].SetActive(false);
                yield return new WaitForSeconds(0.5f);
                _loadingObject[i].SetActive(true);
            }               
        }
    }

    public IEnumerator Loading()
    {
        StartCoroutine(_loadingUICoroutine);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.A)); //���߿� AŰ �Է´�� �����������
        MatchingSucceed();
    }

    public void StopLoading()
    {
        StopCoroutine(_loadingUICoroutine);
        StopCoroutine(_loadingCoroutine);  
    }

    public void MatchingExit()
    {
        StopLoading();
        LobbyWindowController.Instance.ActiveAlertWindow();
    }

    public void MatchingSucceed()
    {
        StopLoading();
        LobbyWindowController.Instance.InActiveMatchingWindow();
        LobbyWindowController.Instance.ActiveSucceedMatchWindow();
    }

    //���������� ������ ������.
    /*
      public void ClickedAcceptMatching()
    {

    }
     */

    public void ClickedRejectMatching()
    {
        StopLoading();
        LobbyWindowController.Instance.InActiveMatchingWindow();
        LobbyWindowController.Instance.InActiveSucceedMatchWindow();
        //������� : ���� Ƚ�� ���� �� �г�Ƽ�ο�
    }
}
