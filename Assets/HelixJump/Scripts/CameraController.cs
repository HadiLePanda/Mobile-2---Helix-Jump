using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    public GameObject target;

    private float offset;

    private void Awake()
    {
        offset = transform.position.y - target.transform.position.y;
    }

    private void LateUpdate()
    {
        Vector3 currentPos = transform.position;
        currentPos.y = target.transform.position.y + offset;
        transform.position = currentPos;
    }
}
