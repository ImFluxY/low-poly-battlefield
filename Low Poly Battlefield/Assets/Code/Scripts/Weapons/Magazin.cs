using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Magazin", menuName = "Weapon/Magazin", order = 1)]
public class Magazin : ScriptableObject
{
    public WeaponProperties weapon;
    public MagazinType type;
    public GameObject magPrefab;
    public GameObject bulletPrefab;
    public int bulletDamage;
    public float bulletSpeed = 900;
    public float bulletLifeTime = 10;
    public int maxAmmoCount = 30;
}

public enum MagazinType
{
    RifleMag,
    PistolMag
}