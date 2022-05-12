using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource musicSource;
    public AudioClip currentSong;

    private const string MUSIC_PATH = "Audio/Music/";
    private bool isLowVolume = false;

    private static MusicManager instance;
    public static MusicManager Instance { get
        {
            if (instance == null) instance = GameObject.FindObjectOfType<MusicManager>();
            return instance;
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        musicSource = GetComponent<AudioSource>();
        musicSource.clip = currentSong;
        musicSource.Play();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            AudioListener.volume += .05f;
        }

        if (Input.GetKeyDown(KeyCode.Minus))
        {
            AudioListener.volume -= .05f;
        }
    }

    public void PlayFloorSong()
    {
        PlaySong("Floor" + Floor.Instance.floorLevel);
    }

    public void PlaySong(string songName)
    {
        AudioClip newSong = Resources.Load<AudioClip>(MUSIC_PATH + songName);
        if (currentSong == newSong) return;

        currentSong = newSong;
        musicSource.clip = currentSong;
        musicSource.Play();
    }
}
