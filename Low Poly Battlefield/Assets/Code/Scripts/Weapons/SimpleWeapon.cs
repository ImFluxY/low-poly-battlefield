using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Photon.Pun;
using System.IO;

public enum FireMode
{
    Semi,
    Auto,
    Burst
}

public class SimpleWeapon : MonoBehaviourPunCallbacks, IPunObservable
{
    public bool control;

    [Header("Debugging")]
    [SerializeField]
    private bool forceAiming;
    [SerializeField]
    private bool noRecoil;
    [SerializeField]
    private bool noAnimation;

    [Header("Weapon")]
    public Weapon weapon;
    [SerializeField]
    private Transform weaponAimingOffset;
    [SerializeField]
    private Transform weaponPivot;
    [SerializeField]
    private Transform weaponSway;
    [SerializeField]
    private Transform weaponParent;
    [SerializeField]
    private Transform weaponRecoil;
    [SerializeField]
    private Transform cameraRecoil;

    [Header("Procedural Animations")]
    [SerializeField]
    private Transform animationTransform;

    [Header("Customization")]
    public Texture camoTexture;
    [Space]
    public Sight sight;
    public Sight cantedSight;
    [SerializeField]
    private int aimingOffsetIndex;
    [SerializeField]
    private List<SightRatio> sightRations;
    [SerializeField]
    private List<Vector3> aimingOffsetPos = new List<Vector3>();
    [SerializeField]
    private List<Vector3> aimingOffsetRot = new List<Vector3>();
    [Space]
    public Attachement[] attachements;
    public List<GameObject> weaponGraphics;

    [Header("Fire Settings")]
    [SerializeField]
    private FireMode fireMode;

    [Header("Magazin")]
    public weaponMag currentMag;

    [Header("Clipping")]
    [SerializeField]
    private LayerMask clippingMask;
    [SerializeField]
    private Vector3 finalClippingRotation;

    [Header("Rig Settings")]
    [SerializeField]
    private Animator rigAnimator;
    [SerializeField]
    private WeaponAnimationEvents weaponAnimationEvent;
    [SerializeField]
    private Transform leftHandMagPos;
    [SerializeField]
    private Transform leftHandThrowPos;
    [SerializeField]
    private Rig aimRigLayer;

    //Private variables

    [HideInInspector]
    public bool isAiming;

    private float fireTimer = 0.0f;
    private bool bursting = false;
    private Vector3 CurrentRecoil1;
    private Vector3 CurrentRecoil2;
    private Vector3 weaponRotationOutput;

    private float m_theta;
    private Vector3 m_swayPos;
    private Vector3 refWeaponPos = Vector3.zero;
    private Quaternion refWeaponRot = Quaternion.identity;

