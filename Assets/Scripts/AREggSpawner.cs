using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;
using TMPro;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Unity.Location;

public class AREggSpawner : MonoBehaviour
{
    [Header("Setup References")]
    public XROrigin arOrigin;
    public TextMeshProUGUI statusText;
    public ARRaycastManager raycastManager;

    [Header("Egg Prefabs")]
    public GameObject redEggPrefab;
    public GameObject greenEggPrefab;
    public GameObject purpleEggPrefab;
    public GameObject goldenEggPrefab;
    public float eggScale = 0.5f;
    private bool spawned = false;

    void OnEnable()
    {
        if (statusText != null) statusText.text = "Initializing AR...";
        StartCoroutine(SpawnEggsRoutine());
    }

    IEnumerator SpawnEggsRoutine()
    {
        // Wait for AR tracking
        while (ARSession.state != ARSessionState.SessionTracking)
        {
            if (statusText != null)
                statusText.text = $"Waiting for AR tracking ({ARSession.state})";
            yield return null;
        }

        if (statusText != null) statusText.text = "AR Ready! Spawning Eggs...";
        yield return new WaitForSeconds(0.3f);

        SpawnEggsAtFloor();
    }

    void SpawnEggsAtFloor()
    {
        if (spawned) return;
        if (EggManager.Instance == null || EggManager.Instance.eggsToSpawn.Count == 0)
        {
            if (statusText != null) statusText.text = "No eggs in manager!";
            return;
        }

        Vector3 playerArPos = arOrigin.transform.position;
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        foreach (var data in EggManager.Instance.eggsToSpawn)
        {
            GameObject prefab = GetPrefabByType(data.eggType);
            Vector3 spawnPos = playerArPos + Vector3.forward * 1.5f; // TEMP forward

            // Raycast to floor
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            if (raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
            {
                spawnPos = hits[0].pose.position;
                Debug.Log($"? Floor detected, spawning {data.eggType}");
            }
            else
            {
                Debug.Log("? No floor detected, skipping egg for now");
                continue;
            }

            GameObject egg = Instantiate(prefab, spawnPos, Quaternion.identity, arOrigin.transform);
            egg.transform.localScale = Vector3.one * eggScale;

            // Face camera
            Vector3 lookTarget = arOrigin.Camera.transform.position;
            lookTarget.y = egg.transform.position.y;
            egg.transform.LookAt(lookTarget);
            egg.transform.Rotate(0, 180f, 0);

            // Assign EggBehavior AR flag + geoPosition for other scripts
            EggBehavior eggBehavior = egg.GetComponent<EggBehavior>();
            if (eggBehavior != null)
            {
                eggBehavior.isARMode = true;
                eggBehavior.geoPosition = new Mapbox.Utils.Vector2d(data.latitude, data.longitude);
            }

            Debug.Log($"Spawned {data.eggType} at {spawnPos}");
        }

        spawned = true;
        if (statusText != null)
            statusText.text = $"Spawned {EggManager.Instance.eggsToSpawn.Count} eggs on floor!";
    }

    GameObject GetPrefabByType(EggType type)
    {
        return type switch
        {
            EggType.Red => redEggPrefab,
            EggType.Green => greenEggPrefab,
            EggType.Purple => purpleEggPrefab,
            EggType.Golden => goldenEggPrefab,
            _ => redEggPrefab
        };
    }
}


/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;
using TMPro;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Unity.Utilities;
using Mapbox.Unity.Location;


public class AREggSpawner : MonoBehaviour
{
    [Header("Setup References")]
    public XROrigin arOrigin;
    public TextMeshProUGUI statusText;
    public AbstractMap map; // Your Mapbox AR map
    public ARRaycastManager raycastManager;

    [Header("Egg Prefabs")]
    public GameObject redEggPrefab;
    public GameObject greenEggPrefab;
    public GameObject purpleEggPrefab;
    public GameObject goldenEggPrefab;
    public float eggScale = 0.5f; // Adjust size
    public float floorY = 0.3f;     // Fixed floor height

