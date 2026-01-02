using UnityEngine;

public class LanderAudio : MonoBehaviour
{
    [SerializeField] private AudioSource _thrusterAudioSource;

    private Lander _lander;

    private void Awake()
    {
        _lander = GetComponent<Lander>();
    }

    private void Start()
    {
        _lander.OnBeforeForce += Lander_OnBeforeForce;
        _lander.OnLeftForce += Lander_OnLeftForce;
        _lander.OnRightForce += Lander_OnRightForce;
        _lander.OnUpForce += Lander_OnUpForce;
        SoundManager.Instance.OnSoundVolumeChanged += SoundManager_OnSoundVolumeChanged;
        _thrusterAudioSource.Pause();
    }

    private void SoundManager_OnSoundVolumeChanged(object sender, System.EventArgs e)
    {
        _thrusterAudioSource.volume = SoundManager.Instance.GetSoundVolumeNormalized();
    }

    private void OnDestroy()
    {
        _lander.OnBeforeForce -= Lander_OnBeforeForce;
        _lander.OnLeftForce -= Lander_OnLeftForce;
        _lander.OnRightForce -= Lander_OnRightForce;
        _lander.OnUpForce -= Lander_OnUpForce;
    }

    private void Lander_OnBeforeForce(object sender, System.EventArgs e)
    {
        _thrusterAudioSource.Pause();
    }

    private void Lander_OnUpForce(object sender, System.EventArgs e)
    {
        if (_thrusterAudioSource.isPlaying) return;
        _thrusterAudioSource.Play();
    }

    private void Lander_OnRightForce(object sender, System.EventArgs e)
    {
        if (_thrusterAudioSource.isPlaying) return;
        _thrusterAudioSource.Play();
    }

    private void Lander_OnLeftForce(object sender, System.EventArgs e)
    {
        if (_thrusterAudioSource.isPlaying) return;
        _thrusterAudioSource.Play();
    }
}