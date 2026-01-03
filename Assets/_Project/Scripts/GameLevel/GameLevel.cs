using UnityEngine;

public class GameLevel : MonoBehaviour
{
    [field: SerializeField] public int LevelNumber { get; private set; }
    [field: SerializeField] public Transform LanderStartPosition { get; private set; }
    [field: SerializeField] public Transform CameraStartTarget { get; private set; }
    [field: SerializeField] public float ZoomedOutOrthographicSize { get; private set; }
}