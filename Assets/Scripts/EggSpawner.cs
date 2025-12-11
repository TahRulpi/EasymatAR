using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;

public class EggSpawner : MonoBehaviour
{
    public AbstractMap map;
    public GameObject eggPrefab;
    public List<Vector2d> eggLocations;//EggSpawner loads real-world GPS coordinates

    public Transform player; // AR Camera

    void Start()
    {
        if (map == null)
        {
            Debug.LogError("Map is null! Assign it in the Inspector.");
            return;
        }
        else
        {
            Debug.Log("Map assigned, waiting for OnInitialized...");
        }

        // Subscribe to Mapbox initialization
        map.OnInitialized += SpawnEggs;
        SpawnEggs();

        /*#if UNITY_EDITOR
                // For testing in the Editor without a device, call it directly
                SpawnEggs();
        #endif*/

        // Subscribe to Mapbox initialization event
        // map.OnInitialized += SpawnEggs;
    }
   
    void SpawnEggs()
    {
        Debug.Log("?? Map initialized — spawning eggs...");

        foreach (Vector2d geoPos in eggLocations)
        {
            GameObject egg = Instantiate(eggPrefab);
            egg.SetActive(true);

            EggBehavior behavior = egg.AddComponent<EggBehavior>();
            behavior.geoPosition = geoPos;
            behavior.map = map;
            behavior.player = player;

            // Get world position
            Vector3 worldPos = map.GeoToWorldPosition(geoPos, true);

            // Debug confirmations
            Debug.Log("?? EGG SPAWNED!");
            Debug.Log($"?? GPS Provided: lat={geoPos.x}, lon={geoPos.y}");
            Debug.Log($"??? Mapbox World Position: {worldPos}");

            // Check if accidentally at zero
            if (worldPos == Vector3.zero)
            {
                Debug.LogError("?? WARNING: Egg world position is (0,0,0). GPS or Mapbox may be wrong.");
            }
            else
            {
                Debug.Log("? Egg world position is NOT at zero. Conversion OK.");
            }
            Debug.Log("EGG SPAWNED at GPS: " + geoPos);
        }
        


    }

}