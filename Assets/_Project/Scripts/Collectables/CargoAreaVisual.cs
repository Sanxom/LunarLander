using UnityEngine;
using UnityEngine.UI;

public class CargoAreaVisual : MonoBehaviour
{
    [SerializeField] private Image _interactBar;

    private CargoArea _cargoArea;

    private void Awake()
    {
        _cargoArea = GetComponent<CargoArea>();
    }

    private void Start()
    {
        _cargoArea.OnInteractTimerChanged += CargoArea_OnInteractTimerChanged;
        _interactBar.fillAmount = 0f;
    }

    private void CargoArea_OnInteractTimerChanged(object sender, System.EventArgs e)
    {
        _interactBar.fillAmount = _cargoArea.GetInteractTimerNormalized();
    }
}