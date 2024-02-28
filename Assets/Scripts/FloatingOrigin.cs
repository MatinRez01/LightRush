using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[DefaultExecutionOrder(1000)]
public class FloatingOrigin : MonoBehaviour
{
    [SerializeField]
    public static Action<Vector3> OriginShiftEventChannel;

    [SerializeField]
    private Rigidbody PlayerRigidbody;

    private float _threshold;

    private void Awake()
    {
        var sphereCollider = GetComponent<SphereCollider>();
        _threshold = sphereCollider.radius;
    }

    private void FixedUpdate()
    {
        var referencePosition = PlayerRigidbody.position;

        if (referencePosition.magnitude >= _threshold)
        {
            OriginShiftEventChannel.Invoke(-referencePosition);
        }
    }
}
