using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    private static Sound s_instance = null;
    public static Sound Instance { get => s_instance; }

    [SerializeField] AudioClip _cardSound;
    [SerializeField] AudioClip _chipSound;
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

    public void PlayCardSound()
    {
        _soundPlayer.clip = _cardSound;
        _soundPlayer.Play();
    }
    public void PlayBattinSound()
    {
        _soundPlayer.clip = _chipSound;
        _soundPlayer.Play();
    }

}
