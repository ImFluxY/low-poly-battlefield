using UnityEngine;
using UnityEngine.Animations.Rigging;
using Photon.Pun;

public class PlayerCamera : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("References")]
    [SerializeField]
    private Transform cam;
    public Transform cameraPivot;
    [SerializeField]
    private Transform cameraOffset;
    [SerializeField]
    private Transform freeCameraPivot;
    [SerializeField]
    private Transform lookOffset;
    [SerializeField]
    private Transform leanOffset;
    [SerializeField]
    private ChainIKConstraint bodyChainIK;
    [SerializeField]
    private Rig cameraRig;

    [Header("Look")]
    [SerializeField]
    [Range(-90, 0)]
    private int minVerticalRotation = -75;
    [SerializeField]
    [Range(0, 90)]
    private int maxVerticalRotation = 25;

    [Header("Free Look")]
    [SerializeField]
    [Range(-90, 0)]
    private int freeLookMinVerticalRotation = -75;
    [SerializeField]
    [Range(0, 90)]
    private int freeLookMaxVerticalRotation = 25;
    [SerializeField]
    [Range(-90, 0)]
    private int freeLookMinHorizontalRoation = -50;
    [SerializeField]
    [Range(0, 90)]
    private int freeLookMaxHorizontalRotation = 50;
    [SerializeField]
    private float freeLookBackTime = .1f;

    [Header("Leaning")]
    [SerializeField]
    private float leaningAngle = 45f;
    [SerializeField]
    private float leaningTime = 10f;
    private bool leaningRight, leaningLeft;

    private float rotationOnXLook = 0f;
    private float rotationOnYLook = 0f;
    private float rotationOnXFreeLook = 0f;
    private float rotationOnYFreeLook = 0f;

    private InputManager inputManager;
    private PlayerLocomotion playerLocomotion;

    private void Awake()
    {
        inputManager = FindObjectOfType<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    private void Start()
    {
        cameraRig.weight = PV.IsMine ? 1 : 0;
    }

    private void Update()
    {
        if (PV.IsMine)
        {
            if(!Pause.paused)
            {
                LookControl();
                LeaningControl();
            }

            Look();
        }

        Leaning();
    }

    private void LookControl()
    {
        if (inputManager.FreeLook)
        {
            if (rotationOnXLook != 0)
                rotationOnXLook = Mathf.Lerp(rotationOnXLook, 0, freeLookBackTime * Time.deltaTime);

            rotationOnXFreeLook -= inputManager.YLookAxis;
            rotationOnXFreeLook = Mathf.Clamp(rotationOnXFreeLook, freeLookMinVerticalRotation, freeLookMaxVerticalRotation);
            rotationOnYFreeLook += inputManager.XLookAxis;
            rotationOnYFreeLook = Mathf.Clamp(rotationOnYFreeLook, freeLookMinHorizontalRoation, freeLookMaxHorizontalRotation);
        }
        else
        {
            if (rotationOnXFreeLook != 0)
                rotationOnXFreeLook = Mathf.Lerp(rotationOnXFreeLook, 0, freeLookBackTime * Time.deltaTime);
            if (rotationOnYFreeLook != 0)
                rotationOnYFreeLook = Mathf.Lerp(rotationOnYFreeLook, 0, freeLookBackTime * Time.deltaTime);

            rotationOnXLook -= inputManager.YLookAxis;
            rotationOnXLook = Mathf.Clamp(rotationOnXLook, minVerticalRotation, maxVerticalRotation);
            rotationOnYLook += inputManager.XLookAxis;
        }
    }

    private void Look()
    {
        lookOffset.localEulerAngles = new Vector3(rotationOnXLook, 0f, 0f);
        cameraPivot.localEulerAngles = new Vector3(rotationOnXLook, 0f, 0f);
        transform.eulerAngles = new Vector3(0f, rotationOnYLook, 0f);
        freeCameraPivot.localEulerAngles = new Vector3(rotationOnXFreeLook, rotationOnYFreeLook, 0f);
    }

    private void LeaningControl()
    {
        leaningRight = inputManager.LeanRight;
        leaningLeft = inputManager.LeanLeft;
    }

    private void Leaning()
    {
        Quaternion targetRotation;

        if (playerLocomotion.movementState == MovementState.Run || inputManager.FreeLook)
        {
            targetRotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            if (leaningLeft)
            {
                targetRotation = Quaternion.Euler(0f, 0f, leaningAngle);
            }
            else if (leaningRight)
            {
                targetRotation = Quaternion.Euler(0f, 0f, -leaningAngle);
            }
            else
            {
                targetRotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }

        leanOffset.localRotation = Quaternion.Lerp(leanOffset.localRotation, targetRotation, Time.deltaTime * leaningTime);
        cameraOffset.localRotation = Quaternion.Lerp(cameraOffset.localRotation, targetRotation, Time.deltaTime * leaningTime);
    }

    public void deleteCamera()
    {
        if (PV.IsMine)
            Destroy(cam.gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(lookOffset.localEulerAngles);
            stream.SendNext(cameraPivot.localEulerAngles);
            stream.SendNext(leaningLeft);
            stream.SendNext(leaningRight);
            stream.SendNext(cameraOffset.localRotation);
        }
        else if(stream.IsReading)
        {
            lookOffset.localEulerAngles = (Vector3)stream.ReceiveNext();
            cameraPivot.localEulerAngles = (Vector3)stream.ReceiveNext();
            leaningLeft = (bool)stream.ReceiveNext();
            leaningRight = (bool)stream.ReceiveNext();
            cameraOffset.localRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
