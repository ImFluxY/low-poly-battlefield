using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public bool control;

    #region Variables
    [Header("Movements")]
    [SerializeField]
    private string m_forwardAxis = "Vertical";
    [SerializeField]
    private string m_sidewayAxis = "Horizontal";

    private KeyCode m_runningKey = KeyCode.LeftShift;
    protected bool m_running;
    public bool Running
    {
        get { return m_running; }
    }

    private KeyCode m_crouchKey = KeyCode.C;
    protected bool m_crouch;
    public bool Crouch
    {
        get { return m_crouch; }
    }

    [Header("Weapon Keys")]
    public KeyCode fireKey = KeyCode.Mouse0;

    private KeyCode m_aimKey = KeyCode.Mouse1;
    protected bool m_aiming;
    public bool Aiming
    {
        get { return m_aiming; }
    }

    private KeyCode m_reloadKey = KeyCode.R;
    protected bool m_reloading;
    public bool Reloading
    {
        get { return m_reloading; }
    }

    public void setReloading(bool reloading)
    {
        m_reloading = reloading;
    }

    private KeyCode m_fireModeKey = KeyCode.V;
    protected bool m_changeFireMode;
    public bool ChangeFireMode
    {
        get { return m_changeFireMode; }
    }

    private KeyCode m_leanLeftKey = KeyCode.A;
    protected bool m_leanLeft;
    public bool LeanLeft
    {
        get { return m_leanLeft; }
    }

    private KeyCode m_leanRightKey = KeyCode.E;
    protected bool m_leanRight;
    public bool LeanRight
    {
        get { return m_leanRight; }
    }

    [Header("Look")]
    [SerializeField]
    private string m_verticalLookAxis = "Mouse Y";
    [SerializeField]
    private string m_horizontalLookAxis = "Mouse X";
    [SerializeField]
    private float m_xAxisSensitivity = 80f;
    [SerializeField]
    private float m_yAxisSensitivity = 80f;
    [SerializeField]
    private int sensitivityDivider = 1;
    [SerializeField]
    private sightDivider[] sightDividers;

    public void SetSightDivider(SightRatio divider)
    {
        sensitivityDivider = 1;

        foreach (sightDivider sightDivider in sightDividers)
        {
            if (sightDivider.sightRatio == divider)
            {
                sensitivityDivider = sightDivider.sightDivision;
            }
        }
    }

    private KeyCode m_freeLookKey = KeyCode.LeftAlt;
    protected bool m_freeLook;
    public bool FreeLook
    {
        get { return m_freeLook; }
    }

    protected float m_forward;
    protected float m_sideway;
    protected float m_xAxis;
    protected float m_yAxis;

    public float XLookAxis
    {
        get { return m_xAxis; }
    }
    public float YLookAxis
    {
        get { return m_yAxis; }
    }

    [Header("Menu / UI")]
    [SerializeField]
    private KeyCode m_pauseKey = KeyCode.Escape;
    protected bool m_pause;
    public bool Pause
    {
        get { return m_pause; }
    }

    [SerializeField]
    private KeyCode m_leaderboardKey = KeyCode.Tab;
    protected bool m_leaderboard;
    public bool Leaderboard
    {
        get { return m_leaderboard; }
    }

    #endregion
    #region Properties
    public float Forward
    {
        get { return m_forward; }
    }
    public float Sideway
    {
        get { return m_sideway; }
    }
    #endregion
    #region BuiltIn Methods
    private void Awake()
    {
        // Instance
        if (Instance != null)
        {
            Debug.LogError("More that one instance of " + this + " in the scene !");
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        HandleInput();
    }
    #endregion
    #region Custom Methods
    protected void HandleInput()
    {
        m_xAxis = Input.GetAxis(m_horizontalLookAxis) * Time.deltaTime * (m_xAxisSensitivity / sensitivityDivider);
        m_yAxis = Input.GetAxis(m_verticalLookAxis) * Time.deltaTime * (m_yAxisSensitivity / sensitivityDivider);
        m_freeLook = Input.GetKey(m_freeLookKey);
        m_leanLeft = Input.GetKey(m_leanLeftKey);
        m_leanRight = Input.GetKey(m_leanRightKey);
        m_pause = Input.GetKeyDown(m_pauseKey);
        m_leaderboard =  Input.GetKey(m_leaderboardKey);
        m_crouch = Input.GetKeyDown(m_crouchKey);
        m_aiming = Input.GetKey(m_aimKey);

        if (!control)
            return;

        m_forward = Input.GetAxisRaw(m_forwardAxis);
        m_sideway = Input.GetAxisRaw(m_sidewayAxis);
        m_changeFireMode = Input.GetKeyDown(m_fireModeKey);
        m_reloading = Input.GetKeyDown(m_reloadKey);
        m_running = Input.GetKey(m_runningKey);
    }
    #endregion
}

[System.Serializable]
public class sightDivider
{
    public SightRatio sightRatio;
    public int sightDivision;
}