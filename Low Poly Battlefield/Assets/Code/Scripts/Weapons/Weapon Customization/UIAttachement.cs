using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAttachement : MonoBehaviour
{
    [SerializeField]
    private Text partName;

    private WeaponCustomization weaponCustomization;
    private WeaponPartType weaponPartType;
    private int itemIndex;

    public void InitializeAttachementUI(string _text, WeaponCustomization _weaponCustomization, WeaponPartType _weaponPartType, int _itemIndex)
    {
        partName.text = _text;
        weaponCustomization = _weaponCustomization;
        weaponPartType = _weaponPartType;
        itemIndex = _itemIndex;
    }

    public void ChangeItem()
    {
        weaponCustomization.ChangeItem(weaponPartType, itemIndex);
    }
}
