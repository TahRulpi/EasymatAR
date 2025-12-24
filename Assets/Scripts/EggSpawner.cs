using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;

public class EggSpawner : MonoBehaviour
{
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

    [Header("Egg Input Settings")]
    public List<EggData> eggInputList = new List<EggData>(); // Set this in Inspector or via code

    [Header("AR Ready List")]
    public List<EggBehavior> spawnedEggs = new List<EggBehavior>(); // For AR spawning

    private bool spawned = false;

    void Start()
    {
        if (map == null)
        {
            Debug.LogError("? Map is NULL! Assign it in Inspector.");
            return;
        }

        StartCoroutine(WaitForMapAndSpawn());
    }

    IEnumerator WaitForMapAndSpawn()
    {
        Debug.Log("? Waiting for Mapbox map to initialize...");

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
            Debug.LogWarning("? No egg input provided!");
            return;
        }

        foreach (EggData data in eggInputList)
        {
            GameObject prefab = GetPrefabByType(data.eggType);
            if (prefab == null) continue;

            GameObject egg = Instantiate(prefab);
            egg.SetActive(true);

            EggBehavior behavior = egg.GetComponent<EggBehavior>();
            if (behavior != null)
            {
                behavior.geoPosition = new Vector2d(data.latitude, data.longitude);
                behavior.map = map;
                behavior.player = player;
                behavior.spawnTime = DateTime.Now;
                behavior.eggType = data.eggType;
                behavior.isCollectable = true; // or use subscription logic if you want
                spawnedEggs.Add(behavior);
            }

            Vector3 worldPos = map.GeoToWorldPosition(new Vector2d(data.latitude, data.longitude), true);
            egg.transform.position = worldPos;

            Debug.Log($"?? Egg spawned: {data.eggType} at {worldPos}");
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
