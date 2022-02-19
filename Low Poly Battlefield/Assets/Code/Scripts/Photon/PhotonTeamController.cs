using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PhotonTeamController : MonoBehaviourPunCallbacks
{
    [SerializeField] private List<PhotonTeam> _roomTeams;
    [SerializeField] private int _teamSize;
    [SerializeField] private PhotonTeam _priorTeam;

    public static Action<List<PhotonTeam>> OnCreateTeams = delegate { };
    public static Action<Player, PhotonTeam> OnSwitchTeam = delegate { };
    public static Action<Player> OnRemovePlayer = delegate { };
    public static Action OnClearTeams = delegate { };

    #region Unity Methods
    private void Awake()
    {
        UITeam.OnSwitchToTeam += HandleSwitchTeam;
        PhotonRoomController.OnJoinRoom += HandleCreateTeams;
        PhotonRoomController.OnRoomLeft += HandleLeaveRoom;
        PhotonRoomController.OnOtherPlayerLeftRoom += HandleOtherPlayerLeftRoom;

        _roomTeams = new List<PhotonTeam>();
    }

    private void OnDestroy()
    {
        UITeam.OnSwitchToTeam -= HandleSwitchTeam;
        PhotonRoomController.OnJoinRoom -= HandleCreateTeams;
        PhotonRoomController.OnRoomLeft -= HandleLeaveRoom;
        PhotonRoomController.OnOtherPlayerLeftRoom -= HandleOtherPlayerLeftRoom;
    }
    #endregion

    #region Handle Methods
    private void HandleSwitchTeam(PhotonTeam newTeam)
    {            
        if (PhotonNetwork.LocalPlayer.GetPhotonTeam() == null)
        {
            _priorTeam = PhotonNetwork.LocalPlayer.GetPhotonTeam();
            PhotonNetwork.LocalPlayer.JoinTeam(newTeam);                
        }
        else if (CanSwitchToTeam(newTeam))
        {
            _priorTeam = PhotonNetwork.LocalPlayer.GetPhotonTeam();
            PhotonNetwork.LocalPlayer.SwitchTeam(newTeam);                
        }
    }

    private void HandleCreateTeams()
    {
        CreateTeams();

        OnCreateTeams?.Invoke(_roomTeams);

        AutoAssignPlayerToTeam(PhotonNetwork.LocalPlayer);
    }

    private void HandleLeaveRoom()
    {
        PhotonNetwork.LocalPlayer.LeaveCurrentTeam();
        _roomTeams.Clear();
        _teamSize = 0;
        OnClearTeams?.Invoke();
    }

    private void HandleOtherPlayerLeftRoom(Player otherPlayer)
    {
        OnRemovePlayer?.Invoke(otherPlayer);
    }
    #endregion

    #region Private Methods
    private void CreateTeams()
    {
        _teamSize = GameSettings.TeamSize;
        int numberOfTeams = GameSettings.MaxPlayers;
        if (GameSettings.HasTeams)
        {
            numberOfTeams = 2;
        }

        for (int i = 1; i <= numberOfTeams; i++)
        {
            PhotonTeam team;
            PhotonTeamsManager.Instance.TryGetTeamByCode((byte)i, out team);
            _roomTeams.Add(team);
        }
    }

    private bool CanSwitchToTeam(PhotonTeam newTeam)
    {
        bool canSwitch = false;

        if (PhotonNetwork.LocalPlayer.GetPhotonTeam().Code != newTeam.Code)
        {
            Player[] players = null;
            if (PhotonTeamsManager.Instance.TryGetTeamMembers(newTeam.Code, out players))
            {
                if (players.Length < _teamSize)
                {
                    canSwitch = true;
                }
                else
                {
                    Debug.Log($"{newTeam.Name} is full");
                }
            }
        }
        else
        {
            Debug.Log($"You are already on the team {newTeam.Name}");
        }

        return canSwitch;
    }

    private void AutoAssignPlayerToTeam(Player player)
    {
        foreach (PhotonTeam team in _roomTeams)
        {
            int teamPlayerCount = PhotonTeamsManager.Instance.GetTeamMembersCount(team.Code);

            if (teamPlayerCount < GameSettings.TeamSize)
            {
                Debug.Log($"Auto assigned {player.NickName} to {team.Name}");
                if (player.GetPhotonTeam() == null)
                {
                    player.JoinTeam(team.Code);
                }
                else if (player.GetPhotonTeam().Code != team.Code)
                {
                    player.SwitchTeam(team.Code);
                }
                break;
            }
        }
    }
    #endregion

    #region Photon Callback Methods
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        object teamCodeObject;
        if (changedProps.TryGetValue(PhotonTeamsManager.TeamPlayerProp, out teamCodeObject))
        {
            if (teamCodeObject == null) return;

            byte teamCode = (byte)teamCodeObject;
                
            PhotonTeam newTeam;
            if(PhotonTeamsManager.Instance.TryGetTeamByCode(teamCode, out newTeam))
            {
                Debug.Log($"Switching {targetPlayer.NickName} to new team {newTeam.Name}");
                OnSwitchTeam?.Invoke(targetPlayer, newTeam);
            }
        }
    }
    #endregion
}