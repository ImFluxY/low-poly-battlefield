using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
using Random = UnityEngine.Random;

public class PhotonConnector : MonoBehaviourPunCallbacks
{
    [SerializeField] private string nickName;
    //public static Action GetPhotonFriends = delegate { };
    public static Action OnLobbyJoined = delegate { };
    public static Action OnConnectedToPhoton = delegate { };

    #region Unity Method
    private void Awake()
    {
        //nickName = PlayerPrefs.GetString("USERNAME");            
        HandleUpdateUsername("Player_" + Random.Range(0, 1000));
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
        Debug.Log($"Connect to Photon as {nickName}");
        OnConnectedToPhoton?.Invoke();
        PhotonNetwork.AuthValues = new AuthenticationValues(nickName);
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
        nickName = newName;
        PhotonNetwork.NickName = nickName;
    }
    #endregion
}