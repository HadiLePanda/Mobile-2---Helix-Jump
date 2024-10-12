using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [Header("References")]
    public GameObject panel;
    public UILeaderboard leaderboardWindow;
    public Button playButton;
    public Button leaderboardButton;
    public TMP_InputField usernameInputField;

    private void Start()
    {
        // load the username if any, otherwise it's an empty string by default
        usernameInputField.text = GameManager.singleton.Username;
    }

    private void Update()
    {
        // show main menu only when in main menu state
        panel.SetActive(GameManager.singleton.GameState == GameState.Menu);

        // make play button only interactable if user entered a valid username
        playButton.interactable = IsUsernameValid();

        // make the leaderboard button only interactable when leaderboard is loaded
        leaderboardButton.interactable = LeaderboardManager.singleton.IsLeaderboardLoaded;
    }

    public void Play()
    {
        // check if username is valid
        if (!IsUsernameValid())
            return;

        // set player's username
        GameManager.singleton.SetPlayerUsername(usernameInputField.text);

        // start the game
        GameManager.singleton.StartGame();
    }

    public void ShowLeaderboard()
    {
        leaderboardWindow.Show();
    }
    public void CloseLeaderboard()
    {
        leaderboardWindow.Hide();
    }

    private bool IsUsernameValid()
    {
        return usernameInputField.text.Length > 2;
    }
}
