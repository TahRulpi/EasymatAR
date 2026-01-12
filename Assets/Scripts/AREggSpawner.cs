using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;
using TMPro;
using Mapbox.Utils;
using Mapbox.Unity.Location;
using Mapbox.Unity.Utilities;

public class AREggSpawner : MonoBehaviour
{
    [Header("AR Setup")]
    public XROrigin arOrigin;
    public ARRaycastManager raycastManager;
    public TextMeshProUGUI statusText;

    [Header("Egg Prefabs")]
    public GameObject redEggPrefab;
    public GameObject greenEggPrefab;
    public GameObject purpleEggPrefab;
    public GameObject goldenEggPrefab;

    [Header("Egg Settings")]
    public float eggScale = 1f;// Bigger for 3D
    public float floorOffset = 0.08f; // Lift egg above floor
    float eggHeight = prefab.GetComponent<Renderer>().bounds.size.y;
    private bool spawned = false;
    private readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Update()
    {
        if (spawned) return;
        if (EggManager.Instance == null || EggManager.Instance.eggsToSpawn.Count == 0) return;

        // 1?? Raycast at screen center to detect floor
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        if (!raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneEstimated | TrackableType.FeaturePoint))
        {
            statusText?.SetText("?? Move phone slowly to find floor...");
            return;
        }

        Vector3 floorPos = hits[0].pose.position;

        // 2?? Get GPS location with fallback
        var playerLoc = LocationProviderFactory.Instance.DefaultLocationProvider.CurrentLocation;
        Vector2d playerLatLon;

        /*if (!playerLoc.IsLocationUpdated)
        {
            // Use default test coordinates (allows editor testing or GPS not ready)
            playerLatLon = new Vector2d(23.752407, 90.352974);
            statusText?.SetText("?? GPS not ready ? using test coordinates");
            Debug.Log("?? GPS not ready ? using test coordinates");
        }
        else
        {
            playerLatLon = playerLoc.LatitudeLongitude;
            statusText?.SetText("?? GPS ready ? placing eggs");
        }*/
        if (!playerLoc.IsLocationUpdated)
        {
            // Use location from Map scene
            playerLatLon = PlayerLocationHolder.LastKnownLocation;
            statusText?.SetText("?? Using map location for AR");
            Debug.Log($"?? GPS not ready ? using map location: {playerLatLon.x}, {playerLatLon.y}");
        }
        else
        {
            playerLatLon = playerLoc.LatitudeLongitude;
            statusText?.SetText("?? GPS ready ? placing eggs");
        }


        // 3?? Spawn eggs in AR
        foreach (var data in EggManager.Instance.eggsToSpawn)
        {
            if (data.isCollected)
                continue; // ?? DO NOT SPAWN COLLECTED EGGS
            GameObject prefab = GetPrefabByType(data.eggType);
            if (prefab == null) continue;

            Vector2d eggLatLon = new Vector2d(data.latitude, data.longitude);

            // Convert GPS to meters relative to player
            Vector2d playerMeters = Conversions.LatLonToMeters(playerLatLon);
            Vector2d eggMeters = Conversions.LatLonToMeters(eggLatLon);
            Vector2d relativeMeters = eggMeters - playerMeters;

            Vector3 offset = new Vector3((float)relativeMeters.x, 0, (float)relativeMeters.y);

            // Limit distance for AR visibility
            if (offset.magnitude > 15f)
                offset = offset.normalized * 15f;

            // Vector3 finalPos = floorPos + offset + Vector3.up * floorOffset;
            Vector3 finalPos = floorPos + Vector3.up * floorOffset - Vector3.up * (eggHeight / 2f);

            // Spawn egg
            GameObject egg = Instantiate(prefab, finalPos, Quaternion.identity, arOrigin.transform);

            // ? Make egg 3D and visible
            egg.transform.localScale = Vector3.one * eggScale;

            // Face camera
            Vector3 lookTarget = new Vector3(arOrigin.Camera.transform.position.x, egg.transform.position.y, arOrigin.Camera.transform.position.z);
            egg.transform.LookAt(lookTarget);
            egg.transform.Rotate(0, 180f, 0);

            // Assign GPS to EggBehavior
            EggBehavior eb = egg.GetComponent<EggBehavior>();
            if (eb != null)
            {
                eb.isARMode = true;
                eb.geoPosition = eggLatLon;
            }

            Debug.Log($"? AR Spawned {data.eggType} at {relativeMeters.magnitude:F1} meters");
        }

