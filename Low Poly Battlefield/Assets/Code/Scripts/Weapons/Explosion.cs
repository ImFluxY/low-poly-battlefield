using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float radiusEffect = 4.5f;
    public float radiusDamage = 150.0f;
    [Range(0, 100)]
    public float dismemberingDistancePercent = 1f;
    public float explostionForce = 500f;

    private void OnEnable()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, radiusEffect);

        foreach (Collider col in colls)
        {
            if(Physics.Raycast(transform.position, col.transform.localPosition, radiusEffect))
            {
                Debug.DrawRay(transform.position, col.transform.localPosition, Color.green);

                float distance = Vector3.Distance(col.transform.position, transform.position);
                float damage = Mathf.InverseLerp(radiusEffect, 0, distance) * radiusDamage;
                //Debug.Log("The object " + col.name + ", at a distance of " + distance + " meter(s), this explosion will give " + damage + " damages");

                if (damage > 0)
                {
                    if (col.TryGetComponent(out CharacterPart characterPart))
                    {
                        //characterPart.OnHit(damage);
                    }

                    if (col.TryGetComponent(out Rigidbody rigidbody) && rigidbody.isKinematic == false)
                        rigidbody.AddForce((rigidbody.position - transform.position) * explostionForce, ForceMode.Force);

                    if (col.transform.parent != null) {
                        if (col.transform.parent.TryGetComponent(out TargetReaction target))
                            target.Hit();
                    }

                    if (distance <= radiusEffect * (dismemberingDistancePercent / 100))
                    {
                        if (col.transform.root.TryGetComponent(out PlayerController healthController))
                        {
                            //healthController.Dismemberment();
                        }
                    }
                }
            }
            else
            {
                Debug.DrawRay(transform.position, col.transform.localPosition, Color.red);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, radiusEffect);
    }
}
