using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponReferences : MonoBehaviour
{
    [Header("Weapon Base")]
    public ParticleSystem muzzleFlash;
    public Transform muzzle;
    public Transform caseEjector;
    public Animator animator;
    [Header("Weapon Customization")]
    public Transform magPos;
    public Transform sightPos;
    public GameObject rail;
    public Transform cantedSightPos;
    public GameObject cantedRail;
    public Transform underBarrelPos;
    public Transform upperBarrelPos;
    public Transform rightSideBarrelPos;
    public Transform leftSideBarrelPos;
    public Transform muzzlePos;
    public Transform stockPos;
    public MeshRenderer[] meshCamos;
    [Header("Magazin")]
    public WeaponMagazin currentMag;
}