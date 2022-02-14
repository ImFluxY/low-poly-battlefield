using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField]
    private string forwardParameter = "Forward";
    [SerializeField]
    private string sidewayParameter = "Sideway";
    [SerializeField]
    private float dampTime = .1f;
    [SerializeField]
    private string runningParameter = "Running";
    [SerializeField]
    private string crouchParameter = "Crouch";

    private Animator animator;
    private InputManager inputManager;
    private PlayerLocomotion playerLocomotion;

    private void Start()
    {
        animator = GetComponent<Animator>();
        inputManager = FindObjectOfType<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    private void Update()
    {
        if (Pause.paused)
            return;

        ApplyToAnimator();
    }

    private void ApplyToAnimator()
    {
        animator.SetFloat(forwardParameter, inputManager.Forward, dampTime, Time.deltaTime);
        animator.SetFloat(sidewayParameter, inputManager.Sideway, dampTime, Time.deltaTime);
        animator.SetBool(runningParameter, playerLocomotion.movementState == MovementState.Run);
        animator.SetBool(crouchParameter, playerLocomotion.characterHeight == CharacterHeight.Crouch);
    }
}
