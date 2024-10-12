using UnityEngine;

public class UIMainMenu : MonoBehaviour
{
    [Header("References")]
    public GameObject panel;

    private void Update()
    {
        panel.SetActive(GameManager.singleton.GameState == GameState.Menu);
    }

    public void Play()
    {
        GameManager.singleton.StartGame();
    }
}
