using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Attachement", menuName = "Weapon/Attachement", order = 1)]
public class Attachement : ScriptableObject
{
    public GameObject attachementPrefab;
    public AttachementType attachementType;
}

public enum AttachementType
{
    Underbarrel,
    Upperbarrel,
    RightSide,
    LeftSide,
    Muzzle,
    Stock
}
