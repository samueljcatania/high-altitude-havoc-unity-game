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


        /*
         * Constructor for the ScoreEntry struct
         */
        public ScoreEntry(string name, float score)
        {
            Name = name;
            Score = score;
        }
    }


    /// <summary>
    /// This class is responsible for storing the leaderboard data using PlayerPrefs.
    /// </summary>
    public class LeaderboardDatabase : MonoBehaviour
    {
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


        /*
         * Ensures that the leaderboard database is not destroyed when a new scene is loaded
         */
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


        /*
         * Called when the game starts, provides initial setup for the leaderboard
         */
        private void Start()
        {
            _leaderboardEntries = new List<GameObject>();

            for (int i = 0; i < EntryCount; i++)
            {
                _leaderboardEntries.Add(null);
            }

            _scores = new List<ScoreEntry>();
            LoadScores();
        }


        /*
         * Initially loads the leaderboard data from PlayerPrefs
         */
        private void LoadScores()
        {
            for (int i = 0; i < EntryCount; ++i)
            {
                ScoreEntry entry;
                entry.Name = PlayerPrefs.GetString("[" + i + "].name", "");
                entry.Score = PlayerPrefs.GetFloat("[" + i + "].score", Infinity);
                _scores.Add(entry);
            }

            SortScores();
        }

        
        /*
         * Sorts the scores in descending order
         */
        private void SortScores()
        {
            _scores.Sort((a, b) => b.Score.CompareTo(a.Score));
        }


        /*
         * Saves the leaderboard data to PlayerPrefs
         */
        private void SaveScores()
        {
            for (int i = 0; i < EntryCount; ++i)
            {
                ScoreEntry entry = _scores[i];
                PlayerPrefs.SetString("[" + i + "].name", entry.Name);
                PlayerPrefs.SetFloat("[" + i + "].score", entry.Score);
            }
        }
        

        /*
         * Displays the leaderboard in the leaderboard menu
         */
        public void DisplayLeaderboard()
        {
            _leaderboardMenu = GameObject.Find("/MenuCanvas/LeaderboardMenu");

            var parentPosition = _leaderboardMenu.transform.position;

            for (int i = 0; i < EntryCount; i++)
            {
                if (_leaderboardEntries[i] != null)
                {
                    Destroy(_leaderboardEntries[i]);
                }

                var entry = _scores[i];

                var entryObject = Instantiate(leaderboardEntry, new Vector3(
                        parentPosition.x,
                        parentPosition.y - (i * 50),
                        parentPosition.z),
                    Quaternion.identity, _leaderboardMenu.transform);

                entryObject.transform.Find("Rank").GetComponent<TMPro.TextMeshProUGUI>().text = (i + 1).ToString();


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


        /*
         * Creates a new leaderboard entry and sets its name and score
         */
        public void RecordScore(string playerName, float playerTime)
        {
            _scores.Add(new ScoreEntry(playerName, playerTime));
            SortScores();
            _scores.RemoveAt(_scores.Count - 1);
            SaveScores();
        }
    
    }
}