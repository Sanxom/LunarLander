using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _statsText;
    [SerializeField] private GameObject _speedLeftArrowGO;
    [SerializeField] private GameObject _speedRightArrowGO;
    [SerializeField] private GameObject _speedUpArrowGO;
    [SerializeField] private GameObject _speedDownArrowGO;
    [SerializeField] private Image _fuelImage;

    private void Update()
    {
        UpdateStatsText();
    }

    private void UpdateStatsText()
    {
        SetArrows();
        _fuelImage.fillAmount = Lander.Instance.GetFuelAmountNormalized();
        _statsText.text = 
            $"{GameManager.Instance.GetLevelNumber()}\n{GameManager.Instance.Score}\n{GameManager.Instance.TimeAmount:0}\n{Mathf.Abs(Lander.Instance.GetSpeedX()):0}\n{Mathf.Abs(Lander.Instance.GetSpeedY()):0}";
    }

    private void SetArrows()
    {
        _speedLeftArrowGO.SetActive(Lander.Instance.GetSpeedX() < 0f);
        _speedRightArrowGO.SetActive(Lander.Instance.GetSpeedX() >= 0f);
        _speedUpArrowGO.SetActive(Lander.Instance.GetSpeedY() >= 0f);
        _speedDownArrowGO.SetActive(Lander.Instance.GetSpeedY() < 0f);
    }
}