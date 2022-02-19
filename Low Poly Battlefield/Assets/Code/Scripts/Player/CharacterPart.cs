using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CharacterPart : MonoBehaviour
{
    [SerializeField]
    private float partMultiplier = 1f;
    private PlayerController playerController;

    private void Start()
    {
        playerController = transform.GetComponentInParent<PlayerController>();
    }

    public void OnHit(float damage, int actor)
    {
        playerController.TakeDamage(damage * partMultiplier, actor);
    }
}