using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class UIDisplayRoom : MonoBehaviour
{
    [SerializeField] private TMP_InputField _roomNameInput;
    [SerializeField] private TMP_Text _roomTitleText;
    [SerializeField] private GameObject _startButton;
    [SerializeField] private GameObject _exitButton;
    [SerializeField] private GameObject _roomContainer;
    [SerializeField] private Transform _roomListContainer;
    [SerializeField] private UIRoom _uiRoomPrefab;
    [SerializeField] private GameObject[] _hideObjects;
    [SerializeField] private GameObject[] _showObjects;

    public static Action OnStartGame = delegate { };
    public static Action<string> OnCreatRoom = delegate { };
    public static Action OnLeaveRoom = delegate { };

    private void Awake()
    {
        PhotonRoomController.OnJoinRoom += HandleJoinRoom;
        PhotonRoomController.OnRoomLeft += HandleRoomLeft;
        PhotonRoomController.OnUpdateRoomList += HandleUpdateRoomList;
        PhotonRoomController.OnMasterOfRoom += HandleMasterOfRoom;
        PhotonRoomController.OnCountingDown += HandleCountingDown;
    }

    private void OnDestroy()
    {
        PhotonRoomController.OnJoinRoom -= HandleJoinRoom;
        PhotonRoomController.OnRoomLeft -= HandleRoomLeft;
        PhotonRoomController.OnUpdateRoomList -= HandleUpdateRoomList;
        PhotonRoomController.OnMasterOfRoom -= HandleMasterOfRoom;
        PhotonRoomController.OnCountingDown -= HandleCountingDown;
    }

    private void HandleUpdateRoomList(List<RoomInfo> roomList)
    {
        foreach (Transform trans in _roomListContainer)
        {
            Destroy(trans.gameObject);
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;

            UIRoom uiRoom = Instantiate(_uiRoomPrefab, _roomListContainer);
            uiRoom.Initialize(roomList[i]);
        }
    }

    private void HandleJoinRoom()
    {
        _roomTitleText.SetText(PhotonNetwork.CurrentRoom.CustomProperties["GAMEMODE"].ToString());

        _exitButton.SetActive(true);
        _roomContainer.SetActive(true);

        foreach (GameObject obj in _hideObjects)
        {
            obj.SetActive(false);
        }
    }

    private void HandleRoomLeft()
    {
        _roomTitleText.SetText("JOINING ROOM");

        _startButton.SetActive(false);
        _exitButton.SetActive(false);
        _roomContainer.SetActive(false);
        foreach (GameObject obj in _showObjects)
        {
            obj.SetActive(true);
        }
    }

    private void HandleMasterOfRoom(Player masterPlayer)
    {
        _roomTitleText.SetText(PhotonNetwork.CurrentRoom.CustomProperties["GAMEMODE"].ToString());

        if (PhotonNetwork.LocalPlayer.Equals(masterPlayer))
        {
            _startButton.SetActive(true);
        }
        else
        {
            _startButton.SetActive(false);
        }
    }

    private void HandleCountingDown(float count)
    {
        _startButton.SetActive(false);
        _exitButton.SetActive(false);
        _roomTitleText.SetText(count.ToString("F0"));
    }

    public void CreateRoom()
    {
        string roomName;

        if (string.IsNullOrEmpty(_roomNameInput.text))
            roomName = "Room_" + Random.Range(0, 1000);
        else
            roomName = _roomNameInput.text;

        OnCreatRoom?.Invoke(roomName);
    }

    public void LeaveRoom()
    {
        OnLeaveRoom?.Invoke();
    }

    public void StartGame()
    {
        Debug.Log($"Starting game...");
        OnStartGame?.Invoke();
    }
}