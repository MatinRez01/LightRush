using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotate : MonoBehaviour
{
    [SerializeField] private float rotateSpeed;

    Vector3  rotateSpeedVector;
    private void OnEnable()
    {
        rotateSpeedVector = new Vector3(0, rotateSpeed, 0);
    }

    void Update()
    {
        transform.Rotate(rotateSpeedVector);
    }
}
