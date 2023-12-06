using UnityEngine;

namespace StealthBomber
{
    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }

        private GameState _currentGameState;

        public static GameState CurrentGameState
        {
            get { return Instance._currentGameState; }
            set { Instance._currentGameState = value; }
        }

        void Awake()
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

        // Additional methods related to game state can be added here
    }
}