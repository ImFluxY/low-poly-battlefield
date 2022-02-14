using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataController : MonoBehaviour
{
    public static DataController instance;

    private void Awake()
    {
        if (!instance)
            instance = this;
    }

    private static readonly int DEFAULT_INT = 0;

    public void SaveInt(string _data, int _value)
    {
        PlayerPrefs.SetInt(_data, _value);
    }

    public int GetInt(string _data)
    {
        return PlayerPrefs.GetInt(_data, DEFAULT_INT);
    }
}
