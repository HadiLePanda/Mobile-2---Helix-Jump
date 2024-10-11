using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    public MeshRenderer meshRenderer;

    [Header("Settings")]
    public float bounceForce = 5.0f;
    public float bounceCooldown = 0.2f;
    public int perfectPassesForSuperSpeed = 2;
    public float superSpeedForceMultiplier = 1.2f;

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
    }

    private void Update()
    {
        // handle activating super speed when condition is met
        if (perfectPassCount >= perfectPassesForSuperSpeed && !isSuperSpeedActive)
        {
            // enable super speed
            isSuperSpeedActive = true;

            // make the ball a little faster
            rb.AddForce(Vector3.down * bounceForce * superSpeedForceMultiplier, ForceMode.Impulse);
        }
    }

    public void ResetBall()
    {
        Freeze();
        transform.position = startPos;
        Unfreeze();
    }

    public void IncreasePerfectPass()
    {
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
            if (!col.transform.GetComponent<Goal>())
            {
                // destroy the next platform we hit
                Platform platform = col.gameObject.GetComponentInParent<Platform>();
                if (platform)
                    Destroy(platform.gameObject);

                // make the ball bounce
                Bounce();
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
    }

    private void Bounce()
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
    }

    private void AllowCollision()
    {
        ignoreNextCollision = false;
    }
}
