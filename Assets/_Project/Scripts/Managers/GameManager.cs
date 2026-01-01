using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;

    [field: SerializeField] public CinemachineCamera CinemachineCamera { get; private set; }

    public float TimeAmount { get; private set; }
    public int Score { get; private set; }
    public bool IsTimerActive { get; private set; }


    [SerializeField] private List<GameLevel> _gameLevelList;

    private static int _levelNumber = 1;
    private static int _totalScore = 0;

    public static void ResetStaticData()
    {
        _levelNumber = 1;
        _totalScore = 0;
    }

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
        GameInputManager.Instance.OnMenuButtonPressed += GameInputManager_OnMenuButtonPressed;

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
        GameInputManager.Instance.OnMenuButtonPressed -= GameInputManager_OnMenuButtonPressed;
    }

    public int GetLevelNumber() => _levelNumber;
    public int GetTotalScore() => _totalScore;

    public void AddScore(int addScoreAmount)
    {
        Score += addScoreAmount;
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        OnGamePaused?.Invoke(this, EventArgs.Empty);
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1f;
        OnGameUnpaused?.Invoke(this, EventArgs.Empty);
    }

    public void PauseUnpauseGame()
    {
        if (Time.timeScale == 1f)
            PauseGame();
        else
            UnpauseGame();
    }

    public void RestartLevel()
    {
        SceneLoader.LoadScene(SceneLoader.Scene.Game);
    }

    public void GoToNextLevel()
    {
        _levelNumber++;
        _totalScore += Score;

        if (GetGameLevel() == null)
            SceneLoader.LoadScene(SceneLoader.Scene.GameOver);
        else
            SceneLoader.LoadScene(SceneLoader.Scene.Game);
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

    private void GameInputManager_OnMenuButtonPressed(object sender, EventArgs e) => PauseUnpauseGame();

    private void LoadCurrentLevel()
    {
        GameLevel gameLevel = GetGameLevel();
        GameLevel spawnedGameLevel = Instantiate(gameLevel, Vector3.zero, Quaternion.identity);
        Lander.Instance.transform.position = spawnedGameLevel.LanderStartPosition.position;
        CinemachineCamera.Target.TrackingTarget = spawnedGameLevel.CameraStartTarget;
        CinemachineCameraZoom2D.Instance.SetTargetOrthographicSize(spawnedGameLevel.ZoomedOutOrthographicSize);
    }

    private GameLevel GetGameLevel()
    {
        foreach (GameLevel gameLevel in _gameLevelList)
        {
            if (gameLevel.LevelNumber == _levelNumber)
                return gameLevel;
        }

        return null;
    }
}