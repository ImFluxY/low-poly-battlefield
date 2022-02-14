using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private bool debug;
    [SerializeField]
    private GameObject laserDecal;
    [SerializeField]
    private Transform laserPoint;

    private GameObject decal;
    private Projector projector;

    private void Start()
    {
        decal = Instantiate(laserDecal, laserPoint.position, laserPoint.rotation);
        projector = decal.GetComponent<Projector>();
    }

    private void LateUpdate()
    {
        Ray ray = new Ray(laserPoint.position, laserPoint.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit))
        {
            if(debug)
                Debug.DrawLine(laserPoint.position, hit.point);

            if (decal.activeSelf == false)
                decal.SetActive(true);

            decal.transform.position = hit.point;
            decal.transform.rotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal);
        }
        else
        {
            decal.SetActive(false);
        }
    }
}