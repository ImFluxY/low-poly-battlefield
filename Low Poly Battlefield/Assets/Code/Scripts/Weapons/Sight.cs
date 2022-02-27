using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Sight", menuName = "Weapon/Sight", order = 1)]
public class Sight : ScriptableObject
{
    public GameObject sightPrefab;
    public GameObject fakeSightPrefab;
    public float switchSpeed = .2f;
    [Header("Primary Sight")]
    public SightRatio primarySightRatio;
    public SightOffsets[] primaryAimingOffset;
    [Header("Secondary Sight")]
    public SightRatio secondarySightRatio;
    public bool twoSights;
    public SightOffsets[] secondaryAimingOffset;
    [Header("Canted Sight")]
    public SightRatio cantedSightRatio;
    public SightOffsets[] cantedAimingOffset;
}

[System.Serializable]
public class SightOffsets{
    public WeaponProperties weapon;
    public bool needRail;
    public Vector3 position;
    public Vector3 rotation;
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