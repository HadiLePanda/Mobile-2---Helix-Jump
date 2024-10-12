using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGame : MonoBehaviour
{
    [Header("References")]
    public GameObject panel;
    public Button homeButton;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highscoreText;

    private void Update()
    {
        panel.SetActive(GameManager.singleton.GameState != GameState.Menu);

        homeButton.interactable = GameManager.singleton.GameState == GameState.Playing;

        scoreText.text = GameManager.singleton.Score.ToString();
        highscoreText.text = $"Best: {GameManager.singleton.HighScore}";
    }

    public void GoToMainMenu()
    {
        // only allow to go to main menu while in playing state
        if (GameManager.singleton.GameState != GameState.Playing)
            return;

        // return to main menu
        GameManager.singleton.GoToMainMenu();
    }
}
