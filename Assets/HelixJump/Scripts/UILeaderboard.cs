using UnityEngine;

public class UILeaderboard : MonoBehaviour
{
    [Header("References")]
    public GameObject panel;
    public UILeaderboardEntry[] entryInstances;

    private void OnEnable()
    {
        LeaderboardManager.OnLeaderboardLoaded += OnLeaderboardLoaded;
    }
    private void OnDisable()
    {
        LeaderboardManager.OnLeaderboardLoaded -= OnLeaderboardLoaded;
    }

    private void Update()
    {
        // make sure to hide the leaderboard if we're not in the menu
        // avoid displaying the leaderboard if it's not loaded
        if (GameManager.singleton.GameState != GameState.Menu ||
            !LeaderboardManager.singleton.IsLeaderboardLoaded)
            Hide();
    }

    private void OnLeaderboardLoaded()
    {
        UpdateLeaderboardUI();
    }

    public void UpdateLeaderboardUI()
    {
        // clear all the entries
        foreach (var entryInstance in entryInstances)
            entryInstance.entryText.text = "";

        // update leaderboard entries
        var entries = LeaderboardManager.singleton.LeaderboardEntries;
        var length = Mathf.Min(entryInstances.Length, entries.Length);
        for (int i = 0; i < length; i++)
            entryInstances[i].entryText.text = !string.IsNullOrEmpty(entries[i]) ? entries[i] : "";
    }

    public void Show()
    {
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}
