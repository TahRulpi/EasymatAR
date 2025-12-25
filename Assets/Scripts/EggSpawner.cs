using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;

public class EggSpawner : MonoBehaviour
{
    [Header("AR Ready List (DO NOT TOUCH AT RUNTIME)")]
    public List<EggBehavior> spawnedEggs = new List<EggBehavior>();

    [Header("Map Settings")]
    public AbstractMap map;
    public Transform player;

    [System.Serializable]
    public class EggData
    {
        public EggType eggType;     // Red, Green, Purple, Golden
        public double latitude;     // GPS latitude
        public double longitude;    // GPS longitude
    }

    [Header("Egg Prefabs")]
    public GameObject redEggPrefab;
    public GameObject greenEggPrefab;
    public GameObject purpleEggPrefab;
    public GameObject goldenEggPrefab;

    [Header("Egg Input List (GPS based)")]
    public List<EggData> eggInputList = new List<EggData>();

    private bool spawned = false;

    void Start()
    {
        if (map == null)
        {
            Debug.LogError("? Map is NULL! Assign AbstractMap in Inspector.");
            return;
        }

        StartCoroutine(WaitForMapAndSpawn());
    }

    IEnumerator WaitForMapAndSpawn()
    {
        Debug.Log("?? Waiting for Mapbox map to initialize...");

        while (map.CenterLatitudeLongitude == Vector2d.zero || map.WorldRelativeScale <= 0)
        {
            yield return null;
        }

        if (spawned) yield break;
        spawned = true;

        Debug.Log("? Map ready — spawning eggs");
        SpawnEggs();
    }

    void SpawnEggs()
    {
        if (eggInputList == null || eggInputList.Count == 0)
        {
            Debug.LogWarning("?? No egg input provided!");
            return;
        }

        spawnedEggs.Clear(); // VERY IMPORTANT

        foreach (EggData data in eggInputList)
        {
            GameObject prefab = GetPrefabByType(data.eggType);
            if (prefab == null)
            {
                Debug.LogWarning("? Missing prefab for egg type: " + data.eggType);
                continue;
            }

            Vector2d gpsPos = new Vector2d(data.latitude, data.longitude);
            Vector3 worldPos = map.GeoToWorldPosition(gpsPos, true);

            GameObject egg = Instantiate(prefab, worldPos, Quaternion.identity);
            egg.SetActive(true);

            EggBehavior behavior = egg.GetComponent<EggBehavior>();
            if (behavior == null)
            {
                Debug.LogError("? Egg prefab missing EggBehavior component!");
                Destroy(egg);
                continue;
            }

            // ?? Bind data
            behavior.geoPosition = gpsPos;
            behavior.map = map;
            behavior.player = player;
            behavior.spawnTime = DateTime.Now;
            behavior.eggType = data.eggType;
            behavior.isCollectable = true;

            // ? REGISTER FOR AR
            spawnedEggs.Add(behavior);

            Debug.Log($"?? Egg registered for AR: {data.eggType} @ {gpsPos}");
        }
    }

    GameObject GetPrefabByType(EggType type)
    {
        switch (type)
        {
            case EggType.Red: return redEggPrefab;
            case EggType.Green: return greenEggPrefab;
            case EggType.Purple: return purpleEggPrefab;
            case EggType.Golden: return goldenEggPrefab;
            default: return redEggPrefab;
        }
    }
}
