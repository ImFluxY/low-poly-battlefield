using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsViewMine : MonoBehaviour
{
    PhotonView PV;

    [SerializeField]
    private MonoBehaviour[] enableIfIsMine;
    [SerializeField]
    private GameObject[] destroyIfIsNotMine;

    public bool destroyOthersCameras = true;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        for (int i = 0; i < enableIfIsMine.Length; i++)
        {
            if (PV.IsMine)
            {
                enableIfIsMine[i].enabled = true;
            }
            else
            {
                enableIfIsMine[i].enabled = false;
            }
        }

        for (int i = 0; i < destroyIfIsNotMine.Length; i++)
        {
            if (!PV.IsMine)
            {
                Destroy(destroyIfIsNotMine[i]);
            }
        }

        /*
        if (destroyOthersCameras)
        {
            Camera[] allCameras = FindObjectsOfType<Camera>();
            foreach(Camera cam in allCameras)
            {
                if (!cam.GetComponentInParent<PhotonView>().IsMine)
                {
                    Destroy(cam);
                }
            }
        }
        */
    }
}
