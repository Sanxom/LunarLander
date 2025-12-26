using UnityEngine;

public class FuelCollectable : MonoBehaviour
{
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}