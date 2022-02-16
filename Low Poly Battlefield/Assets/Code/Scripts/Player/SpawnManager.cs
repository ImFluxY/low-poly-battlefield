using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;

    GameObject[] terroristTeamSpawns;
    GameObject[] armyTeamSpawns;

    private void Awake()
    {
        instance = this;
        terroristTeamSpawns = GameObject.FindGameObjectsWithTag("Terrorist Spawn");
        armyTeamSpawns = GameObject.FindGameObjectsWithTag("Army Spawn");
    }

    public Transform GetRandomTerroristSpawn()
    {
        return terroristTeamSpawns[Random.Range(0, terroristTeamSpawns.Length)].transform;
    }

    public Transform GetRandomArmySpawn()
    {
        return armyTeamSpawns[Random.Range(0, armyTeamSpawns.Length)].transform;
    }

    public Transform GetTeamSpawn(int teamNumber)
    {
        return teamNumber == 1 ? GetRandomArmySpawn() : GetRandomTerroristSpawn();
    }
}
