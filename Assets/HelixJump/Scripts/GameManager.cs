using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static string PREF_HIGHSCORE = "Highscore";

    private int highscore;
    private int score;
    private int currentStage = 0;

    public int HighScore => highscore;
    public int Score => score;

    public static GameManager singleton;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
        }

        // load highscore
        highscore = PlayerPrefs.GetInt(PREF_HIGHSCORE);
    }

    public void NextLevel()
    {
        // reset states
        BallController.singleton.ResetBall();

        // load next stage
        currentStage++;
        HelixController.singleton.LoadStage(currentStage);
    }

    public void RestartLevel()
    {
        // reset states
        singleton.score = 0;
        BallController.singleton.ResetBall();

        // reload stage
        HelixController.singleton.LoadStage(currentStage);
    }

    public void GameOver()
    {
        Debug.Log("Game Over");

        // TODO: show adds
        //Advertisement.Show();

        RestartLevel();
    }

    public void AddScore(int scoreToAdd)
    {
        // increase the score
        score += scoreToAdd;

        // we beat our high score
        if (score > highscore)
        {
            // update the new high score
            highscore = score;
            // save highscore in PlayerPrefs
            PlayerPrefs.SetInt(PREF_HIGHSCORE, highscore);
        }
    }
}
