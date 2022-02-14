using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum CharacterHeight
{
    StandUp,
    Crouch,
}

public enum MovementState
{
    Idle,
    Walk,
    Run
}

public class PlayerLocomotion : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("States")]
    public CharacterHeight characterHeight;
    public MovementState movementState;
    public bool characterGrounded;

    [Header("Parameters")]
    [SerializeField]
    private float gravity = -9.81f;
    [SerializeField]
    private float walkSpeed = 1f;
    [SerializeField]
    private float runSpeed = 2f;

    private CharacterController controller;
    private Vector3 moveDirection;
    private Vector3 velocity;
    private float moveSpeed;

    private InputManager inputManager;
    private SimpleWeapon weapon;

    private void Start()
    {
        inputManager = FindObjectOfType<InputManager>();
        weapon = GetComponent<SimpleWeapon>();
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (PV.IsMine && !Pause.paused)
        {
            ChangeMovementState();
            ChangeCharacterHeight();
            ApplyMovements();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(movementState);
        }
        else if (stream.IsReading)
        {
            movementState = (MovementState)stream.ReceiveNext();
        }
    }

    private void ChangeMovementState()
    {
        if(inputManager.Forward != 0 || inputManager.Sideway != 0)
        {
            if(inputManager.Running && inputManager.Forward > 0 && (weapon != null && !weapon.isAiming))
            {
                movementState = MovementState.Run;
                moveSpeed = runSpeed;
            }
            else
            {
                movementState = MovementState.Walk;
                moveSpeed = walkSpeed;
            }
        }
        else
        {
            movementState = MovementState.Idle;
            moveSpeed = 0;
        }
    }

    private void ChangeCharacterHeight()
    {
        switch (characterHeight)
        {
            case CharacterHeight.StandUp:
                if (inputManager.Crouch)
                {
                    characterHeight = CharacterHeight.Crouch;
                }
                break;
            case CharacterHeight.Crouch:
                if (inputManager.Crouch || movementState == MovementState.Run)
                {
                    characterHeight = CharacterHeight.StandUp;
                }
                break;
            default:
                break;
        }
    }

    private void ApplyMovements()
    {
        characterGrounded = controller.isGrounded;

        if (characterGrounded)
            velocity.y = -1f;
        else
            velocity.y = gravity * Time.deltaTime;

        moveDirection = transform.TransformDirection(new Vector3(inputManager.Sideway, 0f, inputManager.Forward)).normalized;

        controller.Move(moveDirection * moveSpeed * Time.deltaTime);
        controller.Move(velocity);
    }
}
