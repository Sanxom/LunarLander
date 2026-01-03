using UnityEngine;

public class CargoRopeCrate : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.TryGetComponent(out Lander _))
        {
            Lander.Instance.CargoCrashed();
        }
    }
}