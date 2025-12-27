using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float TimeAmount { get; private set; }
    public int Score { get; private set; }

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
    }

    private void Update()
    {
        TimeAmount += Time.deltaTime;
    }

    private void OnDestroy()
    {
        Lander.Instance.OnCoinPickup -= Lander_OnCoinPickup;
        Lander.Instance.OnLanded -= Lander_OnLanded;
    }

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)
    {
        AddScore(e.score);
    }

    private void Lander_OnCoinPickup(object sender, System.EventArgs e)
    {
        AddScore(500); 
    }

    public void AddScore(int addScoreAmount)
    {
        Score += addScoreAmount;
        print(Score);
    }
}