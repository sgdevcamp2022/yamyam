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
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.A)); //나중에 A키 입력대신 서버통신으로
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

    //수락누르면 다음씬 가도록.
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
        //서버통신 : 거절 횟수 적립 및 패널티부여
    }
}
