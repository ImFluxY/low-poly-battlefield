using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGraphicsController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] defaultTerroristGraphics;
    [SerializeField]
    private GameObject[] defaultCounterGraphics;

    PlayerController playerController;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        UpdateGraphics(playerController.GetPlayerTeam());
    }

    private void UpdateGraphics(int team)
    {
        foreach (GameObject graphic in defaultTerroristGraphics)
        {
            graphic.SetActive(team == 2);
        }

        foreach (GameObject graphic in defaultCounterGraphics)
        {
            graphic.SetActive(team == 1);
        }

    }
}
