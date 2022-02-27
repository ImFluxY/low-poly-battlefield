using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Data", menuName = "Weapon/Weapon Properties", order = 1)]
public class WeaponProperties : ScriptableObject
{
    public string weaponName;
    public int weaponId;
    public weaponTeam weaponTeam;
    public weaponType weaponType;
}

public enum weaponTeam{
    Army,
    Terrorist,
    Both
}