using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;

public class EggSpawner : MonoBehaviour
{
    public AbstractMap map;
    public GameObject eggPrefab;
    public List<Vector2d> eggLocations;

    private Transform playerTransform;

    void Start()
    {
        playerTransform = Camera.main.transform;

        // Wait until map is initialized
        map.OnInitialized += SpawnEggs;
    }

    void SpawnEggs()
    {
        foreach (Vector2d geoPos in eggLocations)
        {
            Vector3 worldPos = map.GeoToWorldPosition(geoPos, true);
            Debug.Log($"Spawning egg at lat:{geoPos.x}, lon:{geoPos.y} -> world pos:{worldPos}");

            Instantiate(eggPrefab, worldPos, Quaternion.identity, map.transform);
        }
    }
}
