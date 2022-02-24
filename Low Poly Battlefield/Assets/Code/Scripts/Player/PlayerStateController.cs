using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateController : MonoBehaviour
{
    public static PlayerStateController Instance;

    private PlayerController localPlayerController;
    private PlayerCamera localPlayerCamera;
    private PlayerLocomotion localPlayerLocomotion;
    private SimpleWeapon localPlayerWeapon;

    private void Awake()
    {
        Instance = this;
    }

    public void SetLocalPlayer(GameObject _localPlayer)
    {
        this.localPlayerController = _localPlayer.GetComponent<PlayerController>();
        this.localPlayerCamera = _localPlayer.GetComponent<PlayerCamera>();
        this.localPlayerLocomotion = _localPlayer.GetComponent<PlayerLocomotion>();
        this.localPlayerWeapon = _localPlayer.GetComponent<SimpleWeapon>();
    }

    public void EnableLocalPlayerControl()
    {
        localPlayerLocomotion.control = true;
        localPlayerController.control = true;
        localPlayerWeapon.control = true;
    }

    public void DisableLocalPleyrControl()
    {
        localPlayerLocomotion.control = false;
        localPlayerController.control = false;
        localPlayerWeapon.control = false;
    }
}
