using UnityEngine;

public class cameraFollow : MonoBehaviour
{
	public Transform target;
    private Vector3 followOffset;

    private void Awake()
    {
        followOffset = transform.position - target.position;
    }
    private void FixedUpdate()
    {
        Vector3 targetPosition = target.position + followOffset;
        targetPosition.y = transform.position.y;
        transform.position = targetPosition;
    }
}
