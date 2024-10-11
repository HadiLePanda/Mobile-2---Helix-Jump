using TMPro;
using UnityEngine;

public class UIGame : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highscoreText;

    private void Update()
    {
        scoreText.text = GameManager.singleton.Score.ToString();
        highscoreText.text = $"Best: {GameManager.singleton.HighScore}";
    }
}
