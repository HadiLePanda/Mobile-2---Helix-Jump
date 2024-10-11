using UnityEngine;

public class DeathPart : MonoBehaviour
{
    private Renderer rendererComponent;

    private void OnEnable()
    {
        rendererComponent = GetComponent<MeshRenderer>();
        rendererComponent.material.color = Color.red;
    }

    public void TriggerDeath()
    {
        GameManager.singleton.GameOver();
    }
}
