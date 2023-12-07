using System;
using System.Collections.Generic;
using UnityEngine;

namespace Menu
{
    /// <summary>
    /// A struct that represents a single entry in the leaderboard containing the player's name and score.
    /// This struct makes it easier to use PlayerPrefs to store the leaderboard data.
    /// </summary>
    public struct ScoreEntry
    {
        // The name of the player
        public string Name;

        // The player's score
        public float Score;


        /// <summary>
        /// Constructor for the ScoreEntry struct.
        /// </summary>
        /// <param name="name"> The name of the player. </param>
        /// <param name="score"> The player's score. </param>
        public ScoreEntry(string name, float score)
        {
            Name = name;
            Score = score;
        }
    }


    /// <summary>
    /// This class is responsible for storing the leaderboard data using PlayerPrefs. It also displays the leaderboard
    /// in the leaderboard menu. It utilizes a singleton design pattern to ensure that there is only one instance of the
    /// leaderboard database in the scene, and that it is not destroyed when a new scene is loaded.
    /// </summary>
    public class LeaderboardDatabase : MonoBehaviour
    {
        // The singleton instance of the leaderboard database
        public static LeaderboardDatabase Instance { get; private set; }

        // Reference to the leaderboard entry prefab
        public GameObject leaderboardEntry;

        // Reference to the leaderboard menu
        private GameObject _leaderboardMenu;

        // Lists of all the entries in the leaderboard per difficulty
        private List<ScoreEntry> _scores;

        // Keep track of all the leaderboard entries in the scene so they can be destroyed when the leaderboard is closed
        private List<GameObject> _leaderboardEntries;

        // The total number of entries that will show on the leaderboard
        private const int EntryCount = 7;

        // A constant used to represent infinity
        private const float Infinity = Mathf.Infinity;


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
        /// When the game starts, initialize the leaderboard entries list and load the leaderboard data from PlayerPrefs.
        /// </summary>
        private void Start()
        {
            _leaderboardEntries = new List<GameObject>();

            for (var i = 0; i < EntryCount; i++)
            {
                _leaderboardEntries.Add(null);
            }

            _scores = new List<ScoreEntry>();
            LoadScores();
        }


        /// <summary>
        /// Loads the leaderboard data from PlayerPrefs.
        /// </summary>
        private void LoadScores()
        {
            for (var i = 0; i < EntryCount; ++i)
            {
                ScoreEntry entry;
                entry.Name = PlayerPrefs.GetString("[" + i + "].name", "");
                entry.Score = PlayerPrefs.GetFloat("[" + i + "].score", Infinity);
                _scores.Add(entry);
            }

            SortScores();
        }


        /// <summary>
        /// Sorts the leaderboard entries by score in descending order.
        /// </summary>
        private void SortScores()
        {
            _scores.Sort((a, b) => b.Score.CompareTo(a.Score));
        }


        /// <summary>
        /// Saves the leaderboard data to PlayerPrefs.
        /// </summary>
        private void SaveScores()
        {
            for (var i = 0; i < EntryCount; ++i)
            {
                var entry = _scores[i];
                PlayerPrefs.SetString("[" + i + "].name", entry.Name);
                PlayerPrefs.SetFloat("[" + i + "].score", entry.Score);
            }
        }


        /// <summary>
        /// Displays the leaderboard in the leaderboard menu.
        /// </summary>
        public void DisplayLeaderboard()
        {
            _leaderboardMenu = GameObject.Find("/MenuCanvas/LeaderboardMenu");

            var parentPosition = _leaderboardMenu.transform.position;

            // Destroy all the leaderboard entries from the previous time the leaderboard was opened
            for (var i = 0; i < EntryCount; i++)
            {
                if (_leaderboardEntries[i] != null)
                {
                    Destroy(_leaderboardEntries[i]);
                }

                var entry = _scores[i];

                // Create a new leaderboard entry and set its name and score and add it to the scene
                var entryObject = Instantiate(leaderboardEntry, new Vector3(
                        parentPosition.x,
                        parentPosition.y - (i * 50),
                        parentPosition.z),
                    Quaternion.identity, _leaderboardMenu.transform);

                entryObject.transform.Find("Rank").GetComponent<TMPro.TextMeshProUGUI>().text = (i + 1).ToString();


                // If the score is not infinity, display the name and score
                if (!float.IsPositiveInfinity(entry.Score))
                {
                    entryObject.transform.Find("Name").GetComponent<TMPro.TextMeshProUGUI>().text = entry.Name;
                    entryObject.transform.Find("Time").GetComponent<TMPro.TextMeshProUGUI>().text =
                        TimeSpan.FromSeconds(entry.Score).ToString(@"mm\:ss\:ff");
                }
                else
                {
                    entryObject.transform.Find("Name").GetComponent<TMPro.TextMeshProUGUI>().text = "";
                    entryObject.transform.Find("Time").GetComponent<TMPro.TextMeshProUGUI>().text = "";
                }

                _leaderboardEntries[i] = entryObject;
            }
        }


        /// <summary>
        /// Creates a new leaderboard entry with the given name and score and adds it to the leaderboard.
        /// </summary>
        /// <param name="playerName"> The name of the player. </param>
        /// <param name="playerTime"> The player's score. </param>
        public void RecordScore(string playerName, float playerTime)
        {
            _scores.Add(new ScoreEntry(playerName, playerTime));
            SortScores();
            _scores.RemoveAt(_scores.Count - 1);
            SaveScores();
        }
    }
}