using Unity.Cinemachine;
using UnityEngine;

public class CinemachineCameraZoom2D : MonoBehaviour
{
    public static CinemachineCameraZoom2D Instance { get; private set; }

    private const float NORMAL_ORTHOGRAPHIC_SIZE = 10f;

    [SerializeField] private CinemachineCamera _cinemachineCamera;

    private float _targetOrthographicSize = 10f;

    private void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        float zoomSpeed = 2f;
        _cinemachineCamera.Lens.OrthographicSize = Mathf.Lerp(_cinemachineCamera.Lens.OrthographicSize, _targetOrthographicSize, Time.deltaTime * zoomSpeed);
    }

    public void SetTargetOrthographicSize(float targetOrthographicSize)
    {
        _targetOrthographicSize = targetOrthographicSize;
    }

    public void SetNormalOrthographicSize()
    {
        SetTargetOrthographicSize(NORMAL_ORTHOGRAPHIC_SIZE);
    }
}