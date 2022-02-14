using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class IKFoots : MonoBehaviour
{
    [SerializeField]
    private Transform rightFootTransform;
    [SerializeField]
    private Transform leftFootTransform;
    private Transform[] allFootTransforms;
    [Space]
    [SerializeField]
    private Transform rightFootTarget;
    [SerializeField]
    private Transform leftFootTarget;
    private Transform[] allTargetTransforms;
    [Space]
    [SerializeField]
    private GameObject rightFootRig;
    [SerializeField]
    private GameObject leftFootRig;
    private TwoBoneIKConstraint[] allFootRigs;

    [SerializeField]
    private float maxHitDistance = 1f;
    [SerializeField]
    private float addedHeight = .5f;
    [SerializeField]
    private float yOffset = 0.06f;

    private LayerMask groundLayerMask;
    private bool[] allGroundSpherecastHits;
    private LayerMask hitLayer;
    private Vector3[] allHitsNormals;
    private float angleAboutX;
    private float angleAboutZ;

    private Animator animator;
    private float[] allFootWeights;

    // Start is called before the first frame update
    void Start()
    {
        allFootTransforms = new Transform[2];
        allFootTransforms[0] = rightFootTransform;
        allFootTransforms[1] = leftFootTransform;

        allTargetTransforms = new Transform[2];
        allTargetTransforms[0] = rightFootTarget;
        allTargetTransforms[1] = leftFootTarget;

        allFootRigs = new TwoBoneIKConstraint[2];
        allFootRigs[0] = rightFootRig.GetComponent<TwoBoneIKConstraint>();
        allFootRigs[1] = leftFootRig.GetComponent<TwoBoneIKConstraint>();

        allGroundSpherecastHits = new bool[3];
        allHitsNormals = new Vector3[2];

        groundLayerMask = LayerMask.NameToLayer("Environnement");

        animator = GetComponent<Animator>();
        allFootWeights = new float[2];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PlaceCharacterFeet();
    }

    private void CheckGroundBelow(out Vector3 hitPoint, out bool gotGroundSpherecastHit, out Vector3 hitNormal, out LayerMask hitLayer,
        out float currentHitDistance, Transform objectTransform, int checkForLayerMask, float maxHitDst, float addedHeight)
    {
        RaycastHit hit;
        Vector3 startSpherecast = objectTransform.position + new Vector3(0f, addedHeight, 0f);

        if(checkForLayerMask == -1)
        {
            Debug.LogError("Layer does not exist !");
            gotGroundSpherecastHit = false;
            currentHitDistance = 0f;
            hitLayer = LayerMask.NameToLayer("Player");
            hitNormal = Vector3.up;
            hitPoint = objectTransform.position;
        }
        else
        {
            int layerMask = (1 << checkForLayerMask);
            if (Physics.SphereCast(startSpherecast, .2f, Vector3.down, out hit, maxHitDst, layerMask, QueryTriggerInteraction.UseGlobal))
            {
                hitLayer = hit.transform.gameObject.layer;
                currentHitDistance = hit.distance - addedHeight;
                hitNormal = hit.normal;
                gotGroundSpherecastHit = true;
                hitPoint = hit.point;
            }
            else
            {
                gotGroundSpherecastHit = false;
                currentHitDistance = 0f;
                hitLayer = LayerMask.NameToLayer("Player");
                hitNormal = Vector3.up;
                hitPoint = objectTransform.position;
            }
        }
    }

    Vector3 ProjectOnContactPlane(Vector3 vector, Vector3 hitNormal)
    {
        return vector - hitNormal * Vector3.Dot(vector, hitNormal);
    }

    private void ProjectedAxisAngles(out float angleAboutX, out float angleAboutZ, Transform footTargetTransform, Vector3 hitNormal)
    {
        Vector3 xAxisProjected = ProjectOnContactPlane(footTargetTransform.forward, hitNormal).normalized;
        Vector3 zAxisProjected = ProjectOnContactPlane(footTargetTransform.right, hitNormal).normalized;

        angleAboutX = Vector3.SignedAngle(footTargetTransform.forward, xAxisProjected, footTargetTransform.right);
        angleAboutZ = Vector3.SignedAngle(footTargetTransform.right, zAxisProjected, footTargetTransform.forward);
    }

    private void PlaceCharacterFeet()
    {
        allFootWeights[0] = animator.GetFloat("Right Foot Weight");
        allFootWeights[1] = animator.GetFloat("Left Foot Weight");

        for (int i = 0; i < 2; i++)
        {
            allFootRigs[i].weight = allFootWeights[i];

            CheckGroundBelow(out Vector3 hitPoint, out allGroundSpherecastHits[i], out Vector3 hitNormal, out hitLayer, out _,
                allFootTransforms[i], groundLayerMask, maxHitDistance, addedHeight);
            allHitsNormals[i] = hitNormal;

            if(allGroundSpherecastHits[i] == true)
            {
                ProjectedAxisAngles(out angleAboutX, out angleAboutZ, allFootTransforms[i], allHitsNormals[i]);

                allTargetTransforms[i].position = new Vector3(allFootTransforms[i].position.x, hitPoint.y + yOffset, allFootTransforms[i].position.z);
                allTargetTransforms[i].rotation = allFootTransforms[i].rotation;
            }
            else
            {
                allTargetTransforms[i].position = allFootTransforms[i].position;
                allTargetTransforms[i].rotation = allFootTransforms[i].rotation;
            }
        }
    }
}
