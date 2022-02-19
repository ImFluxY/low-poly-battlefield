using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using ExitGames.Client.Photon;

public enum GameState
{
  Waiting = 0,
  Starting = 1,
  Playing = 2,
  Ending = 3
}

[System.Serializable]
public class PlayerInfo
{
  public ProfileData profile;
  public int actor;
  public short kills;
  public short deaths;
  public int team;

  public PlayerInfo(ProfileData p, int a, short k, short d, int t)
  {
    this.profile = p;
    this.actor = a;
    this.kills = k;
    this.deaths = d;
    this.team = t;
  }
}

public class PhotonGameController : MonoBehaviourPunCallbacks, IOnEventCallback
{
  #region Fields

  public static PhotonGameController Instance;

  public int matchLength = 180;
  public int killcount = 3;
  public bool perpetual = false;

  public GameObject mapCam;
  public PlayerManager localPlayerManager;
  public List<PlayerInfo> playerInfos = new List<PlayerInfo>();

  private int currentMatchTime;
  private Coroutine gameTimerCoroutine;
  private Coroutine waitingTimerCoroutine;

  private GameState state = GameState.Waiting;

  //Actions
  public static Action OnInitializeUI = delegate { };
  public static Action<List<PlayerInfo>, int> OnRefreshStatsUI = delegate { };
  public static Action<int> OnRefreshTimerUI = delegate { };

  #endregion

  #region Codes

  public enum EventCodes : byte
  {
    NewMatch,
    NewPlayer,
    UpdatePlayers,
    ChangeStat,
    ChangeGameState
  }

  public void OnEvent(EventData photonEvent)
  {
    if (photonEvent.Code >= 200) return;

    EventCodes e = (EventCodes)photonEvent.Code;
    object[] o = (object[])photonEvent.CustomData;

    switch (e)
    {
      case EventCodes.NewMatch:
        Recieve_NewMatch();
        break;

      case EventCodes.NewPlayer:
        Recieve_NewPlayer(o);
        break;

      case EventCodes.UpdatePlayers:
        Recieve_UpdatePlayers(o);
        break;

      case EventCodes.ChangeStat:
        Recieve_ChangeStat(o);
        break;

      case EventCodes.ChangeGameState:
        Recieve_ChangeGameState(o);
        break;
    }
  }

  #endregion

  public override void OnEnable()
  {
    PhotonNetwork.AddCallbackTarget(this);
  }

  public override void OnDisable()
  {
    PhotonNetwork.RemoveCallbackTarget(this);
  }

  private void Awake()
  {
    Instance = this;
  }

  private void Start()
  {
    mapCam.SetActive(true);
    if (!PhotonNetwork.IsConnected)
    {
      SceneManager.LoadScene(0);
      return;
    }

    OnInitializeUI?.Invoke();

    Send_NewPlayer(PhotonConnector.localPlayerProfil);
  }

  public PlayerInfo GetPlayerInfos(int ind)
  {
    return playerInfos[ind];
  }

  private void ValidateConnection()
  {
    if (PhotonNetwork.IsConnected) return;
    SceneManager.LoadScene(0);
  }

  public void Send_NewMatch()
  {
    PhotonNetwork.RaiseEvent(
        (byte)EventCodes.NewMatch,
        null,
        new RaiseEventOptions { Receivers = ReceiverGroup.All },
        new SendOptions { Reliability = true }
    );
  }

  public void Recieve_NewMatch()
  {
    // set game state to waiting
    state = GameState.Waiting;

    // deactivate map camera
    //mapcam.SetActive(false);

    // hide end game ui
    //ui_endgame.gameObject.SetActive(false);

    // reset scores
    foreach (PlayerInfo p in playerInfos)
    {
      p.kills = 0;
      p.deaths = 0;
    }

    // reset ui
    OnRefreshStatsUI?.Invoke(playerInfos, localPlayerManager.GetMyInd());

    // spawn
    //Spawn();
  }

  private void Send_NewPlayer(ProfileData profileData)
  {
    object[] package = new object[7];
    package[0] = profileData.username;
    package[1] = profileData.level;
    package[2] = profileData.xp;
    package[3] = PhotonNetwork.LocalPlayer.ActorNumber; //Identifier of the player in the room
    package[4] = (short)0;
    package[5] = (short)0;
    package[6] = (int)PhotonNetwork.LocalPlayer.GetPhotonTeam().Code;

    Debug.Log("Try Send : " + profileData);

    PhotonNetwork.RaiseEvent(
                (byte)EventCodes.NewPlayer,
                package,
                new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
                new SendOptions { Reliability = true }
            );
  }

