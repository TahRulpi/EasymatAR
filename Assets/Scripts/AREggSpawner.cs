using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
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

    [Header("Egg Prefabs")]
    public GameObject redEggPrefab;
    public GameObject greenEggPrefab;
    public GameObject purpleEggPrefab;
    public GameObject goldenEggPrefab;
    public float eggScale = 0.5f; // Adjust size
    public float floorY = 0f;     // Fixed floor height

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
            finalArPos.y = floorY; // fixed floor Y

            // Spawn egg as child of AR Origin
            GameObject prefab = GetPrefabByType(data.eggType);
            GameObject egg = Instantiate(prefab, finalArPos, Quaternion.identity, arOrigin.transform);

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


/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;
using TMPro;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Unity.Utilities;
using Mapbox.Unity.Location;

using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class AREggSpawner : MonoBehaviour
{
    [Header("Setup References")]
    public XROrigin arOrigin;
    public TextMeshProUGUI statusText;
    public AbstractMap map; // Drag your 'MapboxAR_Internal' here

    [Header("Egg Prefabs")]
    public GameObject redEggPrefab;
    public GameObject greenEggPrefab;
    public GameObject purpleEggPrefab;
    public GameObject goldenEggPrefab;
    public float eggScale = 10.0f;

    public ARRaycastManager raycastManager;

    private bool spawned = false;

    void OnEnable()
    {
        if (statusText != null) statusText.text = "Initializing AR GPS...";
        StartCoroutine(SpawnEggsRoutine());
    }

    IEnumerator SpawnEggsRoutine()
    {
        // 1. Wait for AR Tracking
        while (ARSession.state != ARSessionState.SessionTracking) yield return null;

        // 2. Wait for Compass Alignment
        ARCompassAligner aligner = arOrigin.GetComponent<ARCompassAligner>();
        if (aligner != null)
        {
            while (!aligner.isAligned)
            {
                if (statusText != null) statusText.text = "Calibrating Compass...";
                yield return null;
            }
        }

        // 3. Proceed with Spawning (Your existing code)
        if (statusText != null) statusText.text = "GPS Ready! Spawning Eggs...";
        SpawnEggsAtGpsLocations();
    }



    *//*void SpawnEggsAtGpsLocations()
    {
        if (spawned) return;
        if (EggManager.Instance == null || EggManager.Instance.eggsToSpawn.Count == 0)
        {
            if (statusText != null) statusText.text = "No eggs found in Manager!";
            return;
        }

        // Player GPS and AR origin
        var locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
        Vector2d playerGps = locationProvider.CurrentLocation.LatitudeLongitude;
        Vector3 playerArPos = arOrigin.transform.position;

        // Optional: map center update
        map.SetCenterLatitudeLongitude(playerGps);
        map.UpdateMap();

        float arScale = 0.001f; // Adjust based on your map scale
        SubscriptionTier tier = GameManager.Instance.currentTier;

        foreach (var data in EggManager.Instance.eggsToSpawn)
        {
            if (!CanUserSeeEgg(tier, data.eggType)) continue;

            // GPS offset relative to player
            Vector2d eggGps = new Vector2d(data.latitude, data.longitude);
            Vector3 worldOffset = Conversions.GeoToWorldPosition(eggGps, playerGps, map.WorldRelativeScale).ToVector3xz();
            worldOffset *= arScale;

            // Limit distance if needed
            if (worldOffset.magnitude > 50f)
                worldOffset = worldOffset.normalized * 50f;

            Vector3 finalArPos = playerArPos + worldOffset;

            // --- Ground Detection ---
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            Vector3 rayOrigin = finalArPos + Vector3.up * 2f; // Start raycast above egg
            Vector3 rayDir = Vector3.down;

            if (raycastManager.Raycast(new Ray(rayOrigin, rayDir), hits, TrackableType.PlaneWithinBounds))
            {
                finalArPos.y = hits[0].pose.position.y;
            }
            else
            {
                // fallback: place slightly below camera height
                finalArPos.y = arOrigin.Camera.transform.position.y - 1.2f;
            }

            // Spawn prefab
            GameObject prefab = GetPrefabByType(data.eggType);
            GameObject egg = Instantiate(prefab, finalArPos, Quaternion.identity, null);

            // Scale properly
            float sizeMultiplier = 10f;
            egg.transform.localScale = new Vector3(eggScale * sizeMultiplier, eggScale * 1.3f * sizeMultiplier, eggScale * sizeMultiplier);

            // Optional: face the player
            Vector3 lookTarget = arOrigin.Camera.transform.position;
            lookTarget.y = egg.transform.position.y; // Keep upright
            egg.transform.LookAt(lookTarget);
            egg.transform.Rotate(0, 180f, 0);

            Debug.Log($"Spawned {data.eggType} at {finalArPos} AR scale: {egg.transform.localScale}");
        }

        spawned = true;
        if (statusText != null)
            statusText.text = $"Spawned {EggManager.Instance.eggsToSpawn.Count} eggs in AR!";
    }
*//*

    void SpawnEggsAtGpsLocations()
    {
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

        float arScale = 0.01f; // tweak this based on testing

        SubscriptionTier tier = GameManager.Instance.currentTier;

        foreach (var data in EggManager.Instance.eggsToSpawn)
        {
            if (!CanUserSeeEgg(tier, data.eggType)) continue;

            Vector2d eggGps = new Vector2d(data.latitude, data.longitude);

            // Convert GPS to AR world position
            Vector3 offset = Conversions.GeoToWorldPosition(eggGps, playerGps, map.WorldRelativeScale).ToVector3();
            offset *= arScale; // apply scale

            Vector3 finalArPos = playerArPos + offset;

            // Raycast to find ground if available
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            Vector3 rayOrigin = finalArPos + Vector3.up * 2f;
            if (raycastManager.Raycast(new Ray(rayOrigin, Vector3.down), hits, TrackableType.PlaneWithinBounds))
            {
                finalArPos.y = hits[0].pose.position.y;
            }
            else
            {
                finalArPos.y = arOrigin.Camera.transform.position.y - 0.5f; // safe fallback
            }

            // Spawn the egg as child of AR Origin
            GameObject prefab = GetPrefabByType(data.eggType);
            GameObject egg = Instantiate(prefab, finalArPos, Quaternion.identity, arOrigin.transform);

            // Proper 3D scaling
            float sizeMultiplier = 0.5f;
            egg.transform.localScale = Vector3.one * eggScale * sizeMultiplier;

            // Face the camera
            Vector3 lookTarget = arOrigin.Camera.transform.position;
            lookTarget.y = egg.transform.position.y;
            egg.transform.LookAt(lookTarget);
            egg.transform.Rotate(0, 180f, 0);

            Debug.Log($"Spawned {data.eggType} at {finalArPos}, scale: {egg.transform.localScale}");
        }

        spawned = true;
        if (statusText != null)
            statusText.text = $"Spawned {EggManager.Instance.eggsToSpawn.Count} eggs in AR!";
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
}*/


/*using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;
using TMPro; // Required for the status text

public class AREggSpawner : MonoBehaviour
{
    [Header("Setup References")]
    public XROrigin arOrigin;
    public TextMeshProUGUI statusText; // Assign this in the Inspector

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
        // 1. Wait for main camera
        while (Camera.main == null)
        {
            if (statusText != null) statusText.text = "Waiting for Camera...";
            yield return null;
        }

        // 2. Wait until ARSession tracking is ready
        // This addresses the "kNotTracking" state seen in your logs
        while (ARSession.state != ARSessionState.SessionTracking)
        {
            if (statusText != null)
                statusText.text = $"Status: {ARSession.state}\nMove phone slowly...";
            yield return null;
        }

        if (statusText != null) statusText.text = "Tracking Ready! Spawning...";
        yield return new WaitForSeconds(0.5f);

        SpawnEggs();

        if (statusText != null) statusText.text = ""; // Clear text after spawning
    }

    *//*void SpawnEggs()
    {
        if (spawned) return;

        // Ensure EggManager exists and has data
        if (EggManager.Instance == null || EggManager.Instance.eggsToSpawn.Count == 0)
        {
            if (statusText != null) statusText.text = "Error: No eggs found in Manager!";
            return;
        }

        Camera cam = Camera.main;

        // Calculate position 1.5m in front of the CAMERA's current view
        Vector3 startPos = cam.transform.position + cam.transform.forward * 1.5f;
        float spacing = 0.5f;
        int index = 0;

        foreach (var data in EggManager.Instance.eggsToSpawn)
        {
            // Arrange eggs in a line using the camera's right vector
            Vector3 arPos = startPos + cam.transform.right * spacing * index;
            arPos.y = cam.transform.position.y - 0.3f; // Spawn slightly below eye level

            GameObject prefab = GetPrefabByType(data.eggType);
            GameObject egg = Instantiate(prefab, arPos, Quaternion.identity, arOrigin.transform);

            egg.transform.localScale = Vector3.one * eggScale;
            egg.transform.LookAt(cam.transform);
            egg.transform.Rotate(0, 180f, 0);

            Debug.Log($"Egg spawned: {data.eggType} at ARPos {arPos}");
            index++;
        }

        spawned = true;
    }*//*

    void SpawnEggs()
    {
        if (spawned) return;

        if (EggManager.Instance == null || EggManager.Instance.eggsToSpawn.Count == 0)
        {
            if (statusText != null) statusText.text = "Error: No eggs found in Manager!";
            return;
        }

        Camera cam = Camera.main;
        Vector3 startPos = cam.transform.position + cam.transform.forward * 1.5f;
        float spacing = 0.5f;
        int index = 0;

        // Get current tier from GameManager
        SubscriptionTier tier = GameManager.Instance.currentTier;

        foreach (var data in EggManager.Instance.eggsToSpawn)
        {
            // --- SUBSCRIPTION CHECK START ---
            bool canSee = false;

            if (tier == SubscriptionTier.None)
            {
                // Only Red and Green are visible for free users
                if (data.eggType == EggType.Red || data.eggType == EggType.Green)
                {
                    canSee = true;
                }
            }
            else
            {
                // Pro and Premium can see everything
                canSee = true;
            }

            if (!canSee)
            {
                Debug.Log($"Skipping {data.eggType} egg due to subscription level.");
                continue;
            }
            // --- SUBSCRIPTION CHECK END ---

            Vector3 arPos = startPos + cam.transform.right * spacing * index;
            arPos.y = cam.transform.position.y - 0.3f;

            GameObject prefab = GetPrefabByType(data.eggType);
            GameObject egg = Instantiate(prefab, arPos, Quaternion.identity, arOrigin.transform);

            egg.transform.localScale = Vector3.one * eggScale;
            egg.transform.LookAt(cam.transform);
            egg.transform.Rotate(0, 180f, 0);

            Debug.Log($"Egg spawned: {data.eggType} at ARPos {arPos}");
            index++;
        }

        spawned = true;
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
}*/

