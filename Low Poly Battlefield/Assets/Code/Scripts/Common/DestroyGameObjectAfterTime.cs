using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DestroyGameObjectAfterTime : MonoBehaviour
{
    [SerializeField]
    private float time = 5f;

    private void Start()
    {
        StartCoroutine("Fade");
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(time);
        PhotonNetwork.Destroy(gameObject);
    }
}
