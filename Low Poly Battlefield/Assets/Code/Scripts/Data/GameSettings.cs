public enum GameMode
{
  TDM = 0,
}

[System.Serializable]
public class GameSettings
{
  public static GameMode GameMode = GameMode.TDM;
  public static int MaxPlayers = 20;
  public static bool HasTeams = true;
  public static int TeamSize = 10;
  public static int killCount = 75;
  public static int timeLimit = 600; //In seconds
}