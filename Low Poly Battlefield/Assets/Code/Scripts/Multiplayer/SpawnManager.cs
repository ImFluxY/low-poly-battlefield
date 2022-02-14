using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;

    GameObject[] terroristTeamSpawns;
    GameObject[] counterTerroristTeamSpawns;

    private void Awake()
    {
        instance = this;
        terroristTeamSpawns = GameObject.FindGameObjectsWithTag("TerroristSpawn");
        counterTerroristTeamSpawns = GameObject.FindGameObjectsWithTag("CounterTerroristSpawn");
    }

    public Transform GetRandomTerroristSpawn()
    {
        return terroristTeamSpawns[Random.Range(0, terroristTeamSpawns.Length)].transform;
    }

    public Transform GetRandomCounterTerroristSpawn()
    {
        return counterTerroristTeamSpawns[Random.Range(0, counterTerroristTeamSpawns.Length)].transform;
    }

    public Transform GetTeamSpawn(int teamNumber)
    {
        return teamNumber == 0 ? GetRandomCounterTerroristSpawn() : GetRandomTerroristSpawn();
    }
}