        spawned = true;
        statusText?.SetText("?? AR Eggs spawned on real floor!");
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

/*using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;
using TMPro;
using Mapbox.Utils;
using Mapbox.Unity.Location;
using Mapbox.Unity.Utilities;

public class AREggSpawner : MonoBehaviour
{
    [Header("AR Setup")]
    public XROrigin arOrigin;
    public ARRaycastManager raycastManager;
    public TextMeshProUGUI statusText;

    [Header("Egg Prefabs")]
    public GameObject redEggPrefab;
    public GameObject greenEggPrefab;
    public GameObject purpleEggPrefab;
    public GameObject goldenEggPrefab;

    [Header("Egg Settings")]
    public float eggScale = 1.3f;
    public float floorOffset = 0.02f; // small lift to avoid clipping

    private bool spawned = false;
    private readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Update()
    {
        if (spawned) return;
        if (EggManager.Instance == null || EggManager.Instance.eggsToSpawn.Count == 0) return;

        // 1?? Raycast at screen center to detect floor
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        if (!raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneEstimated | TrackableType.FeaturePoint))
        {
            statusText?.SetText("?? Move phone slowly to find floor...");
            return;
        }

        Vector3 floorPos = hits[0].pose.position;

        // 2?? Get GPS location
        var playerLoc = LocationProviderFactory.Instance.DefaultLocationProvider.CurrentLocation;
        if (!playerLoc.IsLocationUpdated)
        {
            statusText?.SetText("?? Waiting for GPS signal...");
            return;
        }

        Vector2d playerLatLon = playerLoc.LatitudeLongitude;

        // 3?? Spawn eggs
        foreach (var data in EggManager.Instance.eggsToSpawn)
        {
            GameObject prefab = GetPrefabByType(data.eggType);
            if (prefab == null) continue;

            Vector2d eggLatLon = new Vector2d(data.latitude, data.longitude);

            // Convert GPS to meters relative to player
            Vector2d playerMeters = Conversions.LatLonToMeters(playerLatLon);
            Vector2d eggMeters = Conversions.LatLonToMeters(eggLatLon);
            Vector2d relativeMeters = eggMeters - playerMeters;

            Vector3 offset = new Vector3((float)relativeMeters.x, 0, (float)relativeMeters.y);

            // Limit distance for AR visibility
            if (offset.magnitude > 15f) offset = offset.normalized * 15f;

            Vector3 finalPos = floorPos + offset + Vector3.up * floorOffset;

            // Spawn egg
            GameObject egg = Instantiate(prefab, finalPos, Quaternion.identity, arOrigin.transform);
            egg.transform.localScale = Vector3.one * eggScale;

            // Face camera
            Vector3 lookTarget = new Vector3(arOrigin.Camera.transform.position.x, egg.transform.position.y, arOrigin.Camera.transform.position.z);
            egg.transform.LookAt(lookTarget);
            egg.transform.Rotate(0, 180f, 0);

            // Assign GPS to EggBehavior
            EggBehavior eb = egg.GetComponent<EggBehavior>();
            if (eb != null)
            {
                eb.isARMode = true;
                eb.geoPosition = eggLatLon;
            }

            Debug.Log($"? Spawned {data.eggType} at {relativeMeters.magnitude:F1} meters");
        }

        spawned = true;
        statusText?.SetText("?? Eggs spawned on real floor!");
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

/*using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;
using TMPro;
using Mapbox.Utils;
using Mapbox.Unity.Location;

public class AREggSpawner : MonoBehaviour
{
    [Header("AR")]
    public XROrigin arOrigin;
    public ARRaycastManager raycastManager;
    public TextMeshProUGUI statusText;

    [Header("Egg Prefabs")]
    public GameObject redEggPrefab;
    public GameObject greenEggPrefab;
    public GameObject purpleEggPrefab;
    public GameObject goldenEggPrefab;

    public float eggScale = 0.6f;
    public float floorOffset = 0.02f;

    private bool spawned = false;
    private readonly List<ARRaycastHit> hits = new();

    *//* void Update()
     {
         // 1?? Already spawned or no eggs? Stop
         if (spawned) return;
         if (EggManager.Instance == null) return;
         if (EggManager.Instance.eggsToSpawn.Count == 0) return;

         // 2?? Get center of screen for raycast
         Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

         // 3?? Raycast to detect floor / feature point
         bool hit = raycastManager.Raycast(
             screenCenter,
             hits,
             TrackableType.PlaneEstimated | TrackableType.FeaturePoint
         );

         if (!hit)
         {
             // Floor not detected yet
             if (statusText != null)
                 statusText.text = "?? Move phone slowly & look at floor";
             Debug.Log("?? Raycast hit: False");
             return;
         }

         Debug.Log("?? Raycast hit: True");

         // 4?? Get current GPS location
         var playerLoc = LocationProviderFactory.Instance.DefaultLocationProvider.CurrentLocation;

         if (!playerLoc.IsLocationUpdated)
         {
             // GPS not ready yet
             if (statusText != null)
                 statusText.text = "?? Waiting for GPS...";
             Debug.Log("?? GPS not ready");
             return;
         }

         Vector2d playerLatLon = playerLoc.LatitudeLongitude;

         // 5?? Floor detected and GPS ready ? Spawn eggs
         Vector3 floorPos = hits[0].pose.position;

         foreach (var data in EggManager.Instance.eggsToSpawn)
         {
             GameObject prefab = GetPrefabByType(data.eggType);
             if (prefab == null) continue;

             // ?? Convert map egg GPS to AR world relative to player
             double distanceMeters = Vector2d.Distance(playerLatLon, new Vector2d(data.latitude, data.longitude)) * 1000.0;
             double bearing = GetBearing(playerLatLon, new Vector2d(data.latitude, data.longitude));

             // Clamp distance so it fits in AR world
             distanceMeters = Mathf.Clamp((float)distanceMeters, 1f, 20f);

             Vector3 offset = new Vector3(
                 (float)(distanceMeters * Mathf.Sin((float)bearing)),
                 0,
                 (float)(distanceMeters * Mathf.Cos((float)bearing))
             );

             Vector3 approxPos = floorPos + offset;

             // Raycast again to drop egg on exact floor
             List<ARRaycastHit> eggHits = new List<ARRaycastHit>();
             if (!raycastManager.Raycast(arOrigin.Camera.WorldToScreenPoint(approxPos),
                                          eggHits,
                                          TrackableType.PlaneEstimated | TrackableType.FeaturePoint))
             {
                 Debug.Log("?? Floor not found for this egg, skipping");
                 continue;
             }

             Vector3 finalPos = eggHits[0].pose.position + Vector3.up * 0.02f;

             GameObject egg = Instantiate(prefab, finalPos, Quaternion.identity, arOrigin.transform);
             egg.transform.localScale = Vector3.one * eggScale;

             // Face camera
             Vector3 lookTarget = arOrigin.Camera.transform.position;
             lookTarget.y = egg.transform.position.y;
             egg.transform.LookAt(lookTarget);
             egg.transform.Rotate(0, 180f, 0);

             // Assign egg behavior
             EggBehavior eb = egg.GetComponent<EggBehavior>();
             if (eb != null)
             {
                 eb.isARMode = true;
                 eb.geoPosition = new Vector2d(data.latitude, data.longitude);
             }

             Debug.Log($"? Spawned {data.eggType} at {distanceMeters:F1} meters from player");
         }

         spawned = true;

         if (statusText != null)
             statusText.text = "?? Eggs placed on real floor!";

         Debug.Log("?? Eggs placed on real floor!");
     }
 *//*

    void Update()
    {
        // 1?? Already spawned or no eggs? Stop
        if (spawned) return;
        if (EggManager.Instance == null) return;
        if (EggManager.Instance.eggsToSpawn.Count == 0) return;

        // 2?? Raycast to detect floor / feature points
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        bool hit = raycastManager.Raycast(
            screenCenter,
            hits,
            TrackableType.PlaneEstimated | TrackableType.FeaturePoint
        );

        if (!hit)
        {
            // Floor not detected yet
            if (statusText != null)
                statusText.text = "?? Move phone slowly & look at floor";
            Debug.Log("?? Raycast hit: False");
            return;
        }

        Debug.Log("?? Raycast hit: True");

        // 3?? Get player's GPS location
        var playerLoc = LocationProviderFactory.Instance.DefaultLocationProvider.CurrentLocation;

        Vector2d playerLatLon;

        if (!playerLoc.IsLocationUpdated)
        {
            // TEMP: assign test coordinates for quick AR testing
            playerLatLon = new Vector2d(23.752407, 90.352974);
            if (statusText != null)
                statusText.text = "?? GPS not ready ? using test coordinates";
            Debug.Log("?? GPS not ready ? using test coordinates");
        }
        else
        {
            playerLatLon = playerLoc.LatitudeLongitude;
        }

        // 4?? Floor detected AND GPS ready (or test) ? Spawn eggs
        Vector3 floorPos = hits[0].pose.position;

        foreach (var data in EggManager.Instance.eggsToSpawn)
        {
            GameObject prefab = GetPrefabByType(data.eggType);
            if (prefab == null) continue;

            // 4a?? Calculate distance & bearing from player to egg
            double distanceMeters = Vector2d.Distance(playerLatLon, new Vector2d(data.latitude, data.longitude)) * 1000.0;
            double bearing = GetBearing(playerLatLon, new Vector2d(data.latitude, data.longitude));

            // Clamp distance for AR world
            distanceMeters = Mathf.Clamp((float)distanceMeters, 1f, 20f);

            // 4b?? Convert distance & bearing to AR offset
            Vector3 offset = new Vector3(
                (float)(distanceMeters * Mathf.Sin((float)bearing)),
                0,
                (float)(distanceMeters * Mathf.Cos((float)bearing))
            );

            Vector3 approxPos = floorPos + offset;

            // 4c?? Optional: Raycast again to snap egg on exact floor
            List<ARRaycastHit> eggHits = new List<ARRaycastHit>();
            if (raycastManager.Raycast(arOrigin.Camera.WorldToScreenPoint(approxPos),
                                        eggHits,
                                        TrackableType.PlaneEstimated | TrackableType.FeaturePoint))
            {
                approxPos = eggHits[0].pose.position + Vector3.up * 0.02f;
            }

            // 4d?? Spawn egg in AR
            GameObject egg = Instantiate(prefab, approxPos, Quaternion.identity, arOrigin.transform);
            egg.transform.localScale = Vector3.one * eggScale;

            // Face camera
            Vector3 lookTarget = arOrigin.Camera.transform.position;
            lookTarget.y = egg.transform.position.y;
            egg.transform.LookAt(lookTarget);
            egg.transform.Rotate(0, 180f, 0);

            // 4e?? Assign EggBehavior
            EggBehavior eb = egg.GetComponent<EggBehavior>();
            if (eb != null)
            {
                eb.isARMode = true;
                eb.geoPosition = new Vector2d(data.latitude, data.longitude);
            }

            Debug.Log($"? Spawned {data.eggType} at {distanceMeters:F1} meters from player");
        }

        // 5?? Done spawning
        spawned = true;
        if (statusText != null)
            statusText.text = "?? Eggs placed on real floor!";
        Debug.Log("?? Eggs placed on real floor!");
    }



    void SpawnEggsUsingGPS(Vector3 floorCenter)
    {
        var playerLoc = LocationProviderFactory.Instance.DefaultLocationProvider.CurrentLocation;

        if (!playerLoc.IsLocationUpdated)
        {
            Debug.LogError("? GPS not ready");
            return;
        }


        Vector2d playerLatLon = playerLoc.LatitudeLongitude;

        foreach (var data in EggManager.Instance.eggsToSpawn)
        {
            GameObject prefab = GetPrefabByType(data.eggType);
            if (prefab == null) continue;

            // ?? GPS ? distance & bearing
            double distanceMeters = Vector2d.Distance(playerLatLon,
                new Vector2d(data.latitude, data.longitude)) * 1000.0;

            double bearing = GetBearing(playerLatLon,
                new Vector2d(data.latitude, data.longitude));

            // ?? Convert to AR offset
            Vector3 offset = new Vector3(
                (float)(distanceMeters * Mathf.Sin((float)bearing)),
                0,
                (float)(distanceMeters * Mathf.Cos((float)bearing))
            );

            Vector3 approxPos = floorCenter + offset;

            // ?? Drop to floor again
            if (!raycastManager.Raycast(
                arOrigin.Camera.WorldToScreenPoint(approxPos),
                hits,
                TrackableType.PlaneWithinPolygon))
            {
                Debug.Log("?? Floor not found for this egg, skipping");
                continue;
            }

            Vector3 finalPos = hits[0].pose.position + Vector3.up * floorOffset;

            GameObject egg = Instantiate(prefab, finalPos, Quaternion.identity, arOrigin.transform);
            egg.transform.localScale = Vector3.one * eggScale;

            egg.transform.rotation = Quaternion.Euler(
                0,
                arOrigin.Camera.transform.eulerAngles.y,
                0
            );

            EggBehavior eb = egg.GetComponent<EggBehavior>();
            if (eb != null)
            {
                eb.isARMode = true;
                eb.geoPosition = new Vector2d(data.latitude, data.longitude);
            }

            Debug.Log($"? Spawned {data.eggType} at {distanceMeters:F1} meters");

            Debug.Log($"Egg spawned: {data.eggType} @ lat {data.latitude}, lon {data.longitude}");

        }
    }

    double GetBearing(Vector2d from, Vector2d to)
    {
        double lat1 = from.x * Mathf.Deg2Rad;
        double lon1 = from.y * Mathf.Deg2Rad;
        double lat2 = to.x * Mathf.Deg2Rad;
        double lon2 = to.y * Mathf.Deg2Rad;

        double dLon = lon2 - lon1;

        double y = Mathf.Sin((float)dLon) * Mathf.Cos((float)lat2);
        double x =
            Mathf.Cos((float)lat1) * Mathf.Sin((float)lat2) -
            Mathf.Sin((float)lat1) * Mathf.Cos((float)lat2) * Mathf.Cos((float)dLon);

        return Mathf.Atan2((float)y, (float)x);
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