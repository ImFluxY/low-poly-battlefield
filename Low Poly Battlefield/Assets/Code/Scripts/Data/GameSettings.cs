using System;
using UnityEngine;

public enum GameMode
{
  TDM = 0,
}

public class GameSettings : MonoBehaviour
{
  public static GameMode GameMode = GameMode.TDM;
  public static int MaxPlayers = 2;
  public static int TeamSize = 1;
  public static bool HasTeams = true;
}