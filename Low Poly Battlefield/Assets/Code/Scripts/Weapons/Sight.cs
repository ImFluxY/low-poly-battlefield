using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Sight", menuName = "Weapon/Sight", order = 1)]
public class Sight : ScriptableObject
{
    public GameObject sightPrefab;
    public float switchSpeed = .2f;
    [Header("Primary Sight")]
    public SightRatio primarySightRatio;
    public Vector3 aimingOffsetPos;
    public Vector3 aimingOffsetRot;
    [Header("Secondary Sight")]
    public SightRatio secondarySightRatio;
    public bool twoSights;
    public Vector3 secondeAimingOffsetPos;
    public Vector3 secondeAimingOffsetRot;
    [Header("Canted Sight")]
    public SightRatio cantedSightRatio;
    public Vector3 cantedAimingOffsetPos;
    public Vector3 cantedAimingOffsetRot;
}

public enum SightRatio
{
    x1,
    x2,
    x4,
    x6,
    x8,
    x16,
    x32
}