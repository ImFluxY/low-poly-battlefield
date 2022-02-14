using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Ragdoll : MonoBehaviour
{
    [SerializeField]
    private Rigidbody[] rigidbodies;
    private Animator animator;

    private PhotonView PV;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
    }

    public void ActivateRagdoll()
    {
        PV.RPC("RPC_ActivateRagdoll", RpcTarget.All);
    }

    [PunRPC]
    void RPC_ActivateRagdoll()
    {
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = false;
        }
        animator.enabled = false;
    }

    public void DesactivateRagdoll()
    {
        PV.RPC("RPC_DesactivateRagdoll", RpcTarget.All);
    }

    [PunRPC]
    void RPC_DesactivateRagdoll()
    {
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = true;
        }
        animator.enabled = true;
    }
}
