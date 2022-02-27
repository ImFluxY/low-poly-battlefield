using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIGameController : MonoBehaviour
{
    private TMP_Text _uiTimer;
    private Transform _uiLeaderboard;
    [SerializeField] GameObject playerCardPrefab;
    private bool forceShowLeaderboard;
    //private Transform _uiEndgame;

    private void Awake()
    {
        PhotonGameController.OnInitializeUI += HandleInitialize;
        PhotonGameController.OnRefreshTimerUI += HandleRefreshTimer;
        PhotonGameController.OnRefreshLeaderboard += HandlerUpdateLeaderboard;
        PhotonGameController.OnForceShowLeaderboard += HandleForceShowLeaderboard;
    }

    private void OnDestroy()
    {
        PhotonGameController.OnInitializeUI -= HandleInitialize;
        PhotonGameController.OnRefreshTimerUI -= HandleRefreshTimer;
        PhotonGameController.OnRefreshLeaderboard -= HandlerUpdateLeaderboard;
        PhotonGameController.OnForceShowLeaderboard -= HandleForceShowLeaderboard;
    }

    private void Update()
    {
        if(_uiLeaderboard != null)
            _uiLeaderboard.gameObject.SetActive(InputManager.Instance.Leaderboard || forceShowLeaderboard);
    }

    private void HandleForceShowLeaderboard(bool b)
    {
        forceShowLeaderboard = b;
    }

    private void HandleInitialize()
    {
        _uiTimer = GameObject.Find("HUD/Timer/Text").GetComponent<TMP_Text>();
        _uiLeaderboard = GameObject.Find("HUD").transform.Find("Leaderboard").transform;
        //_uiEndgame = GameObject.Find("Canvas").transform.Find("End Game").transform;
    }

    private void HandleRefreshTimer(int time)
    {
        string minutes = (time / 60).ToString("00");
        string seconds = (time % 60).ToString("00");
        _uiTimer.text = $"{minutes}:{seconds}";
    }

    private void HandlerUpdateLeaderboard(List<PlayerInfo> playerInfos)
    {
        Debug.Log("Update Leaderboard");
        // specify leaderboard
        //if (GameSettings.GameMode == GameMode.FFA) p_lb = p_lb.Find("FFA");
        //if (GameSettings.GameMode == GameMode.TDM) p_lb = p_lb.Find("TDM");

        // clean up
        for (int i = 2; i < _uiLeaderboard.childCount; i++)
        {
            Destroy(_uiLeaderboard.GetChild(i).gameObject);
        }

        // set details
        _uiLeaderboard.Find("Header/Mode").GetComponent<TMP_Text>().text = System.Enum.GetName(typeof(GameMode), GameSettings.GameMode);
        _uiLeaderboard.Find("Header/Map").GetComponent<TMP_Text>().text = SceneManager.GetActiveScene().name;

        // set scores
        /*
        if (GameSettings.GameMode == GameMode.TDM)
        {
            _uiLeaderboard.Find("Header/Score/Home").GetComponent<TMP_Text>().text = "0";
            _uiLeaderboard.Find("Header/Score/Away").GetComponent<TMP_Text>().text = "0";
        }
        */

        // sort
        //List<PlayerInfo> sorted = SortPlayers(playerInfos);

        // display
        //bool t_alternateColors = false;
        foreach (PlayerInfo a in playerInfos)
        {
            Debug.Log("Instantiate New Player Card");
            //GameObject newCard = Instantiate(playerCardPrefab, _uiLeaderboard) as GameObject;
            Instantiate(playerCardPrefab, _uiLeaderboard).GetComponent<UIPlayerCard>().UpdateInfos(a);

            /*
            if (GameSettings.GameMode == GameMode.TDM)
            {
                newcard.transform.Find("Home").gameObject.SetActive(!a.awayTeam);
                newcard.transform.Find("Away").gameObject.SetActive(a.awayTeam);
            }
            */

            //if (t_alternateColors) newcard.GetComponent<Image>().color = new Color32(0, 0, 0, 180);
            //t_alternateColors = !t_alternateColors;

            /*
            newcard.transform.Find("Level").GetComponent<TMP_Text>().text = a.profile.level.ToString("00");
            newcard.transform.Find("Username").GetComponent<TMP_Text>().text = a.profile.username;
            newcard.transform.Find("Score Value").GetComponent<TMP_Text>().text = (a.kills * 100).ToString();
            newcard.transform.Find("Kills Value").GetComponent<TMP_Text>().text = a.kills.ToString();
            newcard.transform.Find("Deaths Value").GetComponent<TMP_Text>().text = a.deaths.ToString();

            newcard.SetActive(true);
            */
        }

        // activate
        _uiLeaderboard.gameObject.SetActive(true);
        _uiLeaderboard.parent.gameObject.SetActive(true);
    }

    private List<PlayerInfo> SortPlayers(List<PlayerInfo> p_info)
    {
        List<PlayerInfo> sorted = new List<PlayerInfo>();

        /*
        if (GameSettings.GameMode == GameMode.FFA)
        {
            while (sorted.Count < p_info.Count)
            {
                // set defaults
                short highest = -1;
                PlayerInfo selection = p_info[0];

                // grab next highest player
                foreach (PlayerInfo a in p_info)
                {
                    if (sorted.Contains(a)) continue;
                    if (a.kills > highest)
                    {
                        selection = a;
                        highest = a.kills;
                    }
                }

                // add player
                sorted.Add(selection);
            }
        }
        */

        List<PlayerInfo> armySorted = new List<PlayerInfo>();
        List<PlayerInfo> terroristSorted = new List<PlayerInfo>();

        int armySize = 0;
        int terroristSize = 0;

        foreach (PlayerInfo p in p_info)
        {
            if (p.team == 1) armySize++;
            else terroristSize++;
        }

        while (armySorted.Count < armySize)
        {
            // set defaults
            short highest = -1;
            PlayerInfo selection = p_info[0];

            // grab next highest player
            foreach (PlayerInfo a in p_info)
            {
                if (a.team == 1) continue;
                if (armySorted.Contains(a)) continue;
                if (a.kills > highest)
                {
                    selection = a;
                    highest = a.kills;
                }
            }

            // add player
            armySorted.Add(selection);
        }

        while (terroristSorted.Count < armySize)
        {
            // set defaults
            short highest = -1;
            PlayerInfo selection = p_info[0];

            // grab next highest player
            foreach (PlayerInfo a in p_info)
            {
                if (a.team == 2) continue;
                if (terroristSorted.Contains(a)) continue;
                if (a.kills > highest)
                {
                    selection = a;
                    highest = a.kills;
                }
            }

            // add player
            terroristSorted.Add(selection);
        }

        sorted.AddRange(armySorted);
        sorted.AddRange(terroristSorted);

        return sorted;
    }
}
