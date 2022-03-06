using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class PlayerController : MonoBehaviour, IDamageable
{
  public bool control;

  [SerializeField] private float maxHealth;
  [SerializeField] private float currentHealth;
  [Space]
  [SerializeField] private Transform dismembredTransform;
  [SerializeField] private GameObject[] dismemberedPrefabs;

  [SerializeField] private MonoBehaviour[] disableOnDeath;

  private Ragdoll ragdoll;
  private PhotonView PV;
  [SerializeField] private PlayerManager playerManager;
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
    if (!PV.IsMine || !control)
      return;

    if (Input.GetKeyDown(KeyCode.K))
      TakeDamage(100, PhotonNetwork.LocalPlayer.ActorNumber);
  }

  public int GetPlayerTeam()
  {
    return playerManager.GetMyTeam();
  }

  public void TakeDamage(float damage, int actor)
  {
    PV.RPC("RPC_TakeDamage", RpcTarget.All, damage, actor);
  }

  [PunRPC]
  public void RPC_TakeDamage(float damage, int actor)
  {
    if (!PV.IsMine)
      return;

    currentHealth -= damage;

    if (currentHealth <= 0)
    {
      playerManager.Die(actor);
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

  public void Die()
  {
    PV.RPC("RPC_Die", RpcTarget.All);
  }

  [PunRPC]
  void RPC_Die()
  {
    GetComponent<SimpleWeapon>().detachWeapon();
    GetComponent<PlayerCamera>().DisableCamera();

    ragdoll.ActivateRagdoll();
  }
}
