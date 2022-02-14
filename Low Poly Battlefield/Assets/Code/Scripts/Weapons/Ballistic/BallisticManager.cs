using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallisticManager : MonoBehaviour
{
    public static BallisticManager instance;
    public SurfaceType[] surfaceTypes;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("More that one instance of " + this + " in the scene !");
            return;
        }

        instance = this;
    }
}

[System.Serializable]
public class SurfaceType
{
    public string tagName;
    public GameObject[] hitPrefabs;
    public GameObject[] decalsPrefabs;
}