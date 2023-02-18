using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;
public enum UserType
    {
    Player,
    OtherUser
    }

public class UIUserPage : MonoBehaviour
{

    [SerializeField] TMP_Text _title;
    [SerializeField] TMP_Text _userName;
    [SerializeField] TMP_Text _score;
    [SerializeField] TMP_Text _scorePercent;
    [SerializeField] TMP_Text _joinDate;
    [SerializeField] Button _myPageButton;

    StringBuilder _scoreSb = new StringBuilder();
    StringBuilder _scorePercentSb = new StringBuilder();
    

    private void Start()
    {
        _myPageButton.onClick.AddListener(() => UserPage.Instance.UserPageWebRequest(UserInfo.Instance.UserID));
    }

    string _myTitle = "마이페이지";
    string _userTitle = "유저페이지";

    public void SetUserPageTitle(UserType userType)
    {
        switch(userType)
        {
            case UserType.Player:
                _title.text = _myTitle;
                break;
            case UserType.OtherUser:
                _title.text =_userTitle;
                break;
        }
    }

    public void SetUserPage(JsonUserPageData data)
    {
       

        _userName.text = data.GetNickName();
        _scoreSb.Clear();
        _scoreSb.Append(data.GetVictory().ToString());
        _scoreSb.Append("승 / ");
        _scoreSb.Append(data.GetLoose().ToString());
        _scoreSb.Append("패");
        _score.text = _scoreSb.ToString();

        int _scoreResult;
        if (data.GetVictory() != 0)
             _scoreResult = (data.GetLoose() + data.GetVictory()) % data.GetVictory();
        else
             _scoreResult = 0;


        _scoreResult *= 100;

        _scorePercentSb.Clear();
        _scorePercentSb.Append(_scoreResult);
        _scorePercentSb.Append("%");
        _scorePercent.text = _scorePercentSb.ToString();

        _joinDate.text = data.GetJoinDate().Substring(0, 10);

    }

}
