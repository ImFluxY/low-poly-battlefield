using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;
    GameObject controller;
    int controllerTeam;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        controllerTeam = (int)PV.InstantiationData[0];
        Debug.Log(controllerTeam);
    }

    private void Start()
    {
        if(PV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        Debug.Log("Instantiate Player Controller");
        Transform spawn = SpawnManager.instance.GetTeamSpawn(controllerTeam);
        controller = PhotonNetwork.Instantiate(Path.Combine("Characters", "Player"), spawn.position, spawn.rotation, 0, new object[] { PV.ViewID });
    }

    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();
    }

    public int GetTeam()
    {
        return controllerTeam;
    }
}
