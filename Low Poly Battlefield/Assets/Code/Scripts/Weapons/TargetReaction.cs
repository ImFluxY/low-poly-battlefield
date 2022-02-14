using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetReaction : MonoBehaviour
{
    [SerializeField]
    private float hitRotationSpeed = 0.2f;
    [SerializeField]
    private float normalRotationTime = 5f;
    [SerializeField]
    private float normalRotationSpeed = 2f;
    [SerializeField]
    private Vector3 startRotation;
    [SerializeField]
    private float hitRotation;

    private float timer = 0.0f;
    private bool getHit = false;

    private void Start()
    {
        startRotation = transform.localEulerAngles;
    }

    public void Hit()
    {
        getHit = true;
    }

    private void Update()
    {
        if(getHit)
        {
            timer += Time.deltaTime;
            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3(hitRotation, startRotation.y, startRotation.z), hitRotationSpeed * Time.deltaTime);

            if(timer >= normalRotationTime)
            {
                timer = 0.0f;
                getHit = false;
            }
        }
        else
        {
            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, startRotation, normalRotationSpeed * Time.deltaTime);
        }
    }
}
