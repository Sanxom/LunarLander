using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [field: SerializeField] public CinemachineCamera CinemachineCamera { get; private set; }

    public float TimeAmount { get; private set; }
    public int Score { get; private set; }
    public bool IsTimerActive { get; private set; }


    [SerializeField] private List<GameLevel> _gameLevelList;

    private static int _levelNumber = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        Lander.Instance.OnCoinPickup += Lander_OnCoinPickup;
        Lander.Instance.OnLanded += Lander_OnLanded;
        Lander.Instance.OnStateChanged += Lander_OnStateChanged;

        LoadCurrentLevel();
    }

    private void Update()
    {
        if (!IsTimerActive) return;

        TimeAmount += Time.deltaTime;
    }

    private void OnDestroy()
    {
        Lander.Instance.OnCoinPickup -= Lander_OnCoinPickup;
        Lander.Instance.OnLanded -= Lander_OnLanded;
        Lander.Instance.OnStateChanged -= Lander_OnStateChanged;
    }

    public int GetLevelNumber() => _levelNumber;

    public void AddScore(int addScoreAmount)
    {
        Score += addScoreAmount;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(0);
    }

    public void GoToNextLevel()
    {
        _levelNumber++;

        if (_levelNumber > _gameLevelList.Count)
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false; // TODO: Change this to Main Menu or something
#else
            Application.Quit();
#endif
        }
        SceneManager.LoadScene(0);
    }

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)
    {
        AddScore(e.score);
    }

    private void Lander_OnCoinPickup(object sender, System.EventArgs e)
    {
        AddScore(500); 
    }

    private void Lander_OnStateChanged(object sender, Lander.OnStateChangedEventArgs e)
    {
        IsTimerActive = e.state == Lander.State.Normal;

        if (e.state == Lander.State.Normal)
        {
            CinemachineCamera.Target.TrackingTarget = Lander.Instance.transform;
            CinemachineCameraZoom2D.Instance.SetNormalOrthographicSize();
        }
    }

    private void LoadCurrentLevel()
    {
        foreach (GameLevel gameLevel in _gameLevelList)
        {
            if (gameLevel.LevelNumber == _levelNumber)
            {
                GameLevel spawnedGameLevel = Instantiate(gameLevel, Vector3.zero, Quaternion.identity);
                Lander.Instance.transform.position = spawnedGameLevel.LanderStartPosition.position;
                CinemachineCamera.Target.TrackingTarget = spawnedGameLevel.CameraStartTarget;
                CinemachineCameraZoom2D.Instance.SetTargetOrthographicSize(spawnedGameLevel.ZoomedOutOrthographicSize);
            }
        }
    }
}