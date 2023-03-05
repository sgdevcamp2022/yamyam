using UnityEngine;

public class PersonSound : MonoBehaviour
{
    private static PersonSound s_instance = null;
    public static PersonSound Instance { get => s_instance; }
    [SerializeField] AudioClip _raiseSound;
    [SerializeField] AudioClip _callSound;
    [SerializeField] AudioClip _dieSound;
    [SerializeField] AudioSource _soundPlayer;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        if (s_instance == null)
            s_instance = this;
    }

    public void PlayRaiseSound()
    {
        _soundPlayer.clip = _raiseSound;
        _soundPlayer.Play();
    }

    public void PlayCallSound()
    {
        _soundPlayer.clip = _callSound;
        _soundPlayer.Play();
    }

    public void PlayDieSound()
    {
        _soundPlayer.clip = _dieSound;
        _soundPlayer.Play();
    }
}
