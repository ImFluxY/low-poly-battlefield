using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Grenade", menuName = "Weapon/Create Grenade", order = 1)]
public class Grenade : ScriptableObject 
{
    public GrenadeType grenadeType;
    public GameObject grenadePrefab;
    public float throwForce = 5f;
    public GameObject runtimeGrenade;
}

public enum GrenadeType
{
    Explosive,
    Flash,
    Smoke
}
