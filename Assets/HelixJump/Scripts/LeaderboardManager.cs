using Dan.Main;
using System;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    private string[] leaderboardEntries = new string[10];
    public string[] LeaderboardEntries => leaderboardEntries;

    private bool isLeaderboardLoaded = false;
    public bool IsLeaderboardLoaded => isLeaderboardLoaded;

    public static LeaderboardManager singleton;

    public static Action OnLeaderboardLoaded;

    private void Awake()
    {
        // clear leaderboard
        for (int i = 0; i < leaderboardEntries.Length; i++)
            leaderboardEntries[i] = "";

        singleton = this;
    }

    private void Start()
    {
        isLeaderboardLoaded = false;

        // update the leaderboard
        LoadEntries();
    }

    public void LoadEntries()
    {
        isLeaderboardLoaded = false;

        Leaderboards.HelixJumpLeaderboard.GetEntries(entries =>
        {
            // clear leaderboard
            for (int i = 0; i < leaderboardEntries.Length; i++)
                leaderboardEntries[i] = "";

            // fill leaderboard values
            var length = Mathf.Min(leaderboardEntries.Length, entries.Length);
            for (int i = 0; i < length; i++)
                leaderboardEntries[i] = $"{entries[i].Rank}. {entries[i].Username}\n{entries[i].Score}";

            // update loaded state for UI to react accordingly
            isLeaderboardLoaded = true;

            // send update event for UI to react accordingly
            OnLeaderboardLoaded?.Invoke();
        });
    }

    public void UploadEntry(string username, int score)
    {
        Leaderboards.HelixJumpLeaderboard.UploadNewEntry(username, score, isSuccessful =>
        {
            if (isSuccessful)
                LoadEntries();
        });
    }
}
