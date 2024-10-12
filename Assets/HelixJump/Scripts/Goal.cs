using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (GameManager.singleton.GameState != GameState.Win)
        {
            // trigger level win
            GameManager.singleton.StageWin();
        }
    }
}
