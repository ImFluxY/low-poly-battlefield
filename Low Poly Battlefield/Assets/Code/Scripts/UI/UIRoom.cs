using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIRoom : MonoBehaviour
{
    [SerializeField] private TMP_Text _roomNameText;
    [SerializeField] private TMP_Text _roomGameModeText;
    [SerializeField] private TMP_Text _roomPlayerCountText;
    [SerializeField] private RoomInfo _roomInfo;

    public static Action<RoomInfo> OnJoinTheRoom = delegate { };

    public void Initialize(RoomInfo roomInfo)
    {
        _roomInfo = roomInfo;
        _roomNameText.text = _roomInfo.Name;
        _roomGameModeText.text = (string)_roomInfo.CustomProperties[PhotonRoomController.GAME_MODE];
        _roomPlayerCountText.text = _roomInfo.PlayerCount.ToString() + " / " + _roomInfo.MaxPlayers.ToString();
    }

    public void JoinTheRoom()
    {
        Debug.Log($"Joining the room {_roomInfo.Name}");
        OnJoinTheRoom?.Invoke(_roomInfo);
    }
}