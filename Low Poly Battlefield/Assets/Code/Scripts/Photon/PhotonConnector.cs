using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
using Random = UnityEngine.Random;

[System.Serializable]
public class ProfileData
{
    public string username;
    public int level;
    public int xp;

    public ProfileData()
    {
        this.username = "";
        this.level = 0;
        this.xp = 0;
    }

    public ProfileData(string u, int l, int x)
    {
        this.username = u;
        this.level = l;
        this.xp = x;
    }
}

public class PhotonConnector : MonoBehaviourPunCallbacks
{
    public static ProfileData localPlayerProfil = new ProfileData();
    [SerializeField] private TMPro.TMP_InputField usernameField;

    //public static Action GetPhotonFriends = delegate { };
    public static Action OnLobbyJoined = delegate { };
    public static Action OnConnectedToPhoton = delegate { };

    #region Unity Method
    private void Awake()
    {
        //myProfile = Data.LoadProfile();
        HandleUpdateUsername("Player_" + Random.Range(0, 1000));

        if (!string.IsNullOrEmpty(localPlayerProfil.username))
        {
            usernameField.text = localPlayerProfil.username;
        }

        UIConnector.OnUpdateUsername += HandleUpdateUsername;
    }
    private void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady || PhotonNetwork.IsConnected) return;
            ConnectToPhoton();
    }
    #endregion
    #region Private Methods
    private void ConnectToPhoton()
    {
        Debug.Log($"Connect to Photon as {localPlayerProfil.username}");
        OnConnectedToPhoton?.Invoke();
        PhotonNetwork.AuthValues = new AuthenticationValues(localPlayerProfil.username);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }        
    #endregion
    #region Photon Callbacks
    public override void OnConnectedToMaster()
    {
        Debug.Log("You have connected to the Photon Master Server");
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("You have connected to a Photon Lobby");
        // Debug.Log("Invoking get Playfab friends");
        //GetPhotonFriends?.Invoke();
        OnLobbyJoined?.Invoke();
    }

    private void HandleUpdateUsername(string newName)
    {
        localPlayerProfil.username = newName;
        PhotonNetwork.NickName = localPlayerProfil.username;
    }
    #endregion
}