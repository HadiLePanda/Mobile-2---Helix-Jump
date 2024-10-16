using System.Collections.Generic;
using UnityEngine;

public class HelixController : MonoBehaviour
{
    [Header("References")]
    public GameObject helixPlatformPrefab;
    public List<Stage> stages = new();

    [Header("Settings")]
    public Transform topTransform;
    public Transform goalTransform;
    public float helixRotationSpeedMultiplier = 0.4f;

    private bool canControl = false;
    private Vector2 lastTapPos;
    private Vector2 startRotation;
    private float helixDistance;
    private List<GameObject> spawnedPlatforms = new();

    public static HelixController singleton;

    private void Awake()
    {
        singleton = this;

        // setup starting values
        startRotation = transform.localEulerAngles;
        helixDistance = topTransform.localPosition.y - (goalTransform.localPosition.y + 0.1f);
    }

    private void Update()
    {
        HandleHelixMovement();
    }

    public void EnableControl() => canControl = true;
    public void DisableControl() => canControl = false;

    private void HandleHelixMovement()
    {
        if (canControl)
        {
            // detect when touching the screen
            if (Input.GetMouseButton(0))
            {
                Vector2 currentTapPos = Input.mousePosition;

                // remember last tap position
                if (lastTapPos == Vector2.zero)
                {
                    lastTapPos = currentTapPos;
                }

                float delta = lastTapPos.x - currentTapPos.x;
                lastTapPos = currentTapPos;

                // make the helix rotate based on the touch movement
                transform.Rotate(delta * helixRotationSpeedMultiplier * Vector3.up);
            }
        }

        // stopped screen touch
        if (Input.GetMouseButtonUp(0))
        {
            lastTapPos = Vector2.zero;
        }
    }

    public void LoadStage(int stageIndex)
    {
        // if the stage is higher than the max number of stages, go back to first stage
        if (stageIndex > stages.Count - 1)
        {
            stageIndex = 0;
        }

        // get the stage
        Stage stage = stages[Mathf.Clamp(stageIndex, 0, stages.Count - 1)];
        if (stage == null)
        {
            Debug.LogError("No stage " + stageIndex + " found in the stages list. Are all stages assigned in the list?");
            return;
        }

        // change background color
        GameCamera.singleton.ChangeBackgroundColor(stage.stageBackgroundColor);
        // change ball color
        BallController.singleton.ChangeColor(stage.stageBallColor);

        // reset helix rotation
        transform.localEulerAngles = startRotation;

        // destroy the old platforms if any
        foreach (GameObject platform in spawnedPlatforms)
        {
            Destroy(platform);
        }

        // create new platforms
        GeneratePlatforms(stage);
    }

    private void GeneratePlatforms(Stage stage)
    {
        float platformDistance = helixDistance / stage.platforms.Count;
        float spawnPosY = topTransform.localPosition.y;
        for (int i = 0; i < stage.platforms.Count; i++)
        {
            spawnPosY -= platformDistance;

            // spawn the platform
            GameObject platform = Instantiate(helixPlatformPrefab, transform);
            platform.transform.localPosition = new Vector3(0, spawnPosY, 0);
            spawnedPlatforms.Add(platform);

            // create gaps
            int numberOfPartsToDisable = 12 - stage.platforms[i].partCount; // 12 is the number of parts in a platform
            List<GameObject> disabledParts = new();
            while (disabledParts.Count < numberOfPartsToDisable)
            {
                int randomPartIndex = Random.Range(0, platform.transform.childCount);
                GameObject randomPart = platform.transform.GetChild(randomPartIndex).gameObject;
                if (!disabledParts.Contains(randomPart))
                {
                    randomPart.SetActive(false);
                    disabledParts.Add(randomPart);
                }
            }

            // color the platform parts
            List<GameObject> remainingParts = new();
            foreach (Transform childPart in platform.transform)
            {
                childPart.GetComponent<Renderer>().material.color = stage.stagePlatformPartColor;
                if (childPart.gameObject.activeInHierarchy)
                {
                    remainingParts.Add(childPart.gameObject);
                }
            }

            // create death parts
            int numberOfDeathParts = stage.platforms[i].deathPartCount;
            List<GameObject> deathParts = new();
            while (deathParts.Count < numberOfDeathParts)
            {
                int randomPartIndex = Random.Range(0, remainingParts.Count);
                GameObject randomPart = remainingParts[randomPartIndex];
                if (!deathParts.Contains(randomPart))
                {
                    randomPart.AddComponent<DeathPart>();
                    deathParts.Add(randomPart);
                }
            }
        }
    }
}
