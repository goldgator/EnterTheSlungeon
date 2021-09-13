using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource musicSource;
    public AudioClip currentSong;

    private const string MUSIC_PATH = "Audio/Music/";

    public static MusicManager Instance { get; set; }
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        musicSource = GetComponent<AudioSource>();
        musicSource.clip = currentSong;
        musicSource.Play();
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
