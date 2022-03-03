using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipement : MonoBehaviour
{
  public List<WeaponHolder> weaponHolders;
  public List<MagazinSite> magazinSites;
  [Space]
  public SimpleWeapon currentWeapon;
  public int selectedWeapon = 0;
  public List<EquippedWeapon> equippedWeapons;
  [Space]
  public int selectedGrenade = 0;
  public List<Grenade> equippedGrenades;
  [Space]
  public int magSelected = 0;
  public List<weaponMag> magazins;
  private Weapon currentWeaponProperties;

  [SerializeField]
  private int team;
  private PhotonView PV;

  private void Start()
  {
    PV = GetComponent<PhotonView>();
    currentWeapon = GetComponent<SimpleWeapon>();
    InitializeWeapon();
    SetupMags();
  }

  private void Update()
  {
    if(!PV.IsMine)
      return;

    //Debug.Log("Count " + equippedWeapons.Count);
    if (equippedWeapons.Count <= 0)
      return;

    if (Input.GetKeyDown(KeyCode.F))
    {
      CycleWeaponIndex();
      ReinitializeWeapon();
      InitializeWeapon();
      PV.RPC("RPC_SwitchWeapon", RpcTarget.All);
    }
  }
  [PunRPC]
  private void RPC_SwitchWeapon()
  {
    if (PV.IsMine)
      return;

    CycleWeaponIndex();
    ReinitializeWeapon();
    InitializeWeapon();
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

  private void ReinitializeWeapon()
  {
    for (int i = 0; i < currentWeapon.weaponGraphics.Count; i++)
    {
      Destroy(currentWeapon.weaponGraphics[i]);
    }

    currentWeapon.weaponGraphics.Clear();
  }

  private void InitializeWeapon()
  {
    if (currentWeapon == null)
      currentWeapon = GetComponent<SimpleWeapon>();

    currentWeaponProperties = equippedWeapons[selectedWeapon].weapon;
    currentWeapon.weapon = currentWeaponProperties;

    //Setup Weapon Customization
    currentWeapon.sight = equippedWeapons[selectedWeapon].sight;
    currentWeapon.cantedSight = equippedWeapons[selectedWeapon].cantedSight;
    currentWeapon.camoTexture = equippedWeapons[selectedWeapon].camoTexture;
    currentWeapon.attachements = equippedWeapons[selectedWeapon].attachements;

    currentWeapon.SetupWeapon();

    //Setup Weapon Mag

    if (currentWeapon.weaponRef.currentMag.magProperties == null)
    {
      currentWeapon.weaponRef.currentMag.currentAmmoCount = currentWeaponProperties.magazinType.maxAmmoCount;
      currentWeapon.weaponRef.currentMag.magProperties = currentWeaponProperties.magazinType;
      currentWeapon.weaponRef.currentMag.magazin = Instantiate(currentWeaponProperties.magazinType.magPrefab, currentWeapon.weaponRef.magPos);
    }
  }


  private void SetupMags()
  {
    for (int i = 0; i < magazinSites.Count; i++)
    {
      if (!magazinSites[i].magazinType)
        return;

      weaponMag newMag = new weaponMag();
      newMag.currentAmmoCount = magazinSites[i].magazinType.maxAmmoCount;
      newMag.magProperties = magazinSites[i].magazinType;
      newMag.magazin = Instantiate(magazinSites[i].magazinType.magPrefab, magazinSites[i].magazinSiteTransform);
      magazins.Add(newMag);
    }
  }

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
}

[System.Serializable]
public class EquippedWeapon
{
  public Weapon weapon;
  public Sight sight;
  public Sight cantedSight;
  public Texture camoTexture;
  public Attachement[] attachements;

  public EquippedWeapon(Weapon w, Sight s, Sight cs, Texture t, Attachement[] a)
  {
    this.weapon = w;
    this.sight = s;
    this.cantedSight = cs;
    this.camoTexture = t;
    this.attachements = a;
  }
}

[System.Serializable]
public class MagazinSite
{
  public Transform magazinSiteTransform;
  public Magazin magazinType;
}

[System.Serializable]
public class WeaponHolder
{
  public weaponType weaponHolderType;
  public Transform holderTransform;
}