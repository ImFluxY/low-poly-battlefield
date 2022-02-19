using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIGameController : MonoBehaviour
{
    [SerializeField] private TMP_Text _uiMykills;
    private TMP_Text _uiMydeaths;
    private TMP_Text _uiTimer;
    private Transform _uiLeaderboard;
    //private Transform _uiEndgame;

    //public static Action OnAction = delegate { };

    private void Awake()
    {
        PhotonGameController.OnInitializeUI += HandleInitialize;
        PhotonGameController.OnRefreshStatsUI += HandleRefreshStats;
        PhotonGameController.OnRefreshTimerUI += HandleRefreshTimer;
    }

    private void OnDestroy()
    {
        PhotonGameController.OnInitializeUI -= HandleInitialize;
        PhotonGameController.OnRefreshStatsUI -= HandleRefreshStats;
        PhotonGameController.OnRefreshTimerUI -= HandleRefreshTimer;
    }

    private void HandleInitialize()
    {
        _uiMykills = GameObject.Find("HUD/Stats/Kills/Text").GetComponent<TMP_Text>();
        _uiMydeaths = GameObject.Find("HUD/Stats/Deaths/Text").GetComponent<TMP_Text>();
        _uiTimer = GameObject.Find("HUD/Timer/Text").GetComponent<TMP_Text>();
        _uiLeaderboard = GameObject.Find("HUD").transform.Find("Leaderboard").transform;
        //_uiEndgame = GameObject.Find("Canvas").transform.Find("End Game").transform;
    }

    private void HandleRefreshStats (List<PlayerInfo> playerInfos, int myind)
    {
        if (playerInfos.Count > myind)
        {
            _uiMykills.text = $"{playerInfos[myind].kills} kills";
            _uiMydeaths.text = $"{playerInfos[myind].deaths} deaths";
        }
        else
        {
            _uiMykills.text = "0 kills";
            _uiMydeaths.text = "0 deaths";
        }
    }

    private void HandleRefreshTimer(int currentMatchTime)
    {
        string minutes = (currentMatchTime / 60).ToString("00");
        string seconds = (currentMatchTime % 60).ToString("00");
        _uiTimer.text = $"{minutes}:{seconds}";
    }
}
