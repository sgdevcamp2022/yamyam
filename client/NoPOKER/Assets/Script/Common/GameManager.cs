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
    }

    public Scenes CheckNowScene()
    {
        return (Scenes)SceneManager.GetActiveScene().buildIndex;
    }
}
