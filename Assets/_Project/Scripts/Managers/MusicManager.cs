using System;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    public event EventHandler OnMusicVolumeChanged;

    private static float _musicTime;
    private static int _musicVolume = 1;

    private const int MUSIC_VOLUME_MAX = 10;

    private AudioSource _musicAudioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _musicAudioSource = GetComponent<AudioSource>();
        _musicAudioSource.time = _musicTime;
    }

    private void Start()
    {
        _musicAudioSource.volume = GetMusicVolumeNormalized();
    }

    private void Update()
    {
        _musicTime = _musicAudioSource.time;
    }

    public float GetMusicVolumeNormalized()
    {
        return ((float)_musicVolume) / MUSIC_VOLUME_MAX;
    }

    public int GetMusicVolume()
    {
        return _musicVolume;
    }

    public void ChangeMusicVolume()
    {
        _musicVolume = (_musicVolume + 1) % MUSIC_VOLUME_MAX;
        _musicAudioSource.volume = GetMusicVolumeNormalized();
        OnMusicVolumeChanged?.Invoke(this, EventArgs.Empty);
    }
}