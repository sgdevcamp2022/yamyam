using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIEmoticon : MonoBehaviour
{
    [SerializeField] private Button[] _emoticonButtons;
    [SerializeField] private Sprite[] _emoticonImages;
 
    [SerializeField] private GameObject[] _playerEmoticonObjects;
    [SerializeField] private Image[] _playerEmoticonImages;
    private IEnumerator[] _playersEmoticonView = new IEnumerator[4];
    private void Start()
    {
        _init();
    }

    private void _init()
    {
        _buttonSetting();
    }

    private void _buttonSetting()
    {

        //_emoticonButtons[(int)EmoticonType.Smile].onClick.AddListener : 서버에게 이모티콘 사용했다고 송신하기.
        _emoticonButtons[(int)EmoticonType.Smile].onClick.AddListener(() => ShowEmoticon(0, EmoticonType.Smile));

        //_emoticonButtons[(int)EmoticonType.Smile].onClick.AddListener : 서버에게 이모티콘 사용했다고 송신하기.
        _emoticonButtons[(int)EmoticonType.Sad].onClick.AddListener(() => ShowEmoticon(0, EmoticonType.Sad));

        //_emoticonButtons[(int)EmoticonType.Smile].onClick.AddListener : 서버에게 이모티콘 사용했다고 송신하기.
        _emoticonButtons[(int)EmoticonType.Tear].onClick.AddListener(() => ShowEmoticon(0, EmoticonType.Tear));

        //_emoticonButtons[(int)EmoticonType.Smile].onClick.AddListener : 서버에게 이모티콘 사용했다고 송신하기.
        _emoticonButtons[(int)EmoticonType.Angry].onClick.AddListener(() => ShowEmoticon(0, EmoticonType.Angry));

        //_emoticonButtons[(int)EmoticonType.Smile].onClick.AddListener : 서버에게 이모티콘 사용했다고 송신하기.
        _emoticonButtons[(int)EmoticonType.Surprise].onClick.AddListener(() => ShowEmoticon(0, EmoticonType.Surprise));

    }

    public void ShowEmoticon(int who, EmoticonType type)
    {
        //이전 이모티콘이 계속 보여지고 있는중이라면, 취소하고 새로운 이모티콘 보여지게 하기
        if (_playersEmoticonView[who] != null)
        {
            StopCoroutine(_playersEmoticonView[who]);        
        }
        _playerEmoticonImages[who].sprite = _emoticonImages[(int)type];
        _playersEmoticonView[who] = Showing(who);
        StartCoroutine(_playersEmoticonView[who]);
    }
    IEnumerator Showing(int who)
    {
        _playerEmoticonObjects[who].SetActive(true);
        yield return new WaitForSeconds(4f);

        _playerEmoticonObjects[who].SetActive(false);
        _playersEmoticonView[who] = null;
    }
}
