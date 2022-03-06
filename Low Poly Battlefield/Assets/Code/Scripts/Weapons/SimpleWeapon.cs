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
  public EquippedWeapon weapon;
  [SerializeField]
  private Transform weaponAimingOffset;
  [SerializeField]
  private Transform weaponPivot;
  [SerializeField]
  private Transform weaponSway;
  public Transform weaponParent;
  [SerializeField]
  private Transform weaponRecoil;
  [SerializeField]
  private Transform cameraRecoil;

  [Header("Procedural Animations")]
  [SerializeField]
  private Transform animationTransform;

  [Header("Fire Settings")]
  [SerializeField]
  private FireMode fireMode;

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
  private Animator playerAnimator;
  private InputManager inputManager;
  private BallisticManager ballisticManager;
  private PlayerEquipement playerEquipement;
  private PlayerLocomotion playerLocomotion;
  private PlayerCamera playerCamera;
  private int playerActor;

  private void Awake()
  {
    playerAnimator = GetComponent<Animator>();
    inputManager = FindObjectOfType<InputManager>();
    ballisticManager = FindObjectOfType<BallisticManager>();
    playerLocomotion = GetComponent<PlayerLocomotion>();
    playerEquipement = GetComponent<PlayerEquipement>();
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
    //Weapon
    weaponPivot.localPosition = weapon.properties.weaponPivotPos;

    //Procedural Animations
    proceduralStartPos = animationTransform.localPosition;
    proceduralStartRot = animationTransform.localRotation;
  }

  private void FixedUpdate()
  {
    if (!weapon.properties)
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
    if (!weapon.properties)
      return;

    if (PV.IsMine && !Pause.paused)
    {
      AimingControl();

      if (control)
      {
        FireControl();
        AnimationControl();

        if (fireTimer < weapon.properties.fireRate)
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
        if (fireTimer < weapon.properties.fireRate) return;
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
    for (int i = 0; i < weapon.properties.bulletsPerBurst; i++)
    {
      Shoot();
      yield return new WaitForSeconds(weapon.properties.fireRate);
    }

    bursting = false;
  }

  private void Shoot()
  {
    /* Mag */
    if (weapon.weaponRef.currentMag == null || weapon.weaponRef.currentMag.getCurrentAmmo() <= 0)
      return;

    weapon.weaponRef.currentMag.setCurrentAmmo(weapon.weaponRef.currentMag.getCurrentAmmo() - 1);

    SoundManager.PlayWeaponSound(SoundManager.WeaponSound.Shot, weapon.properties.weaponProperties.weaponId);

    PV.RPC("RPC_Shoot", RpcTarget.All);

    /* Realistic Ballistic */

    GameObject bullet = Instantiate(weapon.weaponRef.currentMag.properties.bulletPrefab, weapon.weaponRef.muzzle.position, weapon.weaponRef.muzzle.rotation);
    ParabolicBullet bulletScript = bullet.GetComponent<ParabolicBullet>();
    if (bulletScript)
    {
      bulletScript.Initialize(ballisticManager, weapon.weaponRef.currentMag.properties.bulletDamage, weapon.weaponRef.muzzle, weapon.weaponRef.currentMag.properties.bulletSpeed, 9.81f, playerActor);
    }
    Destroy(bullet, weapon.weaponRef.currentMag.properties.bulletLifeTime);

    /* Fire Rate */
    fireTimer = 0.0f;
  }

  [PunRPC]
  private void RPC_Shoot()
  {
    /* Sound */
    if (!PV.IsMine)
      SoundManager.PlayWeaponSound(SoundManager.WeaponSound.Shot, weapon.properties.weaponProperties.weaponId, weapon.weaponRef.muzzle.position);

    /* Animation */
    weapon.weaponRef.animator.SetTrigger("Fire");

    /* Recoil */
    if (!noRecoil)
    {
      if (isAiming)
      {
        CurrentRecoil1 += new Vector3(weapon.properties.recoilRotation.x / weapon.properties.aimRecoilReducer, Random.Range(-weapon.properties.recoilRotation.y, weapon.properties.recoilRotation.y) / weapon.properties.aimRecoilReducer, Random.Range(-weapon.properties.recoilRotation.z, weapon.properties.recoilRotation.z) / weapon.properties.aimRecoilReducer);
        CurrentRecoil2 += new Vector3(Random.Range(-weapon.properties.recoilKickBack.x, weapon.properties.recoilKickBack.x) / weapon.properties.aimRecoilReducer, Random.Range(-weapon.properties.recoilKickBack.y, weapon.properties.recoilKickBack.y) / weapon.properties.aimRecoilReducer, weapon.properties.recoilKickBack.z / weapon.properties.aimRecoilReducer);
      }
      else
      {
        CurrentRecoil1 += new Vector3(weapon.properties.recoilRotation.x, Random.Range(-weapon.properties.recoilRotation.y, weapon.properties.recoilRotation.y), Random.Range(-weapon.properties.recoilRotation.z, weapon.properties.recoilRotation.z));
        CurrentRecoil2 += new Vector3(Random.Range(-weapon.properties.recoilKickBack.x, weapon.properties.recoilKickBack.x), Random.Range(-weapon.properties.recoilKickBack.y, weapon.properties.recoilKickBack.y), weapon.properties.recoilKickBack.z);
      }
    }

    /* FX */

    if (Random.Range(0f, 1f) <= weapon.properties.muzzleFlashSpawnChance)
    {
      weapon.weaponRef.muzzleFlash.Play();
    }

    GameObject caseEjected = Instantiate(weapon.properties.caseEjected, weapon.weaponRef.caseEjector.position, weapon.weaponRef.caseEjector.rotation);
    Rigidbody caseEjectedRb = caseEjected.GetComponent<Rigidbody>();
    caseEjectedRb.AddForce(caseEjected.transform.right * Random.Range(weapon.properties.caseMinEjectingForce, weapon.properties.caseMaxEjectingForce), ForceMode.Impulse);
    Vector3 randomRotation = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
    caseEjectedRb.rotation = Quaternion.Euler(randomRotation);
    Destroy(caseEjected, weapon.properties.caseEjectedDrestroyTime);
  }

  private void RecoilLogic()
  {
    if (!weapon.weaponRef.currentMag)
      return;

    if (!Input.GetMouseButton(0) || weapon.weaponRef.currentMag.getCurrentAmmo() <= 0)
    {
      CurrentRecoil1 = Vector3.Lerp(CurrentRecoil1, Vector3.zero, weapon.properties.recoilRotComeBackTime * Time.fixedDeltaTime);
    }
    CurrentRecoil2 = Vector3.Lerp(CurrentRecoil2, Vector3.zero, weapon.properties.recoilPosComeBackTime * Time.fixedDeltaTime);
    weaponRecoil.localPosition = Vector3.Slerp(weaponRecoil.localPosition, CurrentRecoil2, weapon.properties.positionDampTime * Time.fixedDeltaTime);
    weaponRotationOutput = Vector3.Slerp(weaponRotationOutput, CurrentRecoil1, weapon.properties.rotationDampTime * Time.fixedDeltaTime);
    weaponRecoil.localRotation = Quaternion.Euler(weaponRotationOutput);
  }

  private void AimingControl()
  {
    if (inputManager.Aiming && !inputManager.FreeLook || forceAiming)
      isAiming = true;
    else
      isAiming = false;

    if (isAiming && weapon.aimingOffsetPos.Count > 0)
    {
      float scroll = Input.GetAxis("Mouse ScrollWheel");
      int previousIndex = weapon.aimingOffsetIndex;

      if (scroll > 0)
      {
        if (weapon.aimingOffsetIndex < weapon.aimingOffsetPos.Count - 1)
        {
          weapon.aimingOffsetIndex++;
        }
        else
        {
          weapon.aimingOffsetIndex = 0;
        }
      }

      if (scroll < 0)
      {
        if (weapon.aimingOffsetIndex > 0)
        {
          weapon.aimingOffsetIndex--;
        }
        else
        {
          weapon.aimingOffsetIndex = weapon.aimingOffsetPos.Count - 1;
        }
      }

      if (previousIndex != weapon.aimingOffsetIndex)
      {
        weaponAimingOffset.localPosition = Vector3.Lerp(weaponAimingOffset.localPosition, weapon.aimingOffsetPos[weapon.aimingOffsetIndex], Time.deltaTime / weapon.sight.switchSpeed);
        weaponAimingOffset.localRotation = Quaternion.Lerp(weaponAimingOffset.localRotation, Quaternion.Euler(weapon.aimingOffsetRot[weapon.aimingOffsetIndex]), Time.deltaTime / weapon.sight.switchSpeed);

      }
    }
  }

  private void AimingLogic()
  {
    EquippedWeapon weapon = playerEquipement.equippedWeapons[playerEquipement.selectedWeapon];

    if (isAiming)
    {
      inputManager.SetSightDivider(weapon.sightRatios[weapon.aimingOffsetIndex]);
      weaponAimingOffset.localPosition = Vector3.Lerp(weaponAimingOffset.localPosition, weapon.aimingOffsetPos[weapon.aimingOffsetIndex], Time.deltaTime / weapon.properties.aimingDuration);
      weaponAimingOffset.localRotation = Quaternion.Lerp(weaponAimingOffset.localRotation, Quaternion.Euler(weapon.aimingOffsetRot[weapon.aimingOffsetIndex]), Time.deltaTime / weapon.properties.aimingDuration);
      aimRigLayer.weight = Mathf.Lerp(aimRigLayer.weight, 1, Time.deltaTime / weapon.properties.aimingDuration);
    }
    else
    {
      inputManager.SetSightDivider(SightRatio.x1);
      weaponAimingOffset.localPosition = Vector3.Lerp(weaponAimingOffset.localPosition, Vector3.zero, Time.deltaTime / weapon.properties.aimingDuration);
      weaponAimingOffset.localRotation = Quaternion.Lerp(weaponAimingOffset.localRotation, Quaternion.identity, Time.deltaTime / weapon.properties.aimingDuration);
      aimRigLayer.weight = Mathf.Lerp(aimRigLayer.weight, 0, Time.deltaTime / weapon.properties.aimingDuration);
    }
  }

  private void MoveSway()
  {
    float moveX = inputManager.FreeLook ? 0 : Mathf.Clamp(inputManager.XLookAxis * weapon.properties.swayAmount, -weapon.properties.maxSwayAmount, weapon.properties.maxSwayAmount);
    float moveY = inputManager.FreeLook ? 0 : Mathf.Clamp(inputManager.YLookAxis * weapon.properties.swayAmount, -weapon.properties.maxSwayAmount, weapon.properties.maxSwayAmount);

    Vector3 finalPosition = new Vector3(moveX, moveY, 0);
    weaponSway.localPosition = Vector3.Lerp(weaponSway.localPosition, finalPosition + refWeaponPos, Time.deltaTime * weapon.properties.swaySmoothAmount);
  }

  Vector2 tilt;

  private void TiltSway()
  {
    if (isAiming)
    {
      tilt.x = inputManager.YLookAxis * weapon.properties.swayRotationAmount;
      tilt.y = inputManager.XLookAxis * weapon.properties.swayRotationAmount;
    }
    else
    {
      tilt.x += inputManager.YLookAxis * weapon.properties.swayRotationAmount;
      tilt.y += inputManager.XLookAxis * weapon.properties.swayRotationAmount;
    }

    tilt.x = inputManager.FreeLook ? 0 : Mathf.Clamp(tilt.x, -weapon.properties.maxSwayRoationAmount, weapon.properties.maxSwayRoationAmount);
    tilt.y = inputManager.FreeLook ? 0 : Mathf.Clamp(tilt.y, -weapon.properties.maxSwayRoationAmount, weapon.properties.maxSwayRoationAmount);

    Quaternion finalRotation = Quaternion.Euler(new Vector3(
        weapon.properties.swayRotationX ? -tilt.x : 0f,
        weapon.properties.swayRotationY ? tilt.y : 0f,
        weapon.properties.swayRotationZ ? tilt.y : 0f));

    weaponSway.localRotation = Quaternion.Slerp(weaponSway.localRotation, finalRotation * refWeaponRot, Time.deltaTime * weapon.properties.swaySmoothAmount);
  }

  private void AnimationControl()
  {
    if (noAnimation)
      return;

    if (Input.GetKeyDown(KeyCode.R) && weapon.magazins.Count > 0)
    {
      playerAnimator.SetTrigger("Reload");
      rigAnimator.SetTrigger("Reload");
    }

    if (Input.GetKeyDown(KeyCode.G) && playerEquipement.SelectGrenade(GrenadeType.Explosive))
    {
      rigAnimator.SetTrigger("Throw");
    }

    if (Input.GetKeyDown(KeyCode.H) && playerEquipement.SelectGrenade(GrenadeType.Smoke))
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
        ProceduralAnimation(weapon.properties.idleAnimation);
        break;
      case MovementState.Walk:
        ProceduralAnimation(weapon.properties.walkAnimation);
        break;
      case MovementState.Run:
        ProceduralAnimation(weapon.properties.runAnimation);
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
    SoundManager.PlayWeaponSound(SoundManager.WeaponSound.Mag_Out, weapon.properties.weaponProperties.weaponId, weapon.weaponRef.currentMag.transform.position);
    weapon.weaponRef.currentMag.transform.SetParent(leftHandMagPos);
  }

  private void DropMagazine()
  {
    if(!weapon.weaponRef.currentMag)
      return;

    SoundManager.PlayWeaponSound(SoundManager.WeaponSound.Mag_Out, weapon.properties.weaponProperties.weaponId, weapon.weaponRef.currentMag.transform.position);
    weapon.weaponRef.currentMag.setRbKinematic(false);
    weapon.weaponRef.currentMag.transform.SetParent(null);
  }

  private void RefillMagazine()
  {
    weapon.weaponRef.currentMag = weapon.magazins[0].magazinObj;
    weapon.magazins.Remove(weapon.magazins[0]);

    if (weapon.weaponRef.currentMag != null)
    {
      weapon.weaponRef.currentMag.transform.SetParent(leftHandMagPos);
      weapon.weaponRef.currentMag.transform.localPosition = Vector3.zero;
      weapon.weaponRef.currentMag.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
    else
    {
      weapon.weaponRef.currentMag = Instantiate(weapon.magazins[0].magazin.magPrefab, leftHandMagPos).GetComponent<WeaponMagazin>();
    }
  }

  private void AttachMagazine()
  {
    SoundManager.PlayWeaponSound(SoundManager.WeaponSound.Mag_In, weapon.properties.weaponProperties.weaponId, weapon.weaponRef.currentMag.transform.position);
    weapon.weaponRef.currentMag.transform.SetParent(weapon.weaponRef.magPos);
    weapon.weaponRef.currentMag.transform.localPosition = Vector3.zero;
    weapon.weaponRef.currentMag.transform.localRotation = Quaternion.Euler(Vector3.zero);
  }

  private void TakeGrenade()
  {
    Debug.Log("Take Grenade");
    playerEquipement.equippedGrenades[0].runtimeGrenade = Instantiate(playerEquipement.equippedGrenades[playerEquipement.selectedGrenade].grenadePrefab, leftHandThrowPos);
    playerEquipement.equippedGrenades[0].runtimeGrenade.GetComponent<Rigidbody>().isKinematic = true;
  }

  private void ThrowGrenade()
  {
    Debug.Log("Throw Grenade");

    playerEquipement.equippedGrenades[0].runtimeGrenade.transform.SetParent(null);
    Vector3 randomRotation = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
    playerEquipement.equippedGrenades[0].runtimeGrenade.transform.rotation = Quaternion.Euler(randomRotation);
    playerEquipement.equippedGrenades[0].runtimeGrenade.GetComponent<Rigidbody>().isKinematic = false;
    playerEquipement.equippedGrenades[0].runtimeGrenade.GetComponent<Rigidbody>().AddForce(
        (transform.forward + playerCamera.cameraPivot.forward) * playerEquipement.equippedGrenades[playerEquipement.selectedGrenade].throwForce);
    playerEquipement.equippedGrenades.Remove(playerEquipement.equippedGrenades[playerEquipement.selectedGrenade]);
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
    weapon.weaponGraphics[0].GetComponent<Animator>().enabled = false;
    weapon.weaponGraphics[0].transform.SetParent(GetComponent<Animator>().GetBoneTransform(HumanBodyBones.RightHand));
  }
}