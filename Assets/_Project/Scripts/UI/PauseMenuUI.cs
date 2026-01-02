using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _soundVolumeButton;
    [SerializeField] private Button _musicVolumeButton;
    [SerializeField] private TextMeshProUGUI _soundVolumeText;
    [SerializeField] private TextMeshProUGUI _musicVolumeText;

    private void Awake()
    {
        _soundVolumeButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.ChangeSoundVolume();
            _soundVolumeText.text = $"SOUND {SoundManager.Instance.GetSoundVolume()}";
        });

        _musicVolumeButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ChangeMusicVolume();
            _musicVolumeText.text = $"MUSIC {MusicManager.Instance.GetMusicVolume()}";
        });

        _resumeButton.onClick.AddListener(() =>
        {
            GameManager.Instance.UnpauseGame();
        });

        _mainMenuButton.onClick.AddListener(() =>
        {
            SceneLoader.LoadScene(SceneLoader.Scene.MainMenu);
        });
    }

    private void Start()
    {
        GameManager.Instance.OnGamePaused += GameManager_OnGamePaused;
        GameManager.Instance.OnGameUnpaused += GameManager_OnGameUnpaused;

        _soundVolumeText.text = $"SOUND {SoundManager.Instance.GetSoundVolume()}";
        _musicVolumeText.text = $"MUSIC {MusicManager.Instance.GetMusicVolume()}";

        Hide();
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGamePaused -= GameManager_OnGamePaused;
        GameManager.Instance.OnGameUnpaused -= GameManager_OnGameUnpaused;
    }

    private void GameManager_OnGamePaused(object sender, System.EventArgs e)
    {
        Show();
    }

    private void GameManager_OnGameUnpaused(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void Show()
    {
        gameObject.SetActive(true);
        _resumeButton.Select();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}