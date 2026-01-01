using UnityEngine;

public class FuelCollectable : MonoBehaviour, IDestroySelf
{
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}