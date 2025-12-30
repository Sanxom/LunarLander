using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LandedUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _statsText;
    [SerializeField] private TextMeshProUGUI _nextButtonText;
    [SerializeField] private Button _nextButton;

    [SerializeField] private float _timeToWaitBeforeShowing = 1f;

    private Action _nextButtonClickAction;
    
    private void Awake()
    {
        _nextButton.onClick.AddListener(() =>
        {
            _nextButtonClickAction();
        });
    }

    private void Start()
    {
        Lander.Instance.OnLanded += Lander_OnLanded;
        Hide();
    }

    private void OnDestroy()
    {
        Lander.Instance.OnLanded -= Lander_OnLanded;
    }

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)
    {
        if (e.landingType == Lander.LandingType.Success)
        {
            _titleText.text = "SUCCESSFUL LANDING!";
            _nextButtonText.text = "CONTINUE";
            _nextButtonClickAction = GameManager.Instance.GoToNextLevel;
            Show();
        }
        else
        {
            _titleText.text = "<color=#ff0000>CRASHED!</color>";
            _nextButtonText.text = "RETRY";
            _nextButtonClickAction = GameManager.Instance.RestartLevel;
            Invoke(nameof(Show), _timeToWaitBeforeShowing);
        }

        _statsText.text = $"{e.landingSpeed * 2f: 0}\n{e.dotVector * 100f: 0}\nx{e.scoreMultiplier}\n{e.score}";
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}