    private bool spawned = false;

    void OnEnable()
    {
        if (statusText != null) statusText.text = "Initializing AR GPS...";
        StartCoroutine(SpawnEggsRoutine());
    }

    IEnumerator SpawnEggsRoutine()
    {
        // Wait for AR tracking
        while (ARSession.state != ARSessionState.SessionTracking)
        {
            if (statusText != null)
                statusText.text = $"Waiting for AR tracking... ({ARSession.state})";
            yield return null;
        }

        if (statusText != null) statusText.text = "GPS Ready! Spawning Eggs...";
        yield return new WaitForSeconds(0.3f);

        SpawnEggsAtGpsLocations();
    }


    void SpawnEggsAtGpsLocations()
    {
        Debug.Log("SpawnEggsAtGpsLocations START");
        


        if (spawned) return;
        if (EggManager.Instance == null || EggManager.Instance.eggsToSpawn.Count == 0)
        {
            if (statusText != null) statusText.text = "No eggs found in Manager!";
            return;
        }

        var locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
        Vector2d playerGps = locationProvider.CurrentLocation.LatitudeLongitude;
        Vector3 playerArPos = arOrigin.transform.position;

        map.SetCenterLatitudeLongitude(playerGps);
        map.UpdateMap();

        float arScale = 0.01f; // Adjust map scale
        SubscriptionTier tier = GameManager.Instance.currentTier;

        foreach (var data in EggManager.Instance.eggsToSpawn)
        {
            if (!CanUserSeeEgg(tier, data.eggType)) continue;

            // Convert GPS to world position
            Vector2d geoPos2d = Conversions.GeoToWorldPosition(
                new Vector2d(data.latitude, data.longitude),
                playerGps,
                map.WorldRelativeScale
            );

            // Convert to Unity Vector3
            Vector3 offset = new Vector3((float)geoPos2d.x, 0f, (float)geoPos2d.y);
            offset *= arScale;

            // Place egg on floor
            Vector3 finalArPos = playerArPos + offset;
            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            Vector2 screenCenter = new Vector2(
                Screen.width / 2f,
                Screen.height / 2f
            );

            if (raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
            {
                finalArPos.y = hits[0].pose.position.y;
                Debug.Log("? Floor detected, spawning egg");

            }
            else
            {
                Debug.Log("? No floor detected yet");
                return;
            }


            // Spawn egg as child of AR Origin
            GameObject prefab = GetPrefabByType(data.eggType);
            GameObject egg = Instantiate(prefab, finalArPos, Quaternion.identity, arOrigin.transform);

            EggBehavior eggBehavior = egg.GetComponent<EggBehavior>();
            if (eggBehavior != null)
            {
                eggBehavior.isARMode = true; // important
                eggBehavior.geoPosition = new Vector2d(data.latitude, data.longitude); // optional, for other scripts
            }

            // Scale properly
            egg.transform.localScale = Vector3.one * eggScale;

            // Face camera
            Vector3 lookTarget = arOrigin.Camera.transform.position;
            lookTarget.y = egg.transform.position.y;
            egg.transform.LookAt(lookTarget);
            egg.transform.Rotate(0, 180f, 0);

            Debug.Log($"Spawned {data.eggType} at {finalArPos}, scale: {egg.transform.localScale}");
        }

        spawned = true;
        if (statusText != null)
            statusText.text = $"Spawned {EggManager.Instance.eggsToSpawn.Count} eggs on the floor!";
    }

    bool CanUserSeeEgg(SubscriptionTier tier, EggType type)
    {
        if (tier == SubscriptionTier.None)
            return (type == EggType.Red || type == EggType.Green);
        return true;
    }

    GameObject GetPrefabByType(EggType type)
    {
        return type switch
        {
            EggType.Red => redEggPrefab,
            EggType.Green => greenEggPrefab,
            EggType.Purple => purpleEggPrefab,
            EggType.Golden => goldenEggPrefab,
            _ => redEggPrefab
        };
    }
}
*/