using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMagazin : MonoBehaviour
{
    public bool magInWeapon;
    public Magazin properties;
    [SerializeField] private int currentAmmoCount;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public int getCurrentAmmo()
    {
        return currentAmmoCount;
    }

    public void setCurrentAmmo(int newAmmoCount)
    {
        this.currentAmmoCount = newAmmoCount;
    }

    public void setRbKinematic(bool isKinematic){
        this.rb.isKinematic = isKinematic;
    }
}