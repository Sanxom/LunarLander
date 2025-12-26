using TMPro;
using UnityEngine;

public class LandingPadVisual : MonoBehaviour
{
    [SerializeField] private TextMeshPro _scoreMultiplierText;

    private void Awake()
    {
        LandingPad landingPad = GetComponent<LandingPad>();
        _scoreMultiplierText.text = $"x{landingPad.ScoreMultiplier}";
    }
}