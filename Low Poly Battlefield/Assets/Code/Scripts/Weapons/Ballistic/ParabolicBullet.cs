using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class ParabolicBullet : MonoBehaviour
{
    [SerializeField]
    private LayerMask layerMask;

    private float speed;
    private float gravity;
    private Vector3 startPos;
    private Vector3 startForward;
    private float bulletDamage;

    private bool isInitialized = false;
    private float startTime = -1f;

    private BallisticManager ballisticManager;
    private PhotonView PV;

    public void Initialize(BallisticManager ballisticManager, float bulletDamage, Transform startPoint, float speed, float gravity)
    {
        startPos = startPoint.position;
        startForward = startPoint.forward;
        this.ballisticManager = ballisticManager;
        this.bulletDamage = bulletDamage;
        this.speed = speed;
        this.gravity = gravity;
        isInitialized = true;
    }

    private void OnHit(RaycastHit hit, Vector3 direction)
    {
        Debug.Log("Hit : " + hit.transform.name + ", direction : " + direction);

        if (hit.transform.TryGetComponent(out CharacterPart part))
        {
            part.OnHit(bulletDamage);
        }

        /*
        if (hit.transform.parent.TryGetComponent(out TargetReaction target))
        {
            target.Hit();
        }
        */

        if(hit.transform.TryGetComponent(out Rigidbody rb))
        {
            rb.AddForceAtPosition(direction * (speed / 2), hit.point, ForceMode.Force);
        }

        for (int i = 0; i < ballisticManager.surfaceTypes.Length; i++)
        {
            if (ballisticManager.surfaceTypes[i].tagName == hit.transform.tag)
            {
                PhotonNetwork.Instantiate(Path.Combine("Weapons", "Ballistic", ballisticManager.surfaceTypes[i].hitPrefabs[0].name), hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));

                if(ballisticManager.surfaceTypes[i].decalsPrefabs[0] != null)
                {
                    GameObject go = PhotonNetwork.Instantiate(Path.Combine("Weapons", "Ballistic", "Decals", ballisticManager.surfaceTypes[i].decalsPrefabs[0].name), hit.point, Quaternion.LookRotation(hit.normal, Vector3.up));
                    go.transform.SetParent(hit.transform);
                }

            }
        }

        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (!isInitialized) return;
        if (startTime < 0) startTime = Time.time;

        RaycastHit hit;
        float currentTime = Time.time - startTime;
        float prevTime = currentTime - Time.fixedDeltaTime;
        float nextTime = currentTime + Time.fixedDeltaTime;

        Vector3 currentPoint = FindPointOnParabola(currentTime);
        Vector3 nextPoint = FindPointOnParabola(nextTime);

        if (prevTime > 0)
        {
            Vector3 prevPoint = FindPointOnParabola(prevTime);
            if (CastRayBetweenPoints(currentPoint, prevPoint, out hit))
            {
                OnHit(hit, currentPoint - prevPoint);
            }
        }

        if(CastRayBetweenPoints(currentPoint, nextPoint, out hit))
        {
            OnHit(hit, nextPoint - currentPoint);
        }
    }

    private void Update()
    {
        if (!isInitialized || startTime < 0) return;

        float currentTime = Time.time - startTime;
        Vector3 currentPoint = FindPointOnParabola(currentTime);
        transform.position = currentPoint;
    }

    private Vector3 FindPointOnParabola(float time)
    {
        Vector3 point = startPos + (startForward * speed * time);
        Vector3 gravityVec = Vector3.down * gravity * time * time;
        return point + gravityVec;
    }

    private bool CastRayBetweenPoints(Vector3 startPoint, Vector3 endPoint, out RaycastHit hit)
    {
        return Physics.Raycast(startPoint, endPoint - startPoint, out hit, (endPoint - startPoint).magnitude, layerMask);
    }
}
