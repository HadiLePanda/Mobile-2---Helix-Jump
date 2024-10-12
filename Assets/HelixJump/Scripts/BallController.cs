using UnityEngine;
using UnityEngine.UIElements;

public class BallController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    public MeshRenderer meshRenderer;

    [Header("Settings")]
    public float bounceForce = 5.0f;
    public float superspeedImpactBounceForce = 2.0f;
    public float bounceCooldown = 0.2f;
    public int perfectPassesForSuperSpeed = 2;
    public float superSpeedForceMultiplier = 1.2f;

    [Header("Audio")]
    public AudioClip bounceSound;
    public AudioClip scoreSound;
    public AudioClip superspeedImpactSound;
    public AudioClip superspeedActivatedSound;

    [Header("Effects")]
    public ParticleSystem superspeedAura;
    public ParticleSystem superspeedCollisionEffect;
    public ParticleSystem winConfettis;
    public ParticleSystem bounceEffect;

    private bool ignoreNextCollision;
    private Vector3 startPos;
    private int perfectPassCount = 0;
    private bool isSuperSpeedActive;

    public static BallController singleton;

    private void Awake()
    {
        singleton = this;
    }

    private void Start()
    {
        startPos = transform.position;

        // init effects
        winConfettis.Stop();
        superspeedAura.Stop();
        bounceEffect.Stop();
        superspeedCollisionEffect.Stop();
        superspeedAura.gameObject.SetActive(false);
        superspeedCollisionEffect.gameObject.SetActive(false);
        bounceEffect.gameObject.SetActive(false);
    }

    private void Update()
    {
        // handle activating super speed when condition is met
        if (perfectPassCount >= perfectPassesForSuperSpeed &&
            !isSuperSpeedActive)
        {
            // enable super speed
            isSuperSpeedActive = true;
            // enable superspeed aura
            superspeedAura.gameObject.SetActive(true);
            superspeedAura.Play();
            // play superspeed activation sound
            AudioManager.singleton.PlaySound2DOneShot(superspeedActivatedSound);

            // make the ball move a little faster
            rb.AddForce(bounceForce * superSpeedForceMultiplier * Vector3.down, ForceMode.Impulse);
        }
    }

    public void PlayWinEffect()
    {
        winConfettis.Play();
    }

    public void ChangeColor(Color color)
    {
        meshRenderer.material.color = color;
    }

    public void ResetBall()
    {
        Freeze();
        transform.position = startPos;
        Unfreeze();
    }

    public void OnPassedScoreTrigger()
    {
        // play score sound
        AudioManager.singleton.PlaySound2DOneShot(scoreSound, pitchVariation: 0.1f);

        // increase the pass count
        perfectPassCount++;
    }

    public void Freeze()
    {
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
    }
    public void Unfreeze()
    {
        rb.velocity = Vector3.zero;
        rb.isKinematic = false;
        AllowCollision();
    }

    private void OnCollisionEnter(Collision col)
    {
        if (ignoreNextCollision)
            return;

        // we're in superspeed mode
        if (isSuperSpeedActive)
        {
            // we didn't hit a goal part
            if (!col.transform.GetComponent<Goal>())
            {
                // play superspeed impact effect
                superspeedCollisionEffect.gameObject.SetActive(true);
                superspeedCollisionEffect.Play(true);

                // play superspeed impact sound
                AudioManager.singleton.PlaySound2DOneShot(superspeedImpactSound);

                // destroy the next platform we hit
                Platform platform = col.gameObject.GetComponentInParent<Platform>();
                if (platform)
                    platform.Explode();

                // make the ball bounce with a small force
                Bounce(superspeedImpactBounceForce);
            }
        }
        // we're in regular mode
        else
        {
            // we hit a death part
            if (col.gameObject.TryGetComponent(out DeathPart deathPart))
            {
                // trigger death
                Freeze();
                deathPart.TriggerDeath();
            }
            // we hit any other part
            else
            {
                // make the ball bounce
                Bounce();
            }
        }

        // prevent bouncing again before a small cooldown
        ignoreNextCollision = true;
        Invoke(nameof(AllowCollision), bounceCooldown);

        // reset super speed because we hit something
        perfectPassCount = 0;
        isSuperSpeedActive = false;
        // disable superspeed aura
        superspeedAura.Stop();
    }

    private void Bounce(float force = 0.0f)
    {
        // get the bounce force
        var desiredBounceForce = force > 0.0f ? force : bounceForce;

        // simulate upward bounce
        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.up * desiredBounceForce, ForceMode.Impulse);

        // play bounce sound
        AudioManager.singleton.PlaySound2DOneShot(bounceSound, pitchVariation: 0.1f);

        // play bounce effect
        bounceEffect.gameObject.SetActive(true);
        bounceEffect.Play();
    }

    private void AllowCollision()
    {
        ignoreNextCollision = false;
    }
}
