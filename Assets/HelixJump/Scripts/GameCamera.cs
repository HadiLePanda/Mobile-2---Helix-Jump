using UnityEngine;

public class GameCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;

    [Header("Effects")]
    [SerializeField] private ParticleSystem winEffect;
    [SerializeField] private ParticleSystem loseEffect;
    [SerializeField] private ParticleSystem newHighscoreEffect;
    [SerializeField] private ParticleSystem scoreIncreaseEffect;

    [Header("Sounds")]
    [SerializeField] private AudioClip heartbreakSound;

    [Header("Settings")]
    [SerializeField] private float heartbreakSoundDelay = 0.5f;

    public static GameCamera singleton;

    private void Awake()
    {
        singleton = this;
    }

    private void Start()
    {
        // init effects
        winEffect.gameObject.SetActive(false);
        loseEffect.gameObject.SetActive(false);
        newHighscoreEffect.gameObject.SetActive(false);
        scoreIncreaseEffect.gameObject.SetActive(false);
    }

    public void PlayWinEffect()
    {
        winEffect.gameObject.SetActive(true);
        winEffect.Play(true);
    }
    public void PlayLoseEffect()
    {
        loseEffect.gameObject.SetActive(true);
        loseEffect.Play(true);

        // play heartbreak broken glass effect
        AudioManager.singleton.PlaySound2DOneShotWithDelay(heartbreakSound, pitchVariation: 0.1f, delay: heartbreakSoundDelay);
    }
    public void PlayScoreIncreaseEffect()
    {
        scoreIncreaseEffect.gameObject.SetActive(false);
        scoreIncreaseEffect.gameObject.SetActive(true);
        scoreIncreaseEffect.Play(true);
    }
    public void PlayNewHighscoreEffect()
    {
        newHighscoreEffect.gameObject.SetActive(false);
        newHighscoreEffect.gameObject.SetActive(true);
        newHighscoreEffect.Play(true);
    }

    public void ChangeBackgroundColor(Color color)
    {
        cam.backgroundColor = color;
    }
}
