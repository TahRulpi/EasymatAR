/*using Mapbox.Unity.Map;
using Mapbox.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class AREggSpawner : MonoBehaviour
{
    public float eggScale = 0.3f;
    public bool spawnDebugCube = true;

    public GameObject redEggPrefab;
    public GameObject greenEggPrefab;
    public GameObject purpleEggPrefab;
    public GameObject goldenEggPrefab;

    public AbstractMap map;
    public List<EggBehavior> mapEggs;

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
        Camera cam = Camera.main;
        foreach (var mapEgg in mapEggs)
        {
            Vector3 worldPos = map.GeoToWorldPosition(mapEgg.geoPosition, true);

            GameObject prefab = GetPrefabByType(mapEgg.eggType);
            GameObject egg = Instantiate(prefab, worldPos, Quaternion.identity);
            egg.transform.localScale = Vector3.one * eggScale;
            egg.transform.SetParent(null);
            egg.transform.LookAt(cam.transform.position);
            egg.transform.Rotate(0, 180f, 0);

            egg.tag = "Egg";

            EggBehavior behavior = egg.GetComponent<EggBehavior>();
            if (behavior != null)
            {
                behavior.geoPosition = mapEgg.geoPosition;
                behavior.map = map;
                behavior.player = cam.transform;
                behavior.eggType = mapEgg.eggType;
                behavior.isCollectable = mapEgg.isCollectable;
            }

            if (spawnDebugCube)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = worldPos;
                cube.transform.localScale = Vector3.one * 0.2f;
                cube.GetComponent<Renderer>().material.color = Color.green;
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
using static EggSpawner;

public class AREggSpawner : MonoBehaviour
{
    public float eggScale = 0.3f;
    public bool spawnDebugCube = true;

    public GameObject redEggPrefab;
    public GameObject greenEggPrefab;
    public GameObject purpleEggPrefab;
    public GameObject goldenEggPrefab;

    public AbstractMap map;

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

        foreach (EggData data in EggManager.Instance.eggsToSpawn)
        {
            Vector3 worldPos = map.GeoToWorldPosition(new Vector2d(data.latitude, data.longitude), true);

            GameObject prefab = GetPrefabByType(data.eggType);
            GameObject egg = Instantiate(prefab, worldPos, Quaternion.identity);
            egg.transform.localScale = Vector3.one * eggScale;
            egg.transform.SetParent(null);
            egg.transform.LookAt(cam.transform.position);
            egg.transform.Rotate(0, 180f, 0);
            egg.tag = "Egg";

            EggBehavior behavior = egg.GetComponent<EggBehavior>();
            if (behavior != null)
            {
                behavior.geoPosition = new Vector2d(data.latitude, data.longitude);
                behavior.map = map;
                behavior.player = cam.transform;
                behavior.eggType = data.eggType;
                behavior.isCollectable = true; // Or set based on subscription if needed
            }

            if (spawnDebugCube)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = worldPos;
                cube.transform.localScale = Vector3.one * 0.2f;
                cube.GetComponent<Renderer>().material.color = Color.green;
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