  //Only the MasterClient is recieving this
  private void Recieve_NewPlayer(object[] data)
  {
    //Set info of the new user in his PlayerManager
    PlayerInfo newPlayerInfo = new PlayerInfo(
                    new ProfileData(
                    (string)data[0],
                    (int)data[1],
                    (int)data[2]
                ),
                (int)data[3],
                (short)data[4],
                (short)data[5],
                (int)data[6]);

    playerInfos.Add(newPlayerInfo);
    Debug.Log("New Player Manager : " + playerInfos[0].profile.username);

    if (playerInfos.Count == PhotonNetwork.CurrentRoom.PlayerCount)
    {
      Send_ChangeGameStat(GameState.Starting);
    }

    Send_UpdatePlayers(playerInfos);
  }

  private void Send_UpdatePlayers(List<PlayerInfo> infos)
  {
    object[] package = new object[infos.Count + 1]; //Get the number of PlayerManagers from the MasterClient

    for (int i = 0; i < infos.Count; i++) //Foreach manager in the list, extract the informations and send to All
    {
      object[] piece = new object[7];
      piece[0] = infos[i].profile.username;
      piece[1] = infos[i].profile.level;
      piece[2] = infos[i].profile.xp;
      piece[3] = infos[i].actor;
      piece[4] = infos[i].kills;
      piece[5] = infos[i].deaths;
      piece[6] = infos[i].team;

      package[i + 1] = piece;
    }

    PhotonNetwork.RaiseEvent(
                (byte)EventCodes.UpdatePlayers,
                package,
                new RaiseEventOptions { Receivers = ReceiverGroup.All },
                new SendOptions { Reliability = true }
            );
  }

  private void Recieve_UpdatePlayers(object[] data)
  {
    playerInfos = new List<PlayerInfo>();

    for (int i = 1; i < data.Length; i++)
    {
      object[] extract = (object[])data[i];

      PlayerInfo p = new PlayerInfo(
          new ProfileData(
              (string)extract[0],
              (int)extract[1],
              (int)extract[2]
          ),
          (int)extract[3],
          (short)extract[4],
          (short)extract[5],
          (int)extract[6]
      );

      playerInfos.Add(p);

      //If the Local Player hasn't been spawned, then do it now
      if (PhotonNetwork.LocalPlayer.ActorNumber == p.actor)
      {
        if (localPlayerManager == null)
        {
          Debug.Log("Spawn Local Player Manager");
          localPlayerManager = PhotonNetwork.Instantiate(Path.Combine("Managers", "PlayerManager"), Vector3.zero, Quaternion.identity, 0, new object[] { i - 1 }).GetComponent<PlayerManager>();
          //localPlayerManager.SetMyInd(i - 1);
          localPlayerManager.Spawn();
        }
      }
    }
  }

  public void Send_ChangeStat(int actor, byte stat, byte amt)
  {
    object[] package = new object[] { actor, stat, amt };

    PhotonNetwork.RaiseEvent(
        (byte)EventCodes.ChangeStat,
        package,
        new RaiseEventOptions { Receivers = ReceiverGroup.All },
        new SendOptions { Reliability = true }
    );
  }

  private void Recieve_ChangeStat(object[] data)
  {
    int actor = (int)data[0];
    byte stat = (byte)data[1];
    byte amt = (byte)data[2];

    for (int i = 0; i < playerInfos.Count; i++)
    {
      if (playerInfos[i].actor == actor)
      {
        switch (stat)
        {
          case 0: //kills
            playerInfos[i].kills += amt;
            Debug.Log($"Player {playerInfos[i].profile.username} : kills = {playerInfos[i].kills}");
            break;

          case 1: //deaths
            playerInfos[i].deaths += amt;
            Debug.Log($"Player {playerInfos[i].profile.username} : deaths = {playerInfos[i].deaths}");
            break;
        }

        if (i == localPlayerManager.GetMyInd()) OnRefreshStatsUI?.Invoke(playerInfos, localPlayerManager.GetMyInd());
        //if (ui_leaderboard.gameObject.activeSelf) Leaderboard(ui_leaderboard);

        break;
      }
    }
    //ScoreCheck();
  }

  public void Send_ChangeGameStat(GameState newGameState)
  {
    object[] package = new object[] { newGameState };

    PhotonNetwork.RaiseEvent(
                (byte)EventCodes.ChangeGameState,
                package,
                new RaiseEventOptions { Receivers = ReceiverGroup.All },
                new SendOptions { Reliability = true }
            );
  }

  private void Recieve_ChangeGameState(object[] data)
  {
    state = (GameState)data[0];

    switch (state)
    {
      case GameState.Waiting:
        Debug.Log("Game State : Waiting other players");
        break;
      case GameState.Starting:
        Debug.Log("Game State : Starting the game");
        // reinitialize time
        //InitializeTimer();
        break;
      case GameState.Playing:
        Debug.Log("Game State : Playing the game");
        break;
      case GameState.Ending:
        Debug.Log("Game State : Ending the game");
        break;
    }
  }

}