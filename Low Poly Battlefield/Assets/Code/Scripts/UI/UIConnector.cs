using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIConnector : MonoBehaviour
{
    [SerializeField] private TMP_InputField _userNameInput;
    [SerializeField] private GameObject loadScreen;

    public static Action<string> OnUpdateUsername = delegate { };

    private void Awake()
    {
        PhotonConnector.OnConnectedToPhoton += Loading;
        PhotonConnector.OnLobbyJoined += Loaded;
    }

    private void OnDestroy()
    {
        PhotonConnector.OnConnectedToPhoton -= Loading;
        PhotonConnector.OnLobbyJoined -= Loaded;
    }

    public void UpdateUsername()
    {
        OnUpdateUsername?.Invoke(_userNameInput.text);
    }

    public void Loading()
    {
        loadScreen.SetActive(true);
    }

    public void Loaded()
    {
        loadScreen.SetActive(false);
    }
}
