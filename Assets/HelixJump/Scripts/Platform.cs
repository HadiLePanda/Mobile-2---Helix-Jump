using UnityEngine;

public class Platform : MonoBehaviour
{
    [Header("Effects")]
    [SerializeField] private ParticleSystem explosionPrefab;

    [Header("Audio")]
    [SerializeField] private AudioClip explosionSound;

    public void Explode()
    {
        // spawn explosion prefab
        Instantiate(explosionPrefab.gameObject, transform.position, Quaternion.identity);

        // play explosion sound
        AudioManager.singleton.PlaySound2DOneShot(explosionSound, pitchVariation: 0.1f);

        // destroy the platform
        Destroy(gameObject);
    }
}
