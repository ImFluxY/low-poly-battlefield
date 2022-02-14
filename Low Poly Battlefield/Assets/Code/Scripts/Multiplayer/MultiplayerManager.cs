using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MultiplayerManager : MonoBehaviour
{
    public GameObject playerObj;

    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
            return;

        PhotonNetwork.Instantiate(playerObj.name, Vector3.zero, Quaternion.identity);
    }
}
