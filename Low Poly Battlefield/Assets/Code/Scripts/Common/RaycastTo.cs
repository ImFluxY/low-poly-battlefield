using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastTo : MonoBehaviour
{
    [SerializeField]
    private Color rayColor = Color.white;
    [SerializeField]
    private float distance = 100f;

    private void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * distance, rayColor);
    }
}
