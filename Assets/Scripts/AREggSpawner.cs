using Mapbox.Unity.Map;
using Mapbox.Utils;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class AREggSpawner : MonoBehaviour
{
    [Header("Egg Settings")]
    public float eggScale = 0.5f; // Adjust egg size in AR

    public GameObject redEggPrefab;
    public GameObject greenEggPrefab;
    public GameObject purpleEggPrefab;
    public GameObject goldenEggPrefab;

    [Header("References")]
    public AbstractMap map;          // Drag your Mapbox AbstractMap here
    public XROrigin arOrigin; // Drag your AR Session Origin here

    private bool eggsSpawned = false;

    void Start()
    {
        if (arOrigin == null)
        {
            Debug.LogError("ARSessionOrigin not assigned!");
            return;
        }

        if (map == null)
        {
            Debug.LogError("AbstractMap not assigned!");
            return;
        }

        StartCoroutine(SpawnWhenARReady());
    }

    IEnumerator SpawnWhenARReady()
    {
#if UNITY_EDITOR
        yield return new WaitForSeconds(1f); // Editor test
#else
    // ? Correct for AR Foundation 5.x
    while (ARSession.state != ARSessionState.SessionTracking)
    {
        Debug.Log("? Waiting for AR tracking...");
        yield return new WaitForSeconds(0.5f);
    }

    yield return new WaitForSeconds(0.5f);
#endif

        if (!eggsSpawned)
        {
            SpawnEggsInAR();
            eggsSpawned = true;
        }
    }


    void SpawnEggsInAR()
    {
        if (EggManager.Instance == null || EggManager.Instance.eggsToSpawn.Count == 0)
        {
            Debug.LogWarning("No eggs found to spawn in AR!");
            return;
        }

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("No Main Camera found!");
            return;
        }

        float angleStep = 360f / EggManager.Instance.eggsToSpawn.Count;
        int index = 0;

        foreach (var data in EggManager.Instance.eggsToSpawn)
        {
            // --- CHOOSE ONE POSITIONING LOGIC ---

            // OPTION A: Spawn in a circle 2 meters in front of the camera (Best for testing)
            float angle = index * angleStep * Mathf.Deg2Rad;
            Vector3 spawnOffset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * 2.0f; // 2 meters away
            Vector3 arPos = cam.transform.position + cam.transform.forward * 2.0f + spawnOffset;

            /* // OPTION B: Use real GPS (Only works if Mapbox is calibrated)
            Vector3 mapWorldPos = map.GeoToWorldPosition(new Vector2d(data.latitude, data.longitude), true);
            Vector3 arPos = arOrigin.transform.InverseTransformPoint(mapWorldPos); 
            */

            // Instantiate and Scale
            GameObject prefab = GetPrefabByType(data.eggType);
            if (prefab == null) continue;

            GameObject egg = Instantiate(prefab, arPos, Quaternion.identity, arOrigin.transform);
            egg.transform.localScale = Vector3.one * eggScale; // Ensure eggScale is at least 0.5 - 1.0

            // Face the player
            egg.transform.LookAt(cam.transform.position);
            egg.transform.Rotate(0, 180f, 0);

            // Assign Behavior
            EggBehavior behavior = egg.GetComponent<EggBehavior>();
            if (behavior != null)
            {
                behavior.geoPosition = new Vector2d(data.latitude, data.longitude);
                behavior.map = map;
                behavior.player = cam.transform;
                behavior.eggType = data.eggType;
                behavior.isCollectable = true;
            }

            egg.tag = "Egg";
            index++;
        }
    }

    // Return the correct prefab for egg type
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
