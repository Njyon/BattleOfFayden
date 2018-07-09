using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public delegate void StartGameDelegate(string playerName, int characterID);
    public StartGameDelegate onStartGame;

    private void Awake()
    {
        Application.runInBackground = true;
        DontDestroyOnLoad(this);
        
        // Load ingame options overlay
        SceneManager.LoadScene((int)SceneAlias.OptionsMenu, LoadSceneMode.Additive);
    }
}
