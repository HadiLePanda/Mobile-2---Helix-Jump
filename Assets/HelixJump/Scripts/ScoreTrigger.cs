using UnityEngine;

public class ScoreTrigger : MonoBehaviour
{
    [Header("References")]
    public BoxCollider boxCollider;

    private void AddScore()
    {
        GameManager.singleton.AddScore(1);
    }

    private void OnTriggerEnter(Collider other)
    {
        // disable the trigger
        boxCollider.enabled = false;

        // give points
        AddScore();

        // trigger logic fo rball passing through a score trigger
        BallController.singleton.OnPassedScoreTrigger();
    }
}