    [HideInInspector]
    public WeaponReferences weaponRef;
    private Animator playerAnimator;
    private InputManager inputManager;
    private BallisticManager ballisticManager;
    private Equipement equipement;
    private PlayerLocomotion playerLocomotion;
    private PlayerCamera playerCamera;
    private int playerActor;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        inputManager = FindObjectOfType<InputManager>();
        ballisticManager = FindObjectOfType<BallisticManager>();
        equipement = GetComponent<Equipement>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerCamera = GetComponent<PlayerCamera>();
        playerActor = PhotonNetwork.LocalPlayer.ActorNumber;
        weaponAnimationEvent.weaponAnimationEvent.AddListener(OnAnimationEvent);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isAiming);
        }
        else if (stream.IsReading)
        {
            isAiming = (bool)stream.ReceiveNext();
        }
    }

    public void SetupWeapon()
    {
        //New Weapon
        weaponPivot.localPosition = weapon.weaponPivotPos;
        GameObject newWeapon = Instantiate(weapon.weaponObject, weaponParent) as GameObject;
        weaponGraphics.Add(newWeapon);
        weaponRef = newWeapon.GetComponent<WeaponReferences>();

        if (camoTexture)
        {
            for (int i = 0; i < weaponRef.meshCamos.Length; i++)
            {
                weaponRef.meshCamos[i].material.mainTexture = camoTexture;
            }
        }

        //Setup Attachements
        if (!sight)
            sight = weapon.defaultSight;

        if (PV.IsMine && sight.sightPrefab)
            weaponGraphics.Add(Instantiate(sight.sightPrefab, weaponRef.sightPos));
        else if (!PV.IsMine && sight.fakeSightPrefab)
            weaponGraphics.Add(Instantiate(sight.fakeSightPrefab, weaponRef.sightPos));

        aimingOffsetPos.Clear();
        aimingOffsetRot.Clear();
        sightRations.Clear();

        aimingOffsetPos.Add(sight.aimingOffsetPos);
        aimingOffsetRot.Add(sight.aimingOffsetRot);
        sightRations.Add(sight.primarySightRatio);

        if (sight.twoSights)
        {
            aimingOffsetPos.Add(sight.secondeAimingOffsetPos);
            aimingOffsetRot.Add(sight.secondeAimingOffsetRot);
            sightRations.Add(sight.secondarySightRatio);
        }

        if (cantedSight)
        {
            if (weaponRef.cantedRail)
                weaponRef.cantedRail.SetActive(true);

            weaponGraphics.Add(Instantiate(cantedSight.sightPrefab, weaponRef.cantedSightPos));

            aimingOffsetPos.Add(cantedSight.cantedAimingOffsetPos);
            aimingOffsetRot.Add(cantedSight.cantedAimingOffsetRot);
            sightRations.Add(sight.cantedSightRatio);
        }
        else
        {
            if (weaponRef.cantedRail)
                weaponRef.cantedRail.SetActive(false);
        }

        if (attachements.Length > 0)
        {
            for (int i = 0; i < attachements.Length; i++)
            {
                if (attachements[i] == null)
                    return;

                GameObject attachementPrefab = attachements[i].attachementPrefab;
                Transform attachementParent = null;

                switch (attachements[i].attachementType)
                {
                    case AttachementType.Upperbarrel:
                        if (weaponRef.upperBarrelPos != null)
                            attachementParent = weaponRef.upperBarrelPos;
                        break;
                    case AttachementType.Underbarrel:
                        if (weaponRef.underBarrelPos != null)
                            attachementParent = weaponRef.underBarrelPos;
                        break;
                    case AttachementType.RightSide:
                        if (weaponRef.rightSideBarrelPos != null)
                            attachementParent = weaponRef.rightSideBarrelPos;
                        break;
                    case AttachementType.LeftSide:
                        if (weaponRef.leftSideBarrelPos != null)
                            attachementParent = weaponRef.leftSideBarrelPos;
                        break;
                    case AttachementType.Muzzle:
                        if (weaponRef.muzzlePos != null)
                            attachementParent = weaponRef.muzzlePos;
                        break;
                    case AttachementType.Stock:
                        if (weaponRef.stockPos != null)
                            attachementParent = weaponRef.stockPos;
                        break;
                }

                if (attachementPrefab != null && attachementParent != null)
                {
                    weaponGraphics.Add(Instantiate(attachementPrefab, attachementParent));
                }
            }
        }

        //Procedural Animations

        proceduralStartPos = animationTransform.localPosition;
        proceduralStartRot = animationTransform.localRotation;
    }

    private void FixedUpdate()
    {
        if (!weapon)
            return;

        if (PV.IsMine && !Pause.paused)
        {
            MoveSway();
            TiltSway();
        }

        RecoilLogic();
    }

    private void Update()
    {
        if (!weapon)
            return;

        if (PV.IsMine && !Pause.paused)
        {
            AimingControl();

            if (control)
            {
                FireControl();
                AnimationControl();

                if (fireTimer < weapon.fireRate)
                    fireTimer += Time.deltaTime;
            }
        }

        AnimationLogic();
        AimingLogic();
    }

    private void FireControl()
    {
        switch (fireMode)
        {
            case FireMode.Semi:
                if (Input.GetKeyDown(inputManager.fireKey))
                    Shoot();
                break;
            case FireMode.Auto:
                if (fireTimer < weapon.fireRate) return;
                if (Input.GetKey(inputManager.fireKey))
                    Shoot();
                break;
            case FireMode.Burst:
                if (Input.GetKeyDown(inputManager.fireKey) && !bursting)
                {
                    StartCoroutine(BurstFire());
                    bursting = true;
                }
                break;
        }
    }

    private IEnumerator BurstFire()
    {
        for (int i = 0; i < weapon.bulletsPerBurst; i++)
        {
            Shoot();
            yield return new WaitForSeconds(weapon.fireRate);
        }

        bursting = false;
    }

    private void Shoot()
    {
        /* Mag */
        if (currentMag == null || currentMag.currentAmmoCount <= 0)
            return;

        currentMag.currentAmmoCount--;

        PV.RPC("RPC_Shoot", RpcTarget.All);

        /* Realistic Ballistic */

        GameObject bullet = Instantiate(currentMag.magProperties.bulletPrefab, weaponRef.muzzle.position, weaponRef.muzzle.rotation);
        ParabolicBullet bulletScript = bullet.GetComponent<ParabolicBullet>();
        if (bulletScript)
        {
            bulletScript.Initialize(ballisticManager, currentMag.magProperties.bulletDamage, weaponRef.muzzle, currentMag.magProperties.bulletSpeed, 9.81f, playerActor);
        }
        Destroy(bullet, currentMag.magProperties.bulletLifeTime);

        /* Fire Rate */
        fireTimer = 0.0f;
    }

    [PunRPC]
    private void RPC_Shoot()
    {
        /* Animation */
        weaponRef.animator.SetTrigger("Fire");

        /* Recoil */
        if (!noRecoil)
        {
            if (isAiming)
            {
                CurrentRecoil1 += new Vector3(weapon.recoilRotation.x / weapon.aimRecoilReducer, Random.Range(-weapon.recoilRotation.y, weapon.recoilRotation.y) / weapon.aimRecoilReducer, Random.Range(-weapon.recoilRotation.z, weapon.recoilRotation.z) / weapon.aimRecoilReducer);
                CurrentRecoil2 += new Vector3(Random.Range(-weapon.recoilKickBack.x, weapon.recoilKickBack.x) / weapon.aimRecoilReducer, Random.Range(-weapon.recoilKickBack.y, weapon.recoilKickBack.y) / weapon.aimRecoilReducer, weapon.recoilKickBack.z / weapon.aimRecoilReducer);
            }
            else
            {
                CurrentRecoil1 += new Vector3(weapon.recoilRotation.x, Random.Range(-weapon.recoilRotation.y, weapon.recoilRotation.y), Random.Range(-weapon.recoilRotation.z, weapon.recoilRotation.z));
                CurrentRecoil2 += new Vector3(Random.Range(-weapon.recoilKickBack.x, weapon.recoilKickBack.x), Random.Range(-weapon.recoilKickBack.y, weapon.recoilKickBack.y), weapon.recoilKickBack.z);
            }
        }

        /* FX */

        if (Random.Range(0f, 1f) <= weapon.muzzleFlashSpawnChance)
        {
            weaponRef.muzzleFlash.Play();
        }

        GameObject caseEjected = Instantiate(weapon.caseEjected, weaponRef.caseEjector.position, weaponRef.caseEjector.rotation);
        Rigidbody caseEjectedRb = caseEjected.GetComponent<Rigidbody>();
        caseEjectedRb.AddForce(caseEjected.transform.right * Random.Range(weapon.caseMinEjectingForce, weapon.caseMaxEjectingForce), ForceMode.Impulse);
        Vector3 randomRotation = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        caseEjectedRb.rotation = Quaternion.Euler(randomRotation);
        Destroy(caseEjected, weapon.caseEjectedDrestroyTime);
    }

    private void RecoilLogic()
    {
        if (!Input.GetMouseButton(0) || currentMag.currentAmmoCount <= 0)
        {
            CurrentRecoil1 = Vector3.Lerp(CurrentRecoil1, Vector3.zero, weapon.recoilRotComeBackTime * Time.fixedDeltaTime);
        }
        CurrentRecoil2 = Vector3.Lerp(CurrentRecoil2, Vector3.zero, weapon.recoilPosComeBackTime * Time.fixedDeltaTime);
        weaponRecoil.localPosition = Vector3.Slerp(weaponRecoil.localPosition, CurrentRecoil2, weapon.positionDampTime * Time.fixedDeltaTime);
        weaponRotationOutput = Vector3.Slerp(weaponRotationOutput, CurrentRecoil1, weapon.rotationDampTime * Time.fixedDeltaTime);
        weaponRecoil.localRotation = Quaternion.Euler(weaponRotationOutput);
    }

    private void AimingControl()
    {
        if (inputManager.Aiming && !inputManager.FreeLook || forceAiming)
            isAiming = true;
        else
            isAiming = false;

        if (isAiming && aimingOffsetPos.Count > 0)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            int previousIndex = aimingOffsetIndex;

            if (scroll > 0)
            {
                if (aimingOffsetIndex < aimingOffsetPos.Count - 1)
                {
                    aimingOffsetIndex++;
                }
                else
                {
                    aimingOffsetIndex = 0;
                }
            }

            if (scroll < 0)
            {
                if (aimingOffsetIndex > 0)
                {
                    aimingOffsetIndex--;
                }
                else
                {
                    aimingOffsetIndex = aimingOffsetPos.Count - 1;
                }
            }

            if (previousIndex != aimingOffsetIndex)
            {
                weaponAimingOffset.localPosition = Vector3.Lerp(weaponAimingOffset.localPosition, aimingOffsetPos[aimingOffsetIndex], Time.deltaTime / sight.switchSpeed);
                weaponAimingOffset.localRotation = Quaternion.Lerp(weaponAimingOffset.localRotation, Quaternion.Euler(aimingOffsetRot[aimingOffsetIndex]), Time.deltaTime / sight.switchSpeed);

            }
        }
    }

    private void AimingLogic()
    {
        if (isAiming)
        {
            inputManager.SetSightDivider(sightRations[aimingOffsetIndex]);
            weaponAimingOffset.localPosition = Vector3.Lerp(weaponAimingOffset.localPosition, aimingOffsetPos[aimingOffsetIndex], Time.deltaTime / weapon.aimingDuration);
            weaponAimingOffset.localRotation = Quaternion.Lerp(weaponAimingOffset.localRotation, Quaternion.Euler(aimingOffsetRot[aimingOffsetIndex]), Time.deltaTime / weapon.aimingDuration);
            aimRigLayer.weight = Mathf.Lerp(aimRigLayer.weight, 1, Time.deltaTime / weapon.aimingDuration);
        }
        else
        {
            inputManager.SetSightDivider(SightRatio.x1);
            weaponAimingOffset.localPosition = Vector3.Lerp(weaponAimingOffset.localPosition, Vector3.zero, Time.deltaTime / weapon.aimingDuration);
            weaponAimingOffset.localRotation = Quaternion.Lerp(weaponAimingOffset.localRotation, Quaternion.identity, Time.deltaTime / weapon.aimingDuration);
            aimRigLayer.weight = Mathf.Lerp(aimRigLayer.weight, 0, Time.deltaTime / weapon.aimingDuration);
        }
    }

    private void MoveSway()
    {
        float moveX = inputManager.FreeLook ? 0 : Mathf.Clamp(inputManager.XLookAxis * weapon.swayAmount, -weapon.maxSwayAmount, weapon.maxSwayAmount);
        float moveY = inputManager.FreeLook ? 0 : Mathf.Clamp(inputManager.YLookAxis * weapon.swayAmount, -weapon.maxSwayAmount, weapon.maxSwayAmount);

        Vector3 finalPosition = new Vector3(moveX, moveY, 0);
        weaponSway.localPosition = Vector3.Lerp(weaponSway.localPosition, finalPosition + refWeaponPos, Time.deltaTime * weapon.swaySmoothAmount);
    }

    Vector2 tilt;

    private void TiltSway()
    {
        if (isAiming)
        {
            tilt.x = inputManager.YLookAxis * weapon.swayRotationAmount;
            tilt.y = inputManager.XLookAxis * weapon.swayRotationAmount;
        }
        else
        {
            tilt.x += inputManager.YLookAxis * weapon.swayRotationAmount;
            tilt.y += inputManager.XLookAxis * weapon.swayRotationAmount;
        }

        tilt.x = inputManager.FreeLook ? 0 : Mathf.Clamp(tilt.x, -weapon.maxSwayRoationAmount, weapon.maxSwayRoationAmount);
        tilt.y = inputManager.FreeLook ? 0 : Mathf.Clamp(tilt.y, -weapon.maxSwayRoationAmount, weapon.maxSwayRoationAmount);

        Quaternion finalRotation = Quaternion.Euler(new Vector3(
            weapon.swayRotationX ? -tilt.x : 0f,
            weapon.swayRotationY ? tilt.y : 0f,
            weapon.swayRotationZ ? tilt.y : 0f));

        weaponSway.localRotation = Quaternion.Slerp(weaponSway.localRotation, finalRotation * refWeaponRot, Time.deltaTime * weapon.swaySmoothAmount);
    }

    private void AnimationControl()
    {
        if (noAnimation)
            return;

        if (Input.GetKeyDown(KeyCode.R) && equipement.magazins.Count > 0)
        {
            playerAnimator.SetTrigger("Reload");
            rigAnimator.SetTrigger("Reload");
        }

        if (Input.GetKeyDown(KeyCode.G) && equipement.SelectGrenade(GrenadeType.Explosive))
        {
            rigAnimator.SetTrigger("Throw");
        }

        if (Input.GetKeyDown(KeyCode.H) && equipement.SelectGrenade(GrenadeType.Smoke))
        {
            rigAnimator.SetTrigger("Throw");
        }
    }

    private void AnimationLogic()
    {
        if (noAnimation)
            return;

        switch (playerLocomotion.movementState)
        {
            case MovementState.Idle:
                ProceduralAnimation(weapon.idleAnimation);
                break;
            case MovementState.Walk:
                ProceduralAnimation(weapon.walkAnimation);
                break;
            case MovementState.Run:
                ProceduralAnimation(weapon.runAnimation);
                break;
        }
    }

    void OnAnimationEvent(string eventName)
    {
        switch (eventName)
        {
            case "detach_magazine":
                DetachMagazine();
                break;
            case "drop_magazine":
                DropMagazine();
                break;
            case "refill_magazine":
                RefillMagazine();
                break;
            case "attach_magazine":
                AttachMagazine();
                break;
            case "grenade_take":
                TakeGrenade();
                break;
            case "grenade_throw":
                ThrowGrenade();
                break;
        }
    }

    /* Reloading Logic */

    private void DetachMagazine()
    {
        currentMag.magazin.transform.SetParent(leftHandMagPos);
    }

    private void DropMagazine()
    {
        currentMag.magazin.GetComponent<Rigidbody>().isKinematic = false;
        currentMag.magazin.transform.SetParent(null);
    }

    private void RefillMagazine()
    {
        currentMag = equipement.magazins[equipement.magSelected];
        equipement.magazins.Remove(currentMag);

        if (currentMag.magazin != null)
        {
            currentMag.magazin.transform.SetParent(leftHandMagPos);
            currentMag.magazin.transform.localPosition = Vector3.zero;
            currentMag.magazin.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
        else
        {
            currentMag.magazin = Instantiate(weapon.magazinType.magPrefab, leftHandMagPos);
        }
    }

    private void AttachMagazine()
    {
        currentMag.magazin.transform.SetParent(weaponRef.magPos);
        currentMag.magazin.transform.localPosition = Vector3.zero;
        currentMag.magazin.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    private void TakeGrenade()
    {
        Debug.Log("Take Grenade");
        equipement.equippedGrenades[0].runtimeGrenade = Instantiate(equipement.equippedGrenades[equipement.selectedGrenade].grenadePrefab, leftHandThrowPos);
        equipement.equippedGrenades[0].runtimeGrenade.GetComponent<Rigidbody>().isKinematic = true;
    }

    private void ThrowGrenade()
    {
        Debug.Log("Throw Grenade");

        equipement.equippedGrenades[0].runtimeGrenade.transform.SetParent(null);
        Vector3 randomRotation = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        equipement.equippedGrenades[0].runtimeGrenade.transform.rotation = Quaternion.Euler(randomRotation);
        equipement.equippedGrenades[0].runtimeGrenade.GetComponent<Rigidbody>().isKinematic = false;
        equipement.equippedGrenades[0].runtimeGrenade.GetComponent<Rigidbody>().AddForce(
            (transform.forward + playerCamera.cameraPivot.forward) * equipement.equippedGrenades[equipement.selectedGrenade].throwForce);
        equipement.equippedGrenades.Remove(equipement.equippedGrenades[equipement.selectedGrenade]);
    }

    private float Normalize(float min, float max, float x)
    {
        float z;
        z = (x - min) / (max - min);
        return z;
    }

    private float proceduralAnimTimer = 0.0f;
    private float proceduralLerpRatio = 0.0f;
    private Vector3 proceduralStartPos;
    private Quaternion proceduralStartRot;

    private void ProceduralAnimation(AnimationCurveVector3 currentAnimation)
    {
        proceduralAnimTimer += Time.deltaTime;

        if (proceduralAnimTimer >= currentAnimation.lerpTime)
        {
            proceduralAnimTimer = 0;
        }

        if (currentAnimation.lerpTime > 0)
            proceduralLerpRatio = proceduralAnimTimer / currentAnimation.lerpTime;

        Vector3 pos = new Vector3(currentAnimation.xPos.Evaluate(proceduralLerpRatio), currentAnimation.yPos.Evaluate(proceduralLerpRatio), currentAnimation.zPos.Evaluate(proceduralLerpRatio));
        Vector3 rot = new Vector3(currentAnimation.xRot.Evaluate(proceduralLerpRatio), currentAnimation.yRot.Evaluate(proceduralLerpRatio), currentAnimation.zRot.Evaluate(proceduralLerpRatio));

        Vector3 posOffset = currentAnimation.posOffset ? proceduralStartPos : Vector3.zero;
        Quaternion rotOffset = currentAnimation.rotOffset ? proceduralStartRot : Quaternion.identity;

        animationTransform.localPosition = Vector3.Lerp(animationTransform.localPosition, (pos) + posOffset, proceduralLerpRatio);
        animationTransform.localRotation = Quaternion.Lerp(animationTransform.localRotation, Quaternion.Euler(rot) * rotOffset, proceduralLerpRatio);
    }

    public void detachWeapon()
    {
        weaponGraphics[0].GetComponent<Animator>().enabled = false;
        weaponGraphics[0].transform.SetParent(GetComponent<Animator>().GetBoneTransform(HumanBodyBones.RightHand));
    }
}

[System.Serializable]
public class weaponMag
{
    public Magazin magProperties;
    public Transform magazinSite;
    public GameObject magazin;
    public int currentAmmoCount;
}