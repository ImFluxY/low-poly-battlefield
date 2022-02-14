using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using System.IO;

public class TestScene : MonoBehaviourPunCallbacks
{
    public static TestScene instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Debug.Log("Connecting to Master.");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master.");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined the Lobby.");
        PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");

        int newTeam = Random.Range(0, 2);
        JoinTeam(newTeam);

        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        MenuManager.instance.OpenMenu("Error");
    }

    public override void OnJoinedRoom()
    {
        int team = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];
        Debug.Log($"Team number {team} is being instantiated");

        PhotonNetwork.Instantiate(Path.Combine("Managers", "PlayerManager"), Vector3.zero, Quaternion.identity, 0, new object[] { team });
    }

    public void JoinTeam(int team)
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
        {
            PhotonNetwork.LocalPlayer.CustomProperties["Team"] = team;
        }
        else
        {
            Hashtable playerProps = new Hashtable
            {
                { "Team", team }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
        }
    }
}