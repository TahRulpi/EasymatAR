/*using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;

public class AREggSpawner : MonoBehaviour
{
    public XROrigin arOrigin;
    public GameObject redEggPrefab;
    public GameObject greenEggPrefab;
    public GameObject purpleEggPrefab;
    public GameObject goldenEggPrefab;
    public float eggScale = 0.5f;

    private bool spawned = false;

    void OnEnable()
    {
        StartCoroutine(SpawnEggsRoutine());
    }

    IEnumerator SpawnEggsRoutine()
    {
        // Wait for main camera
        while (Camera.main == null)
            yield return null;

        // Wait until ARSession tracking is ready
        while (ARSession.state != ARSessionState.SessionTracking)
            yield return null;

        yield return new WaitForSeconds(0.1f);

        SpawnEggs();
    }

    *//*void SpawnEggs()
    {
        if (spawned) return;
        if (EggManager.Instance == null)
        {
            Debug.LogError("EggManager.Instance is NULL in AR scene!");
            return;
        }

        Camera cam = Camera.main;

        if (EggManager.Instance.eggsToSpawn.Count == 0)
        {
            Debug.Log("Eggs list empty in AR scene, spawning default red egg in front of player.");
            Vector3 arPos = arOrigin.transform.position + arOrigin.transform.forward * 1.0f;
            GameObject egg = Instantiate(redEggPrefab, arPos, Quaternion.identity, arOrigin.transform);
            egg.transform.localScale = Vector3.one * eggScale;
            egg.transform.LookAt(cam.transform);
            egg.transform.Rotate(0, 180f, 0);
            Debug.Log("Default red egg spawned at: " + arPos);
            spawned = true;
            return;
        }

        foreach (var data in EggManager.Instance.eggsToSpawn)
        {
            Vector3 offset = GpsToARPosition(
                data.latitude, data.longitude,
                EggManager.Instance.playerLatitude,
                EggManager.Instance.playerLongitude
            );

            Vector3 arPos = arOrigin.transform.position + offset;
            arPos.y += 0.5f;

            GameObject prefab = GetPrefabByType(data.eggType);
            GameObject egg = Instantiate(prefab, arPos, Quaternion.identity, arOrigin.transform);
            egg.transform.localScale = Vector3.one * eggScale;
            egg.transform.LookAt(cam.transform);
            egg.transform.Rotate(0, 180f, 0);

            Debug.Log($"Egg spawned: {data.eggType} at GPS({data.latitude},{data.longitude}) -> ARPos {arPos}");
        }

        spawned = true;
        Debug.Log("All eggs spawned successfully!");
    }*//*

    void SpawnEggs()
    {
        if (spawned) return;
        if (EggManager.Instance == null || EggManager.Instance.eggsToSpawn.Count == 0)
        {
            Debug.LogError("No eggs to spawn in AR!");
            return;
        }

        Camera cam = Camera.main; // Get the actual AR camera

        // Position the start point 1.5m in front of the CAMERA'S current view
        Vector3 startPos = cam.transform.position + cam.transform.forward * 1.5f;
        float spacing = 0.5f;
        int index = 0;

        foreach (var data in EggManager.Instance.eggsToSpawn)
        {
            // Use the camera's right vector for the line arrangement
            Vector3 arPos = startPos + cam.transform.right * spacing * index;

            // Match the height to the camera's height or slightly lower
            arPos.y = cam.transform.position.y - 0.2f;

            GameObject prefab = GetPrefabByType(data.eggType);
            // Still parent to arOrigin to keep them in the AR world scale
            GameObject egg = Instantiate(prefab, arPos, Quaternion.identity, arOrigin.transform);

            egg.transform.localScale = Vector3.one * eggScale;
            egg.transform.LookAt(cam.transform);
            egg.transform.Rotate(0, 180f, 0);

            Debug.Log($"Egg spawned: {data.eggType} at ARPos {arPos}");
            index++;
        }

        spawned = true;
        Debug.Log("All eggs spawned successfully!");
    }


    Vector3 GpsToARPosition(double eggLat, double eggLon, double playerLat, double playerLon)
    {
        const double earthRadius = 6378137;
        double dLat = (eggLat - playerLat) * Mathf.Deg2Rad;
        double dLon = (eggLon - playerLon) * Mathf.Deg2Rad;
        double x = dLon * earthRadius * Mathf.Cos((float)playerLat * Mathf.Deg2Rad);
        double z = dLat * earthRadius;
        return new Vector3((float)x, 0, (float)z);
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


using System.Collections;
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

    void SpawnEggs()
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