using UnityEngine;

/// <summary>
/// This class implements a Singleton design pattern to manage the game state globally.
/// </summary>
public class GameStateManager : MonoBehaviour
{
    // The Instance property that's used to enforce the Singleton design pattern
    private static GameStateManager Instance { get; set; }

    // The current game state
    private GameState currentGameState;

    
    /// <summary>
    /// Keeps track of the current game state.
    /// </summary>
    public static GameState CurrentGameState
    {
        get => Instance.currentGameState;
        set => Instance.currentGameState = value;
    }


    /// <summary>
    /// Enforces the Singleton design pattern when the game starts.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

        
    /// <summary>
    /// Set the initial game state to Cutscene.
    /// </summary>
    private void Start()
    {
        currentGameState = GameState.CutsceneWidePan;
    }
}