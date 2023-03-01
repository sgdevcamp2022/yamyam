using UnityEngine;
using UnityEngine.SceneManagement;
public enum Scenes
{
    LoginScene,
    LobbyScene,
    PokerGameScene,
    ActionGameScene
}

public class GameManager : MonoBehaviour
{
    static private GameManager s_instance = null;
    public static GameManager Instance
    {
        get => s_instance;
    }
    Scenes _scenes = Scenes.LoginScene;

    private void Awake()
    {
        _init();
    }

    private void _init()
    {
        if(s_instance ==null)
        {
            s_instance = this;
        }
        DontDestroyOnLoad(this);
    }

    public void ChangeScene(Scenes sceneNum)
    {
        SceneManager.LoadScene((int)sceneNum);
        _scenes = sceneNum;
    }

    public Scenes CheckNowScene()
    {
        return _scenes;
    }

    public void SendGameStartMessage()
    {
        if (UserInfo.Instance.IsLeader)
        {
            Debug.Log("user ID : " + UserInfo.Instance.UserID);
            Debug.Log("user NickName : " + UserInfo.Instance.NickName);
            // Debug.Log("Leader ID : " + Team.Instance._teamData.leader.id);
            // Debug.Log("Leader NickName : " + Team.Instance._teamData.leader.nickname);
            Team.Instance._teamData.leader =  new UserSocketData(UserInfo.Instance.UserID, UserInfo.Instance.NickName);
            //Team.Instance._teamData.leader.id = UserInfo.Instance.UserID;
            //Team.Instance._teamData.leader.nickname = UserInfo.Instance.NickName;
            LobbyConnect.Instance.SendGameStartMessage(Team.Instance.GetTeamData);
        }
    }

    public void SendGameExitMessage()
    {
        if (UserInfo.Instance.IsLeader)
        {
            Team.Instance._teamData.leader.id = UserInfo.Instance.UserID;
            Team.Instance._teamData.leader.nickname = UserInfo.Instance.NickName;
            LobbyConnect.Instance.SendGameStartMessage(Team.Instance.GetTeamData);
        }
    }

    public void SendExitMessage()
    {
        GameExitRequestSocketData _exitRequestData = new GameExitRequestSocketData();
        _exitRequestData.player.id = UserInfo.Instance.UserID;
        _exitRequestData.player.nickname = UserInfo.Instance.NickName;

        LobbyConnect.Instance.SendGameExitMessage(_exitRequestData);       
    }
}
