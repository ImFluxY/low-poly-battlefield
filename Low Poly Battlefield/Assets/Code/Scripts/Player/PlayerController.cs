using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float maxHealth;
    [SerializeField]
    private float currentHealth;
    [Space]
    [SerializeField]
    private Transform dismembredTransform;
    [SerializeField]
    private GameObject[] dismemberedPrefabs;

    [SerializeField]
    private MonoBehaviour[] disableOnDeath;

    private Ragdoll ragdoll;
    private PhotonView PV;
    [SerializeField]
    private PlayerManager playerManager;
    private bool isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
        PV = GetComponent<PhotonView>();
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
        ragdoll = GetComponent<Ragdoll>();
    }

    private void Start()
    {
        isDead = false;
    }

    public void Update()
    {
        if (!PV.IsMine)
            return;

        if (Input.GetKeyDown(KeyCode.K))
            TakeDamage(100);
    }

    public int GetPlayerTeam()
    {
        return playerManager.GetTeam();
    }

    public void TakeDamage(float damage)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        if (!PV.IsMine || isDead)
            return;

        currentHealth -= damage;
        if (currentHealth <= 0.0f)
        {
            Die();
        }
    }

    /*
    public void Dismemberment()
    {
        for (int i = 0; i < dismemberedPrefabs.Length; i++)
        {
            Instantiate(dismemberedPrefabs[i], dismembredTransform.position, dismembredTransform.rotation);
        }
        Destroy(gameObject);
    }
    */

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        PV.RPC("RPC_Die", RpcTarget.All);
        playerManager.Invoke("Die", 5);
    }

    [PunRPC]
    void RPC_Die()
    {
        GetComponent<SimpleWeapon>().detachWeapon();
        GetComponent<PlayerCamera>().deleteCamera();

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        ragdoll.ActivateRagdoll();
    }
}
