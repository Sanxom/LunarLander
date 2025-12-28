using TMPro;
using UnityEngine;

public class LandedUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _statsText;

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
            _titleText.text = "SUCCESSFUL LANDING!";
        else
            _titleText.text = "<color=#ff0000>CRASHED!</color>";

        _statsText.text = $"{e.landingSpeed * 2f: 0}\n{e.dotVector * 100f: 0}\nx{e.scoreMultiplier}\n{e.score}";
        Show();
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