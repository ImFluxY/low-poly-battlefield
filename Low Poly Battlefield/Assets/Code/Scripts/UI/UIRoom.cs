using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;

public class UIRoom : MonoBehaviour
{
    [SerializeField] private TMP_Text _roomNameText;
    [SerializeField] private TMP_Text _roomGameModeText;
    [SerializeField] private TMP_Text _roomMapText;
    [SerializeField] private TMP_Text _roomPlayerCountText;
    [SerializeField] private RoomInfo _roomInfo;

    public static Action<RoomInfo> OnJoinTheRoom = delegate { };

    public void Initialize(RoomInfo roomInfo)
    {
        _roomInfo = roomInfo;
        _roomNameText.text = _roomInfo.Name;

        string gameMode = "";
        switch(_roomInfo.CustomProperties[PhotonRoomController.GAME_MODE])
        {
            case 0:
                gameMode = "TDM";
                break;
        }

        Debug.Log(_roomInfo.CustomProperties[PhotonRoomController.GAME_MAP]);
        _roomMapText.text = (string)_roomInfo.CustomProperties[PhotonRoomController.GAME_MAP];
        _roomGameModeText.text = gameMode;
        _roomPlayerCountText.text = _roomInfo.PlayerCount.ToString() + " / " + _roomInfo.MaxPlayers.ToString();
    }

    public void JoinTheRoom()
    {
        Debug.Log($"Joining the room {_roomInfo.Name}");
        OnJoinTheRoom?.Invoke(_roomInfo);
    }
}