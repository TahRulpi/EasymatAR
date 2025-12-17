using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using System;

public class EggSpawner : MonoBehaviour
{
    public AbstractMap map;
    public GameObject eggPrefab;
    public List<Vector2d> eggLocations;
    public Transform player;

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

        // Wait until map has a center and scale
        while (map.CenterLatitudeLongitude == Vector2d.zero || map.WorldRelativeScale <= 0)
        {
            yield return null;
        }

        if (spawned) yield break;
        spawned = true;

        Debug.Log("?? Map ready — spawning eggs");
        SpawnEggs();
    }

    void SpawnEggs()
    {
        Debug.Log("?? SpawnEggs() CALLED");

        foreach (Vector2d geoPos in eggLocations)
        {
            GameObject egg = Instantiate(eggPrefab);
            Debug.Log("?? Egg instantiated");

            EggBehavior behavior = egg.GetComponent<EggBehavior>();
            behavior.geoPosition = geoPos;
            behavior.map = map;
            behavior.player = player;
            behavior.spawnTime = DateTime.Now;

            Vector3 worldPos = map.GeoToWorldPosition(geoPos, true);
            egg.transform.position = worldPos;

            Debug.Log("?? Egg world pos: " + worldPos);
        }
    }
}
