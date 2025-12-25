/*using Mapbox.Unity.Map;
using Mapbox.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class AREggSpawner : MonoBehaviour
{
    public float eggScale = 0.5f; // adjust until it looks good
    public bool spawnDebugCube = true;

    public GameObject redEggPrefab;
    public GameObject greenEggPrefab;
    public GameObject purpleEggPrefab;
    public GameObject goldenEggPrefab;

    public AbstractMap map; // your Mapbox map
    public ARSessionOrigin arOrigin; // drag ARSessionOrigin here

    private bool eggSpawned = false;

    void Start()
    {
        StartCoroutine(SpawnWhenReady());
    }

    IEnumerator SpawnWhenReady()
    {
#if UNITY_EDITOR
        yield return new WaitForSeconds(1f); // Editor test
#else
        while (ARSession.state != ARSessionState.SessionTracking)
        {
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(0.5f);
#endif

        if (!eggSpawned)
        {
            SpawnEggsInAR();
            eggSpawned = true;
        }
    }

    void SpawnEggsInAR()
    {
        if (EggManager.Instance == null || EggManager.Instance.eggsToSpawn.Count == 0)
        {
            Debug.LogWarning("No eggs found in EggManager!");
            return;
        }

        Camera cam = Camera.main;

        foreach (var data in EggManager.Instance.eggsToSpawn)
        {
            // Convert Geo to Mapbox world position
            Vector3 mapWorldPos = map.GeoToWorldPosition(new Vector2d(data.latitude, data.longitude), true);

            // Convert Mapbox world to ARSessionOrigin space
            Vector3 arPos = arOrigin.transform.TransformPoint(mapWorldPos);

            GameObject prefab = GetPrefabByType(data.eggType);
            GameObject egg = Instantiate(prefab, arPos, Quaternion.identity, arOrigin.transform);

            // Scale egg
            egg.transform.localScale = Vector3.one * eggScale;

            // Face camera
            egg.transform.LookAt(cam.transform.position);
            egg.transform.Rotate(0, 180f, 0);

            egg.tag = "Egg";

            // Set behavior
            EggBehavior behavior = egg.GetComponent<EggBehavior>();
            if (behavior != null)
            {
                behavior.geoPosition = new Vector2d(data.latitude, data.longitude);
                behavior.map = map;
                behavior.player = cam.transform;
                behavior.eggType = data.eggType;
                behavior.isCollectable = true;
            }

            if (spawnDebugCube)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = arPos;
                cube.transform.localScale = Vector3.one * 0.2f;
                cube.GetComponent<Renderer>().material.color = Color.green;
                cube.transform.SetParent(arOrigin.transform);
            }
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
*/


using Mapbox.Unity.Map;
using Mapbox.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


public class AREggSpawner : MonoBehaviour
{
    public float baseScale = 0.2f; // base egg size
    public bool spawnDebugCube = true;

    public GameObject redEggPrefab;
    public GameObject greenEggPrefab;
    public GameObject purpleEggPrefab;
    public GameObject goldenEggPrefab;

    public AbstractMap map;
    public ARSessionOrigin arOrigin;

    private bool eggSpawned = false;

    /*void Start()
    {
        StartCoroutine(SpawnWhenReady());
    }*/

    void Start()
    {
        if (arOrigin == null)
        {
            Debug.LogError("ARSessionOrigin not assigned!");
        }
        else
        {
            Debug.Log("ARSessionOrigin assigned: " + arOrigin.name);
        }
    }


    IEnumerator SpawnWhenReady()
    {
#if UNITY_EDITOR
        yield return new WaitForSeconds(1f); // Editor test
#else
        while (ARSession.state != ARSessionState.SessionTracking)
        {
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(0.5f);
#endif

        if (!eggSpawned)
        {
            SpawnEggsInAR();
            eggSpawned = true;
        }
    }

    /*void SpawnEggsInAR()
    {
        if (EggManager.Instance == null || EggManager.Instance.eggsToSpawn.Count == 0)
        {
            Debug.LogWarning("No eggs found in EggManager!");
            return;
        }

        Camera cam = Camera.main;

        *//* foreach (var data in EggManager.Instance.eggsToSpawn)
         {
             // Convert Geo to Mapbox world position
             Vector3 mapWorldPos = map.GeoToWorldPosition(new Vector2d(data.latitude, data.longitude), true);

             // Convert Mapbox world to ARSessionOrigin space
             Vector3 arPos = arOrigin.transform.TransformPoint(mapWorldPos);

             // Spawn egg prefab
             GameObject prefab = GetPrefabByType(data.eggType);
             GameObject egg = Instantiate(prefab, arPos, Quaternion.identity, arOrigin.transform);

             // Auto-scale egg based on distance to camera
             float distance = Vector3.Distance(cam.transform.position, egg.transform.position);
             egg.transform.localScale = Vector3.one * baseScale * distance; // scales with distance

             // Face camera
             egg.transform.LookAt(cam.transform.position);
             egg.transform.Rotate(0, 180f, 0);

             egg.tag = "Egg";

             // Set behavior
             EggBehavior behavior = egg.GetComponent<EggBehavior>();
             if (behavior != null)
             {
                 behavior.geoPosition = new Vector2d(data.latitude, data.longitude);
                 behavior.map = map;
                 behavior.player = cam.transform;
                 behavior.eggType = data.eggType;
                 behavior.isCollectable = true;
             }

             // Optional debug cube
             if (spawnDebugCube)
             {
                 GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                 cube.transform.position = arPos;
                 cube.transform.localScale = Vector3.one * 0.2f * distance;
                 cube.GetComponent<Renderer>().material.color = Color.green;
                 cube.transform.SetParent(arOrigin.transform);
             }
         }
     }*//*


        foreach (var data in EggManager.Instance.eggsToSpawn)
        {
            // 1?? Convert Geo to Mapbox world position
            Vector3 mapWorldPos = map.GeoToWorldPosition(new Vector2d(data.latitude, data.longitude), true);

            // 2?? Convert Mapbox world position to AR space
            Vector3 arPos = arOrigin.transform.InverseTransformPoint(mapWorldPos);

            // 3?? Instantiate the egg at the AR position, parented to ARSessionOrigin
            GameObject prefab = GetPrefabByType(data.eggType);
            GameObject egg = Instantiate(prefab, arPos, Quaternion.identity, arOrigin.transform);

            // 4?? Scale and orient the egg
            egg.transform.localScale = Vector3.one * baseScale;
            egg.transform.LookAt(Camera.main.transform.position);
            egg.transform.Rotate(0, 180f, 0);

            egg.tag = "Egg";

            // 5?? Optional: Set behavior
            EggBehavior behavior = egg.GetComponent<EggBehavior>();
            if (behavior != null)
            {
                behavior.geoPosition = new Vector2d(data.latitude, data.longitude);
                behavior.map = map;
                behavior.player = Camera.main.transform;
                behavior.eggType = data.eggType;
                behavior.isCollectable = true;
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
    }*/


    void SpawnEggsInAR()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("No Main Camera found!");
            return;
        }

        Vector3 testPos = cam.transform.position + cam.transform.forward * 1f;
        GameObject egg = Instantiate(redEggPrefab, testPos, Quaternion.identity, arOrigin.transform);
        egg.transform.localScale = Vector3.one * 0.5f;
        egg.transform.LookAt(cam.transform.position);
        egg.transform.Rotate(0, 180f, 0);
        egg.tag = "Egg";

        Debug.Log("Test egg spawned at: " + testPos);
    }

}