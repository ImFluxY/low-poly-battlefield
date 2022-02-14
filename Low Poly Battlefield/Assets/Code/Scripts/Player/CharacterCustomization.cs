using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomization : MonoBehaviour
{
    public CharacterSex sex;
    public CharacterCamp camp;
    public CustomizedBodyPart[] customizedBodyPart;

    private void Update()
    {
        ChangeItems();
    }

    private void ChangeItems()
    {
        for (int i = 0; i < customizedBodyPart.Length; i++)
        {
            for (int j = 0; j < customizedBodyPart[i].types.Count; j++)
            {
                for (int k = 0; k < customizedBodyPart[i].types[j].items.Count; k++)
                {
                    if (k == customizedBodyPart[i].types[j].selectedItem)
                    {
                        customizedBodyPart[i].types[j].items[k].SetActive(true);
                    }
                    else
                    {
                        customizedBodyPart[i].types[j].items[k].SetActive(false);
                    }
                }
            }
        }
    }
}

public enum CharacterSex
{
    Man,
    Women
}

public enum CharacterCamp
{
    Contractor,
    Insurgent
}

[System.Serializable]
public class CustomizedBodyPart
{
    public string bodyPartName;
    public List<CustomizedBodyItemType> types;
}

[System.Serializable]
public class CustomizedBodyItemType
{
    public string itemType;
    public int selectedItem;
    public List<GameObject> items;
}