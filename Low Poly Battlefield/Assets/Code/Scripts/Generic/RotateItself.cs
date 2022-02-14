using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateItself : MonoBehaviour
{
    public float RotationSpeed = 200;

    void Update()
    {
        transform.Rotate(0, RotationSpeed * Time.deltaTime, 0, Space.World);
    }
}
