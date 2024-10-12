using TMPro;
using UnityEngine;

public class UIGame : MonoBehaviour
{
    [Header("References")]
    public GameObject panel;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highscoreText;

    private void Update()
    {
        panel.SetActive(GameManager.singleton.GameState != GameState.Menu);

        scoreText.text = GameManager.singleton.Score.ToString();
        highscoreText.text = $"Best: {GameManager.singleton.HighScore}";
    }
}
