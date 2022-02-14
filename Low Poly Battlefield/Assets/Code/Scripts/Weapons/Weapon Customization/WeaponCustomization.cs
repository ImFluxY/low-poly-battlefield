using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;

public class WeaponCustomization : MonoBehaviour
{
    public string weaponName;
    public List<WeaponPartType> weaponPartTypes;
    [Space]
    public Transform selectorCanvas;
    public GameObject selectorPref;
    public GameObject attachmentPref;
    public Transform attachementPrefParent;

    private void Start()
    {
        UpdateItems();
    }

    public void UpdateItems()
    {
        for (int i = 0; i < weaponPartTypes.Count; i++)
        {
            ChangeItem(weaponPartTypes[i], weaponPartTypes[i].selectedItem);
        }
    }

    public void SelectItem(WeaponPartType weaponPartType)
    {
        // Destroy all childs of the attachements parent
        foreach (Transform child in attachementPrefParent)
        {
            Destroy(child.gameObject);
        }

        //Add the attachements UI
        for (int i = 0; i < weaponPartType.items.Count; i++)
        {
            if (canBeActivated(weaponPartType, i))
            {
                GameObject newAttachementUI = Instantiate(attachmentPref, attachementPrefParent) as GameObject;
                UIAttachement UIAttachement = newAttachementUI.GetComponent<UIAttachement>();
                UIAttachement.InitializeAttachementUI(weaponPartType.items[i].weaponPartName, this, weaponPartType, i);
            }
        }
    }

    public void ChangeItem(WeaponPartType weaponPartType, int value)
    {
        //Destroy graphics items
        foreach (Transform child in weaponPartType.partGraphicParent)
        {
            Destroy(child.gameObject);
        }

        weaponPartType.selectedItem = value;

        for (int i = 0; i < weaponPartType.items.Count; i++)
        {
            if (weaponPartType.selectedItem == i && canBeActivated(weaponPartType, i))
            {
                if (weaponPartType.items[i].partPrefab != null)
                {
                    //Graphic prefab
                    GameObject part = Instantiate(weaponPartType.items[i].partPrefab, weaponPartType.partGraphicParent) as GameObject;
                    part.transform.localPosition = weaponPartType.items[i].posOffset;
                }

                //UI Prefab
                if (weaponPartType.selectableUI != null)
                    Destroy(weaponPartType.selectableUI);

                if (canBeSelectable(i))
                {
                    GameObject selector = Instantiate(selectorPref, selectorCanvas) as GameObject;
                    weaponPartType.selectableUI = selector;
                    RectTransform selectorTransform = selector.GetComponent<RectTransform>();
                    selectorTransform.position = weaponPartType.partGraphicParent.position;
                    selector.GetComponentInChildren<Text>().text = weaponPartType.weaponPartTypeName;
                    selector.GetComponentInChildren<Button>().onClick.AddListener(delegate { this.SelectItem(weaponPartType); });
                }
            }
        }
    }

    public bool canBeSelectable(int partTypeIndex)
    {
        if (weaponPartTypes[partTypeIndex].needOneToBeSelectable.Count == 0)
            return true;

        for (int i = 0; i < weaponPartTypes[partTypeIndex].needOneToBeSelectable.Count; i++)
        {
            if (weaponPartTypes[partTypeIndex].needOneToBeSelectable[i].activeSelf)
            {
                return true;
            }
        }

        return false;
    }

    public bool canBeActivated(WeaponPartType weaponPartType, int _i)
    {
        bool minimumToBeActive = true;
        bool neededToBeActive = true;

        for (int j = 0; j < weaponPartType.items[_i].minimumToBeActive.Length; j++)
        {
            if (weaponPartType.items[_i].minimumToBeActive[j].activeSelf)
            {
                minimumToBeActive = false;
            }
        }

        for (int j = 0; j < weaponPartType.items[_i].neededToBeActive.Length; j++)
        {
            if (!weaponPartType.items[_i].neededToBeActive[j].activeSelf)
            {
                neededToBeActive = false;
            }
        }

        if (minimumToBeActive && neededToBeActive)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

[System.Serializable]
public class WeaponPartType
{
    [Header("UI")]
    public string weaponPartTypeName;
    public GameObject selectableUI;
    public List<GameObject> needOneToBeSelectable;

    [Header("Graphics")]
    public Transform partGraphicParent;
    public int selectedItem;
    public List<WeaponParts> items;
}

[System.Serializable]
public class WeaponParts
{
    public string weaponPartName;
    public GameObject partPrefab;
    public Vector3 posOffset = Vector3.zero;
    public GameObject[] minimumToBeActive;
    public GameObject[] neededToBeActive;
}