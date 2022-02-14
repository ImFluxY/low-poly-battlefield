using UnityEngine;

[CreateAssetMenu(fileName = "Weapons Data", menuName = "Weapon/Create Weapon", order = 1)]
public class Weapon : ScriptableObject
{
    [Header("References")]
    public weaponType weaponType;
    public GameObject weaponObject;
    public Vector3 weaponPivotPos;
    public Sight defaultSight;
    public Magazin magazinType;
    public float caseEjectedDrestroyTime = 5f;
    public float fireRate = 0.13f;
    public float aimingDuration = 0.15f;
    public float muzzleFlashSpawnChance = 0.4f;
    public GameObject muzzleSmoke;
    public GameObject caseEjected;
    public float caseMaxEjectingForce = 8f;
    public float caseMinEjectingForce = 4f;
    [Space]
    public bool canSingle;
    public bool canFullAuto;
    public bool canBurst;
    public int bulletsPerBurst;

    [Header("Recoil")]
    public float positionDampTime = 6;
    public float rotationDampTime = 9;
    [Space(10)]
    public Vector3 recoilRotation;
    public Vector3 recoilKickBack;
    [Space(20)]
    public float aimRecoilReducer = 2f;
    [Space(5)]
    public float recoilRotComeBackTime = 35;
    public float recoilPosComeBackTime = 35;

    [Header("Procedural Animations")]
    public AnimationCurveVector3 idleAnimation;
    public AnimationCurveVector3 walkAnimation;
    public AnimationCurveVector3 runAnimation;

    [Header("Sway Settings")]
    /*
    public float weaponSwaySpeed = 1f;
    public float a = 1;
    public float b = 2;
    public float sizeReducerFactor = 10f;
    public float thetaIncreaseFactor = 0.01f;
    public float swayLerpSpeed = 15f;
    */
    [Header("Sway Settings")]
    public float swayAmount = 1;
    public float maxSwayAmount = 1;
    public float swayRotationAmount = 0.5f;
    public float maxSwayRoationAmount = 1;
    public float swaySmoothAmount = 10;
    public bool swayRotationX = true;
    public bool swayRotationY = true;
    public bool swayRotationZ = true;

    /*
    [Header("Clipping")]
    public float clippingDistance;
    public Vector3 leftClippingStartOffset;
    public Vector3 rightClippingStartOffset;
    public Vector3 maxClippingRotation = new Vector3(90, 0);
    */
}

public enum weaponType
{
    AssaultRifle,
    Pistol
}