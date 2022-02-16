using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using Photon.Pun.UtilityScripts;

public enum GameState
{
    Waiting = 0,
    Starting = 1,
    Playing = 2,
    Ending = 3
}

public class PhotonGameController : MonoBehaviourPunCallbacks
{
    public static PhotonGameController Instance;
    public int matchLength = 180;
    public bool perpetual = false;

    public GameObject mapCam;

    private int currentMatchTime;
    private Coroutine timerCoroutine;

    private GameState state = GameState.Waiting;

    private void Awake()
    {
        if (!PhotonNetwork.InRoom)
            SceneManager.LoadScene(0);

        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    /*
    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        int team = PhotonNetwork.LocalPlayer.GetPhotonTeam().Code;
        PhotonNetwork.Instantiate(Path.Combine("Managers", "PlayerManager"), Vector3.zero, Quaternion.identity, 0, new object[] { team });
    }
    */

    private void InitializeTimer()
    {
        currentMatchTime = matchLength;
        RefreshTimerUI();

        if (PhotonNetwork.IsMasterClient)
        {
            timerCoroutine = StartCoroutine(Timer());
        }
    }

    private void RefreshTimerUI()
    {
        string minutes = (currentMatchTime / 60).ToString("00");
        string secondes = (currentMatchTime % 60).ToString("00");
        ui_timer.text = $"{minutes}:{secondes}";
    }

    private void EndGame()
    {
        state = GameState.Ending;

        if(timerCoroutine)
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }
}