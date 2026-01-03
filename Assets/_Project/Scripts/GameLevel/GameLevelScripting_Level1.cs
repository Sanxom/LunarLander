using UnityEngine;

public class GameLevelScripting_Level1 : MonoBehaviour
{
    [SerializeField] private CargoArea _dropOffCargoArea;

    private void Start()
    {
        _dropOffCargoArea.OnDroppedOff += DropOffCargoArea_OnDroppedOff;
    }

    private void DropOffCargoArea_OnDroppedOff(object sender, System.EventArgs e)
    {
        GameManager.Instance.AddScore(1000);
    }
}