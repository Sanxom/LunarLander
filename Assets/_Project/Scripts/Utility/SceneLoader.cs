using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public enum Scene
    {
        MainMenu,
        Game,
        GameOver,
    }

    public static void LoadScene(Scene scene)
    {
        SceneManager.LoadScene($"{scene}");
    }
}