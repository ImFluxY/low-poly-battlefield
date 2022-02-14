using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipement : MonoBehaviour
{
    public int selectedWeapon = 0;
    public List<EquippedWeapon> equippedWeapons;
    public List<weaponHolder> weaponHolders;
    public SimpleWeapon currentWeapon;
    [Space]
    public int selectedGrenade = 0;
    public List<Grenade> equippedGrenades;
    [Space]
    public int magSelected = 0;
    public List<magazinSite> magazinSites;
    public List<weaponMag> magazins;
    private Weapon currentWeaponProperties;

    private void Start()
    {
        SetupMags();

        if (equippedWeapons.Count > 0)
            InitializeWeapon();
    }

    private void Update()
    {
        if (equippedWeapons.Count > 0)
            return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            CycleWeaponIndex();
            ReinitializeWeapon();
            InitializeWeapon();
        }
    }

    private void CycleWeaponIndex()
    {
        if(selectedWeapon < equippedWeapons.Count - 1)
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
            if(equippedGrenades[i].grenadeType == grenadeType)
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
        if(currentWeapon == null)
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
        if(currentWeapon.currentMag.magProperties == null)
        {
            currentWeapon.currentMag.currentAmmoCount = currentWeaponProperties.magazinType.maxAmmoCount;
            currentWeapon.currentMag.magProperties = currentWeaponProperties.magazinType;
            currentWeapon.currentMag.magazin = Instantiate(currentWeaponProperties.magazinType.magPrefab, currentWeapon.weaponRef.magPos);
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
}

[System.Serializable]
public class magazinSite
{
    public Transform magazinSiteTransform;
    public Magazin magazinType;
}

public class weaponHolder
{
    public weaponType weaponHolderType;
    public Transform holderTransform;
}
