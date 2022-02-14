using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsManager : MonoBehaviour
{
    public CustomWeapon[] weapons;
    public int currentWeapon;

    private DataController dataController;

    private void Start()
    {
        dataController = FindObjectOfType<DataController>();
        ChargeItems();
    }

    public void SaveChanges()
    {
        for (int i = 0; i < weapons[currentWeapon].weapon.weaponPartTypes.Count; i++)
        {
            dataController.SaveInt(weapons[currentWeapon].weapon.weaponName + "_" + weapons[currentWeapon].weapon.weaponPartTypes[i].weaponPartTypeName, weapons[currentWeapon].weapon.weaponPartTypes[i].selectedItem);
        }
    }

    public void ChargeItems()
    {
        for (int i = 0; i < weapons[currentWeapon].weapon.weaponPartTypes.Count; i++)
        {
            weapons[currentWeapon].weapon.weaponPartTypes[i].selectedItem = dataController.GetInt(weapons[currentWeapon].weapon.weaponName + "_" + weapons[currentWeapon].weapon.weaponPartTypes[i].weaponPartTypeName);
        }

        weapons[currentWeapon].weapon.UpdateItems();
    }

    public void ChangeTeam()
    {

    }
}

[System.Serializable]
public class CustomWeapon
{
    public WeaponCustomization weapon;

    public enum membership
    {
        Security,
        Insurgent,
        Both
    }
    public membership memberShip;
}