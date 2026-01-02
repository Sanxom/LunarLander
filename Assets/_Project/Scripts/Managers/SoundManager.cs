using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public event EventHandler OnSoundVolumeChanged;

    private static int _soundVolume = 1;

    private const int SOUND_VOLUME_MAX = 10;

    [SerializeField] private AudioClip _fuelPickupAudioClip;
    [SerializeField] private AudioClip _coinPickupAudioClip;
    [SerializeField] private AudioClip _crashAudioClip;
    [SerializeField] private AudioClip _landingSuccessAudioClip;
    [SerializeField] private Camera _mainCamera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        Lander.Instance.OnFuelPickup += Lander_OnFuelPickup;
        Lander.Instance.OnCoinPickup += Lander_OnCoinPickup;
        Lander.Instance.OnLanded += Lander_OnLanded;
    }
    
    private void OnDestroy()
    {
        Lander.Instance.OnFuelPickup -= Lander_OnFuelPickup;
        Lander.Instance.OnCoinPickup -= Lander_OnCoinPickup;
        Lander.Instance.OnLanded -= Lander_OnLanded;
    }

    public float GetSoundVolumeNormalized()
    {
        return ((float)_soundVolume) / SOUND_VOLUME_MAX;
    }

    public int GetSoundVolume()
    {
        return _soundVolume;
    }

    public void ChangeSoundVolume()
    {
        _soundVolume = (_soundVolume + 1) % SOUND_VOLUME_MAX;
        OnSoundVolumeChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)
    {
        switch (e.landingType)
        {
            case Lander.LandingType.Success:
                AudioSource.PlayClipAtPoint(_landingSuccessAudioClip, _mainCamera.transform.position, GetSoundVolumeNormalized());
                break;
            default:
                AudioSource.PlayClipAtPoint(_crashAudioClip, _mainCamera.transform.position, GetSoundVolumeNormalized());
                break;
        }
    }

    private void Lander_OnCoinPickup(object sender, System.EventArgs e)
    {
        AudioSource.PlayClipAtPoint(_coinPickupAudioClip, _mainCamera.transform.position, GetSoundVolumeNormalized());
    }

    private void Lander_OnFuelPickup(object sender, System.EventArgs e)
    {
        AudioSource.PlayClipAtPoint(_fuelPickupAudioClip, _mainCamera.transform.position, GetSoundVolumeNormalized());
    }
}