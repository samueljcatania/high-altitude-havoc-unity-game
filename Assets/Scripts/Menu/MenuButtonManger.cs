using System.Collections;
using System.Linq;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu
{
    /// <summary>
    /// A class that manages all the interactive UI elements in the main menu.
    /// </summary>
    public class MenuButtonManager : MonoBehaviour
    {
        // References to the three submenus in the main menu
        public GameObject mainMenu;
        public GameObject leaderboardMenu;
        public GameObject startMenu;

        // Reference to the FPS toggle in the main menu
        public Toggle fpsToggle;

        // References to the UI elements in the start menu
        public TMP_InputField playerName;

        // Reference to the game object that contains the leaderboard database
        private LeaderboardDatabase _leaderboardDatabase;

        // Reference to the menu audio manager that manages the audio in the main menu
        public MenuAudioManager menuAudioManager;

        // Reference to the stealth bomber engine manager that manages the engines in the stealth bomber
        public StealthBomberEngineManager stealthBomberEngineManager;

        // Reference to the two cinemachine cameras in the menu scene
        public CinemachineVirtualCamera cinemachineOrbitCamera;
        public CinemachineVirtualCamera cinemachineStartCamera;
        public CinemachineVirtualCamera cinemachinePlayCamera;

        // Reference to the hangar door glass
        public GameObject hangarDoorGlass;

        // Reference to the glowing glass material
        public Material glowingGlassMaterial;
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissiveColor");

        /*
         * Called when the game starts, provides initial setup for the main menu and its submenus
         */
        private void Start()
        {
            // Set the orbit camera to be active by default when the menu scene is loaded
            cinemachineOrbitCamera.gameObject.SetActive(true);
            cinemachineStartCamera.gameObject.SetActive(false);
            cinemachinePlayCamera.gameObject.SetActive(false);

            if (LeaderboardDatabase.Instance != null)
            {
                _leaderboardDatabase = LeaderboardDatabase.Instance;
            }

            // Set only the main menu to be active
            mainMenu.SetActive(true);
            leaderboardMenu.SetActive(false);
            startMenu.SetActive(false);

            // Add a listener to the player name input field to reset its color when the player starts typing
            playerName.onValueChanged.AddListener(delegate { playerName.image.color = Color.white; });

            // If PlayerPrefs does not contain a key for the FPS counter, set it to 0 (disabled)
            if (!PlayerPrefs.HasKey("FPS"))
            {
                PlayerPrefs.SetInt("FPS", 0);
            }

            // Set the FPS counter to its last known state
            fpsToggle.isOn = PlayerPrefs.GetInt("FPS") == 1;
            fpsToggle.onValueChanged.AddListener(ToggleFPS);
        }


        /*
         * Loads the main game scene
         */
        public void PlayGame()
        {
            if (!string.IsNullOrWhiteSpace(playerName.text))
            {
                startMenu.SetActive(false);
                
                cinemachineStartCamera.gameObject.SetActive(false);
                cinemachinePlayCamera.gameObject.SetActive(true);

                string stringWithoutSpaces = new string(playerName.text.Where(c => !char.IsWhiteSpace(c)).ToArray());

                // Cut the string to 3 characters if it's longer than that
                if (stringWithoutSpaces.Length > 3)
                {
                    stringWithoutSpaces = stringWithoutSpaces[..3].ToUpper();
                }

                PlayerPrefs.SetString("PlayerName", stringWithoutSpaces);

                // Set the hangar door glass material to the glowing glass material
                hangarDoorGlass.GetComponent<MeshRenderer>().material = glowingGlassMaterial;
                
                // Create a coroutine that will animate the hangar door glowing then load the game scene
                StartCoroutine(HangarDoorGlowing());
            }
            else
            {
                playerName.image.color = Color.red;
            }
        }

        private IEnumerator HangarDoorGlowing()
        {
            // Increase the intensity of the glow
            for (float i = 2; i < 50; i += 2)
            {
                glowingGlassMaterial.SetColor(EmissionColor, new Color(1f, 1f, 1f) * i);
                yield return new WaitForSeconds(0.1f);
            }

            SceneManager.LoadScene("GameScene");
        }


        /*
         * Transitions from the main menu to the start menu
         */
        public void OpenStartMenu()
        {
            // When the player enters the start menu, switch the camera to the start camera
            cinemachineOrbitCamera.gameObject.SetActive(false);
            cinemachineStartCamera.gameObject.SetActive(true);

            menuAudioManager.PlayJetStartupSound();
            stealthBomberEngineManager.StartEngines();
            mainMenu.SetActive(false);
            startMenu.SetActive(true);
        }


        /*
         * Transitions from the start menu to the main menu
         */
        public void CloseStartMenu()
        {
            // When the player exits the start menu, switch the camera to the orbit camera
            cinemachineStartCamera.gameObject.SetActive(false);
            cinemachineOrbitCamera.gameObject.SetActive(true);

            menuAudioManager.StopJetStartupSound();
            stealthBomberEngineManager.StopEngines();
            startMenu.SetActive(false);
            mainMenu.SetActive(true);
        }


        /*
         * Transition to the leaderboard menu from the main menu
         */
        public void OpenLeaderboard()
        {
            mainMenu.SetActive(false);
            leaderboardMenu.SetActive(true);

            _leaderboardDatabase.GetComponent<LeaderboardDatabase>().DisplayLeaderboard();
        }

        /*
         * Transitions from the leaderboard menu to the main menu
         */
        public void CloseLeaderboard()
        {
            leaderboardMenu.SetActive(false);
            mainMenu.SetActive(true);
        }


        /*
         * Exits the game
         */
        public void ExitGame()
        {
            Application.Quit();
        }

        /*
         * Called whenever the toggle button is clicked, toggles the FPS counter to the opposite of its current state
         */
        private static void ToggleFPS(bool value)
        {
            PlayerPrefs.SetInt("FPS", value ? 1 : 0);
        }
    }
}