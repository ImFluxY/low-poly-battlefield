using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipement : MonoBehaviour
{
  public List<MagazinSite> magazinSites;
  [Space]
  public SimpleWeapon currentWeapon;
  public int selectedWeapon = 0;
  public List<EquippedWeapon> equippedWeapons;
  [Space]
  public int selectedGrenade = 0;
  public List<Grenade> equippedGrenades;
  private Weapon currentWeaponProperties;

  private PhotonView PV;

  private void Start()
  {
    PV = GetComponent<PhotonView>();
    currentWeapon = GetComponent<SimpleWeapon>();
    InitializeWeapons();
    SetupMags();
  }

  private void Update()
  {
    if (!PV.IsMine)
      return;

    //Debug.Log("Count " + equippedWeapons.Count);
    if (equippedWeapons.Count <= 0)
      return;

    if (Input.GetKeyDown(KeyCode.F))
    {
      CycleWeaponIndex();
      UnequipeWeapon(equippedWeapons[selectedWeapon]);
      EquipeWeapon();
      PV.RPC("RPC_SwitchWeapon", RpcTarget.All);
    }
  }
  [PunRPC]
  private void RPC_SwitchWeapon()
  {
    if (PV.IsMine)
      return;

    CycleWeaponIndex();
    UnequipeWeapon(equippedWeapons[selectedWeapon]);
    EquipeWeapon();
  }

  private void CycleWeaponIndex()
  {
    if (selectedWeapon < equippedWeapons.Count - 1)
    {
      selectedWeapon++;
    }
    else
    {
      selectedWeapon = 0;
    }
  }

  public bool SelectGrenade(GrenadeType grenadeType)
  {
    for (int i = 0; i < equippedGrenades.Count; i++)
    {
      if (equippedGrenades[i].grenadeType == grenadeType)
      {
        Debug.Log(equippedGrenades[i].grenadeType);

        selectedGrenade = i;
        return true;
      }
    }

    return false;
  }

  private void ReinitializeWeapon(EquippedWeapon weapon)
  {
    for (int i = 0; i < weapon.weaponGraphics.Count; i++)
    {
      Destroy(weapon.weaponGraphics[i]);
    }

    weapon.weaponGraphics.Clear();
  }

  private void InitializeWeapons()
  {
    foreach (EquippedWeapon equippedWeapon in equippedWeapons)
    {
      GameObject newWeapon = Instantiate(equippedWeapon.properties.weaponObject, currentWeapon.weaponParent) as GameObject;
      equippedWeapon.weaponRef = newWeapon.GetComponent<WeaponReferences>();
      equippedWeapon.weaponGraphics.Add(newWeapon);

      if (equippedWeapon.camoTexture)
      {
        for (int i = 0; i < equippedWeapon.weaponRef.meshCamos.Length; i++)
        {
          equippedWeapon.weaponRef.meshCamos[i].material.mainTexture = equippedWeapon.camoTexture;
        }
      }

      //Setup Attachements
      if (!equippedWeapon.sight)
        equippedWeapon.sight = equippedWeapon.properties.defaultSight;

      if (PV.IsMine && equippedWeapon.sight.sightPrefab)
        equippedWeapon.weaponGraphics.Add(Instantiate(equippedWeapon.sight.sightPrefab, equippedWeapon.weaponRef.sightPos));
      else if (!PV.IsMine && equippedWeapon.sight.fakeSightPrefab)
        equippedWeapon.weaponGraphics.Add(Instantiate(equippedWeapon.sight.fakeSightPrefab, equippedWeapon.weaponRef.sightPos));

      equippedWeapon.aimingOffsetPos.Clear();
      equippedWeapon.aimingOffsetRot.Clear();
      equippedWeapon.sightRatios.Clear();

      foreach (SightOffsets primarySight in equippedWeapon.sight.primaryAimingOffset)
      {
        if (primarySight.weapon.weaponId == equippedWeapon.properties.weaponProperties.weaponId)
        {
          if (primarySight.needRail && equippedWeapon.weaponRef.rail)
            equippedWeapon.weaponRef.rail.SetActive(true);

          equippedWeapon.aimingOffsetPos.Add(primarySight.position);
          equippedWeapon.aimingOffsetRot.Add(primarySight.rotation);
        }
      }
      equippedWeapon.sightRatios.Add(equippedWeapon.sight.primarySightRatio);

      if (equippedWeapon.sight.twoSights)
      {
        foreach (SightOffsets secondarySight in equippedWeapon.sight.secondaryAimingOffset)
        {
          if (secondarySight.weapon.weaponId == equippedWeapon.properties.weaponProperties.weaponId)
          {
            equippedWeapon.aimingOffsetPos.Add(secondarySight.position);
            equippedWeapon.aimingOffsetRot.Add(secondarySight.rotation);
          }
        }
        equippedWeapon.sightRatios.Add(equippedWeapon.sight.secondarySightRatio);
      }

      if (equippedWeapon.cantedSight)
      {
        if (equippedWeapon.weaponRef.cantedRail)
          equippedWeapon.weaponRef.cantedRail.SetActive(true);

        equippedWeapon.weaponGraphics.Add(Instantiate(equippedWeapon.cantedSight.sightPrefab, equippedWeapon.weaponRef.cantedSightPos));

        foreach (SightOffsets cantedSight in equippedWeapon.sight.cantedAimingOffset)
        {
          if (cantedSight.weapon.weaponId == equippedWeapon.properties.weaponProperties.weaponId)
          {
            equippedWeapon.aimingOffsetPos.Add(cantedSight.position);
            equippedWeapon.aimingOffsetRot.Add(cantedSight.rotation);
          }
        }
        equippedWeapon.sightRatios.Add(equippedWeapon.sight.cantedSightRatio);
      }
      else
      {
        if (equippedWeapon.weaponRef.cantedRail)
          equippedWeapon.weaponRef.cantedRail.SetActive(false);
      }

      if (equippedWeapon.attachements.Length > 0)
      {
        for (int i = 0; i < equippedWeapon.attachements.Length; i++)
        {
          if (equippedWeapon.attachements[i] == null)
            return;

          GameObject attachementPrefab = equippedWeapon.attachements[i].attachementPrefab;
          Transform attachementParent = null;

          switch (equippedWeapon.attachements[i].attachementType)
          {
            case AttachementType.Upperbarrel:
              if (equippedWeapon.weaponRef.upperBarrelPos != null)
                attachementParent = equippedWeapon.weaponRef.upperBarrelPos;
              break;
            case AttachementType.Underbarrel:
              if (equippedWeapon.weaponRef.underBarrelPos != null)
                attachementParent = equippedWeapon.weaponRef.underBarrelPos;
              break;
            case AttachementType.RightSide:
              if (equippedWeapon.weaponRef.rightSideBarrelPos != null)
                attachementParent = equippedWeapon.weaponRef.rightSideBarrelPos;
              break;
            case AttachementType.LeftSide:
              if (equippedWeapon.weaponRef.leftSideBarrelPos != null)
                attachementParent = equippedWeapon.weaponRef.leftSideBarrelPos;
              break;
            case AttachementType.Muzzle:
              if (equippedWeapon.weaponRef.muzzlePos != null)
                attachementParent = equippedWeapon.weaponRef.muzzlePos;
              break;
            case AttachementType.Stock:
              if (equippedWeapon.weaponRef.stockPos != null)
                attachementParent = equippedWeapon.weaponRef.stockPos;
              break;
          }

          if (attachementPrefab != null && attachementParent != null)
          {
            equippedWeapon.weaponGraphics.Add(Instantiate(attachementPrefab, attachementParent));
          }
        }
      }
    }

    EquipeWeapon();
  }

  private void SetupMags()
  {
    foreach (EquippedWeapon weapon in equippedWeapons)
    {
      for (int i = 0; i < weapon.magazins.Count; i++)
      {
        for (int j = 0; j < magazinSites.Count; j++)
        {
          Debug.Log(weapon.properties.weaponProperties.weaponName + ", " + i + ", " + j);

          if (magazinSites[j].magazinSiteTransform && !magazinSites[j].mag)
          {
            Debug.Log("Put a mag in this one");

            weapon.magazins[i].magazinObj = Instantiate(weapon.magazins[i].magazin.magPrefab, magazinSites[j].magazinSiteTransform).GetComponent<WeaponMagazin>();
            weapon.magazins[i].magazinObj.setCurrentAmmo(weapon.magazins[i].magazin.maxAmmoCount);
            weapon.magazins[i].magazinObj.properties = weapon.magazins[i].magazin;
            magazinSites[j].mag = weapon.magazins[i].magazinObj;
            break;
          }
        }
      }
    }
  }

  /*
    public void ChangeMagIndex()
    {
      if (magSelected >= magazins.Count)
      {
        magSelected = 0;
      }
      else
      {
        magSelected++;
      }

    }
    */

  private void EquipeWeapon()
  {
    for (int i = 0; i < equippedWeapons.Count; i++)
    {
      if (i == selectedWeapon)
      {
        equippedWeapons[i].weaponGraphics[0].transform.SetParent(currentWeapon.weaponParent, false);
        equippedWeapons[i].weaponGraphics[0].SetActive(true);
        currentWeapon.weapon = equippedWeapons[i];
        currentWeapon.SetupWeapon();
      }
      else
      {
        UnequipeWeapon(equippedWeapons[i]);
      }
    }
  }

  private void UnequipeWeapon(EquippedWeapon weapon)
  {
    weapon.weaponGraphics[0].SetActive(false);
  }
}

[System.Serializable]
public class EquippedWeapon
{
  public Weapon properties;
  public List<GameObject> weaponGraphics;
  public WeaponReferences weaponRef;
  public Sight sight;
  public Sight cantedSight;
  public Texture camoTexture;
  public Attachement[] attachements;
  public int aimingOffsetIndex;
  public List<SightRatio> sightRatios;
  public List<Vector3> aimingOffsetPos = new List<Vector3>();
  public List<Vector3> aimingOffsetRot = new List<Vector3>();
  public List<EquippedMagazin> magazins;
}

[System.Serializable]
public class MagazinSite
{
  public Transform magazinSiteTransform;
  public MagazinType magazinType;
  public WeaponMagazin mag;
}

[System.Serializable]
public class EquippedMagazin
{
  public Magazin magazin;
  public WeaponMagazin magazinObj;
}