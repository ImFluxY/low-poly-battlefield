using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIPlayerCard : MonoBehaviour
{
     [SerializeField] private TMP_Text level;
     [SerializeField] private TMP_Text username;
     [SerializeField] private TMP_Text score;
     [SerializeField] private TMP_Text kills;
     [SerializeField] private TMP_Text deaths;

    public void UpdateInfos(PlayerInfo playerInfo)
    {
        level.text = playerInfo.profile.level.ToString("00");
        username.text = playerInfo.profile.username;
        score.text = (playerInfo.kills * 100).ToString();
        kills.text = playerInfo.kills.ToString();
        deaths.text = playerInfo.deaths.ToString();
    }
}
