using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCurveDebug : MonoBehaviour
{
    [SerializeField]
    private AnimationCurveVector3 animationCurves;

    private Vector3 startPos;
    private Quaternion startRot;

    private float timer = 0.0f;
    private float lerpRatio = 0.0f;

    private void Start()
    {
        startPos = transform.localPosition;
        startRot = transform.localRotation;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if(timer > animationCurves.lerpTime)
        {
            timer = 0;
        }

        if(animationCurves.lerpTime > 0)
            lerpRatio = timer / animationCurves.lerpTime;

        Vector3 pos = new Vector3(animationCurves.xPos.Evaluate(lerpRatio), animationCurves.yPos.Evaluate(lerpRatio), animationCurves.zPos.Evaluate(lerpRatio));
        Vector3 rot = new Vector3(animationCurves.xRot.Evaluate(lerpRatio), animationCurves.yRot.Evaluate(lerpRatio), animationCurves.zRot.Evaluate(lerpRatio));

        Vector3 posOffset = animationCurves.posOffset ? startPos : Vector3.zero;
        Quaternion rotOffset = animationCurves.rotOffset ? startRot : Quaternion.identity;

        transform.localPosition = Vector3.Lerp(transform.localPosition, pos + posOffset, lerpRatio);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(rot) * rotOffset, lerpRatio);
    }
}
