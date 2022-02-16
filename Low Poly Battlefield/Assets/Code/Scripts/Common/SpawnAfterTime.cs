using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAfterTime : MonoBehaviour
{
    public GameObject spawnObject;
    public float spawnAfterTime = 5f;
    public bool destroySpawnedObject = true;
    public float destroySpawnedObjectTime = 5f;

    private float timer = 0.0f;

    private void Update()
    {
        timer += Time.deltaTime;

        if(timer >= spawnAfterTime)
        {
            GameObject spawnedObject = Instantiate(spawnObject, transform.position, transform.rotation);

            if (destroySpawnedObject)
                Destroy(spawnedObject, destroySpawnedObjectTime);

            Destroy(gameObject);
        }
    }
}