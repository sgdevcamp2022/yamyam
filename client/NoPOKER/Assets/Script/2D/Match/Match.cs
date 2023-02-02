using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match : MonoBehaviour
{
    [SerializeField] private List<GameObject> _loadingObject = new List<GameObject>();
    private IEnumerator _loadingCoroutine;


    private void Start()
    {
        Init();
    }

    public void Init()
    {
        _loadingCoroutine = _loadingUI();
    }

    public void MatchLoading()
    {
        StartCoroutine(_loadingCoroutine);
        Debug.Log("next");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            MatchingSucceed();
        }

    }

    private IEnumerator _loadingUI()
    {

        while(true)
        {
          
              
            Debug.Log("...");
            for(int i=0;i<_loadingObject.Count;i++)
            {
                Debug.Log(i);
                _loadingObject[i].SetActive(false);
                yield return new WaitForSeconds(0.5f);
                _loadingObject[i].SetActive(true);
            }               
        }
  
    }

    public void StopLoading()
    {
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
