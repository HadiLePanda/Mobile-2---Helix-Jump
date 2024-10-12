using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Menu,
    Playing,
    Win,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static string PREF_HIGHSCORE = "Highscore";
    public static string PREF_USERNAME = "Username";

    [Header("Settings")]
    [SerializeField] private int triesBeforeShowingAd = 3;
    [SerializeField] private float winSequenceTime = 2f;
    [SerializeField] private float gameOverSequenceTime = 2f;
    [SerializeField] private float menuMusicVolume = 0.1f;
    [SerializeField] private float gameplayMusicVolume = 0.3f;
    [SerializeField] private float sequenceMusicVolume = 0.15f;

    [Header("Audio")]
    public AudioClip levelWinSound;
    public AudioClip levelLoseSound;

    private int highscore;
    private int score;
    private int currentStageIndex = 0;
    private int currentTriesBeforeAd = 0;
    private GameState gameState;
    private string username = string.Empty;
    private int highscoreAtGameStart;

    private Coroutine winRoutine;
    private Coroutine gameOverRoutine;

    public int HighScore => highscore;
    public int Score => score;
    public GameState GameState => gameState;
    public string Username => username;

    public static GameManager singleton;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
        }

        // load username
        username = PlayerPrefs.GetString(PREF_USERNAME, string.Empty);

        // load highscore
        highscore = PlayerPrefs.GetInt(PREF_HIGHSCORE, 0);
    }

    private void Start()
    {
        // init game states
        LoadMainMenu();
    }

    public void SetPlayerUsername(string username)
    {
        // in case the username is empty set it to 'User' by default
        // otherwise store the player's chosen username
        if (string.IsNullOrEmpty(username))
            this.username = "User";
        else
            this.username = username;

        // save username in playerprefs
        PlayerPrefs.SetString(PREF_USERNAME, this.username);
    }

    public void StartGame()
    {
        score = 0;
        highscoreAtGameStart = highscore;
        LoadFirstStage();
        UnpauseGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        HelixController.singleton.DisableControl();
        if (gameState != GameState.Menu)
            AudioManager.singleton.PauseMusic();
    }
    public void UnpauseGame()
    {
        Time.timeScale = 1;
        HelixController.singleton.EnableControl();
        if (gameState != GameState.Menu)
            AudioManager.singleton.ResumeMusic();
    }

    public void GoToMainMenu()
    {
        // reload the game to get back to the default state when launching the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    // only when finished reloading the main menu, initialize its state
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // we loaded the first scene, supposed to be the main menu
        // in this game there's only one scene, the main menu is included in the same scene as the gameplay
        if (scene.buildIndex == 0)
        {
            // load main menu states
            LoadMainMenu();
        }
    }
    public void LoadMainMenu()
    {
        gameState = GameState.Menu;

        score = 0;
        highscoreAtGameStart = highscore;

        AudioManager.singleton.StopMusic();
        AudioManager.singleton.SetMusicVolume(menuMusicVolume);
        AudioManager.singleton.PlayMusic();
        PauseGame();
    }
    public void LoadFirstStage() => LoadStage(0);
    public void RestartStage() => LoadStage(currentStageIndex);
    public void LoadNextStage()
    {
        var lastStageIndex = HelixController.singleton.stages.Count - 1;
        var isLastStage = currentStageIndex + 1 > lastStageIndex;

        var stageIndex = !isLastStage ? currentStageIndex + 1 : 0;
        LoadStage(stageIndex);
    }

    public void LoadStage(int stageIndex)
    {
        gameState = GameState.Playing;

        // set music volume to gameplay volume
        AudioManager.singleton.SetMusicVolume(gameplayMusicVolume);

        // update current stage reference
        currentStageIndex = stageIndex;

        // reset ball
        BallController.singleton.ResetBall();

        // load stage
        HelixController.singleton.LoadStage(stageIndex);

        // enable controls
        HelixController.singleton.EnableControl();
    }

    public void StageWin()
    {
        // play win sequence
        if (winRoutine == null)
            StopCoroutine(PlayWinSequence());
        winRoutine = StartCoroutine(PlayWinSequence());
    }
    private IEnumerator PlayWinSequence()
    {
        gameState = GameState.Win;

        // freeze ball
        BallController.singleton.Freeze();

        // disable controls
        HelixController.singleton.DisableControl();

        // play win effects
        BallController.singleton.PlayWinEffect();
        GameCamera.singleton.PlayWinEffect();

        // play win sound
        AudioManager.singleton.PlaySound2DOneShot(levelWinSound);

        // lower music volume
        AudioManager.singleton.SetMusicVolume(sequenceMusicVolume);

        // wait for win sequence
        yield return new WaitForSeconds(winSequenceTime);

        // load next stage
        LoadNextStage();
    }

    public void GameOver()
    {
        // play game over sequence
        if (gameOverRoutine == null)
            StopCoroutine(PlayGameOverSequence());
        gameOverRoutine = StartCoroutine(PlayGameOverSequence());
    }
    private IEnumerator PlayGameOverSequence()
    {
        gameState = GameState.GameOver;

        // freeze ball
        BallController.singleton.Freeze();

        // disable controls
        HelixController.singleton.DisableControl();

        // show game over effect
        GameCamera.singleton.PlayLoseEffect();

        // play game over sound
        AudioManager.singleton.PlaySound2DOneShot(levelLoseSound);

        // increase tries count to trigger ads
        currentTriesBeforeAd++;

        // lower music volume
        AudioManager.singleton.SetMusicVolume(sequenceMusicVolume);

        // send score to leaderboard
        // only send when there is a new highscore recorded
        if (score > highscoreAtGameStart)
        {
            LeaderboardManager.singleton.UploadEntry(username, score);
        }

        // wait for game over sequence
        yield return new WaitForSeconds(gameOverSequenceTime);

        // restart the game
        StartGame();

        // play ads if needed
        // when we played a number of sessions, show ad
        if (currentTriesBeforeAd >= triesBeforeShowingAd)
        {
            // pause the game to show ads
            PauseGame();

            // show ad
            AdsIntersital.singleton.ShowAd();
            // reset tries count
            currentTriesBeforeAd = 0;
        }
    }

    public void AddScore(int scoreToAdd)
    {
        // increase the score
        score += scoreToAdd;

        // play score increase effect
        GameCamera.singleton.PlayScoreIncreaseEffect();

        // we beat our high score
        if (score > highscore)
        {
            // update the new high score
            highscore = score;
            // save highscore in PlayerPrefs
            PlayerPrefs.SetInt(PREF_HIGHSCORE, highscore);

            // play new highscore effect
            GameCamera.singleton.PlayNewHighscoreEffect();  
        }
    }
}
