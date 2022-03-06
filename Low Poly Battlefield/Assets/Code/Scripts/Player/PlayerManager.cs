using System.Collections;
using Photon.Pun;
using UnityEngine;
using System.IO;

[System.Serializable]
public class PlayerManager : MonoBehaviourPunCallbacks
{
  [SerializeField] public static GameObject LocalPlayerInstance;
  [SerializeField] int myPlayerInd;
  bool playerAdded;
  [SerializeField] PlayerInfo myPlayerInfos;
  private PlayerController localPlayerController;
  private PlayerCamera localPlayerCamera;
  private PlayerLocomotion localPlayerLocomotion;
  private SimpleWeapon localPlayerWeapon;
  private InputManager localPlayerInputManager;
  private PlayerEquipement localPlayerEquipement;

  public void EnablePlayerControl()
  {
    Debug.Log("Enable Control");
    localPlayerLocomotion.control = true;
    localPlayerController.control = true;
    localPlayerWeapon.control = true;
    localPlayerInputManager.control = true;
    localPlayerCamera.control = true;
  }

  public void DisablePlayerControl()
  {
    Debug.Log("Disable control");
    localPlayerLocomotion.control = false;
    localPlayerController.control = false;
    localPlayerWeapon.control = false;
    localPlayerInputManager.control = false;
    localPlayerCamera.control = false;
  }

  private void Start()
  {
    localPlayerInputManager = InputManager.Instance;
    playerAdded = false;
    myPlayerInd = (int)PV.InstantiationData[0];
    myPlayerInfos = PhotonGameController.Instance.GetPlayerInfos(myPlayerInd);
  }

  public int GetMyInd()
  {
    return this.myPlayerInd;
  }

  public int GetMyTeam()
  {
    return this.myPlayerInfos.team;
  }

  public void Spawn(bool canControl)
  {
    Transform spawn = SpawnManager.instance.GetTeamSpawn(0);
    LocalPlayerInstance = PhotonNetwork.Instantiate(Path.Combine("Characters", "Player"), spawn.position, spawn.rotation, 0, new object[] { PV.ViewID });
    localPlayerController = LocalPlayerInstance.GetComponent<PlayerController>();
    localPlayerCamera = LocalPlayerInstance.GetComponent<PlayerCamera>();
    localPlayerLocomotion = LocalPlayerInstance.GetComponent<PlayerLocomotion>();
    localPlayerWeapon = LocalPlayerInstance.GetComponent<SimpleWeapon>();

    if (canControl)
      EnablePlayerControl();
    else
      DisablePlayerControl();
  }

  public void Die(int actor)
  {
    //Send to all players that the Local Player is dead
    PhotonGameController.Instance.Send_ChangeStat(PhotonNetwork.LocalPlayer.ActorNumber, 1, 1);

    //If someone kill me (actor = 0 → Suicide), then add a kill to that player
    if (actor >= 0)
      PhotonGameController.Instance.Send_ChangeStat(actor, 0, 1);

    DisablePlayerControl();
    localPlayerController.Die();
    StartCoroutine(Dead(10));
  }

  private IEnumerator Dead(float seconds)
  {
    yield return new WaitForSeconds(seconds);

    //Destroy the Local Player gameobject 
    PhotonNetwork.Destroy(LocalPlayerInstance);

    //Respawn Instantly
    Spawn(true);
  }

  /*
  public void TrySync()
  {
    if (!PV.IsMine) return;

    PV.RPC("SyncProfile", RpcTarget.All, PhotonConnector.localPlayerProfil.username, PhotonConnector.localPlayerProfil.level, PhotonConnector.localPlayerProfil.xp);

    if (GameSettings.GameMode == GameMode.TDM)
    {
      PV.RPC("SyncTeam", RpcTarget.All, 0);
    }
  }

  [PunRPC]
  private void SyncProfile(string p_username, int p_level, int p_xp)
  {
    myPlayerInfos = new ProfileData(p_username, p_level, p_xp);
    //playerUsername.text = playerProfile.username;
  }

  [PunRPC]
  private void SyncTeam(int team)
  {

  }
  */
}
