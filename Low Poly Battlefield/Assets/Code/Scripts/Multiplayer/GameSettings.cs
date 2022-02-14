using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameMode
{
    TDM = 1
}

public class GameSettings : MonoBehaviour
{
    public static GameMode GameMode = GameMode.TDM;
}
