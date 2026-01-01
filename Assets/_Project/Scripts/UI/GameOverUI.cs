using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private TextMeshProUGUI _scoreText;

    private void Awake()
    {
        _mainMenuButton.onClick.AddListener(() => 
        {
            SceneLoader.LoadScene(SceneLoader.Scene.MainMenu);
        });
    }

    private void Start()
    {
        _scoreText.text = $"FINAL SCORE: {GameManager.Instance.GetTotalScore()}";

        _mainMenuButton.Select();
    }
}