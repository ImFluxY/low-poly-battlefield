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
            if (team == 0)
            {
                graphic.SetActive(false);
            }
            else
            {
                graphic.SetActive(true);
            }
        }

        foreach (GameObject graphic in defaultCounterGraphics)
        {
            if (team == 0)
            {
                graphic.SetActive(true);
            }
            else
            {
                graphic.SetActive(false);
            }
        }

    }
}
