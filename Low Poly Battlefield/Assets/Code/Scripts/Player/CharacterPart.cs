using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPart : MonoBehaviour
{
    [SerializeField]
    private float partMultiplier = 1f;
    private PlayerController playerController;

    private void Start()
    {
        playerController = transform.GetComponentInParent<PlayerController>();
    }

    public void OnHit(float damage)
    {
        playerController.TakeDamage(damage * partMultiplier);
    }
}