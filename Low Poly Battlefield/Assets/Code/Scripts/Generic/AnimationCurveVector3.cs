using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Animation Curve Vector3", menuName = "Generic/Vector3Curve", order = 1)]
public class AnimationCurveVector3 : ScriptableObject
{
    public AnimationCurve xPos;
    public AnimationCurve yPos;
    public AnimationCurve zPos;
    [Space]
    public AnimationCurve xRot;
    public AnimationCurve yRot;
    public AnimationCurve zRot;
    [Space]
    public float lerpTime = 1f;
    public bool posOffset;
    public bool rotOffset;
}
