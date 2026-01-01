using UnityEngine;

public class CoinCollectable : MonoBehaviour, IDestroySelf
{
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}