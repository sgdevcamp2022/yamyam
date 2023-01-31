using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

public enum FindNum
{
    ID,
    PW
}

public class Find : MonoBehaviour
{
    [SerializeField] private TMP_InputField _input;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _viewText;
    [SerializeField] private TMP_Text _content;
    [SerializeField] private GameObject _nextButton;
    [SerializeField] private GameObject _closeButton;

    private string[] _titles = {"아이디 찾기","비밀번호 찾기"};
    private string[] _viewTexts = { "이메일", "아이디" };

    private string[] _correctContent = { "이메일로 아이디를 보냈습니다.\n확인해주세요", "이메일로 재설정 링크를 보냈습니다.\n확인해주세요" };
    private string _blank = "";
    private bool _isCorrect;
    private FindNum _findInstance;

    public void InitSetting(FindNum find)
    {
        _isCorrect = false;
        _findInstance = find;

        _title.text = _titles[(int)find];
        _viewText.text = _viewTexts[(int)find];      
        _input.text = _blank;

        _nextButton.SetActive(true);
        _input.gameObject.SetActive(true);
        _viewText.gameObject.SetActive(true);
        _content.gameObject.SetActive(false);
        _closeButton.SetActive(false);
    }

    public void FindUserInfo()
    {
        if (_input.text.Equals(_blank))
        {
            WindowController.Instance.SendAlertMessage(AlertMessage.Blank);
            return;
        }
        //서버통신 : DB확인 및 결과값 수신
        if (_isCorrect)
        {
            _content.text = _correctContent[(int)_findInstance];

            _input.gameObject.SetActive(false);
            _viewText.gameObject.SetActive(false);
            _content.gameObject.SetActive(true);

            _closeButton.SetActive(true);
            _nextButton.SetActive(false);
        }
    }
}
