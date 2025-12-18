using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using System;

public class EggSpawner : MonoBehaviour
{
    public AbstractMap map;
    public GameObject redEggPrefab;
    public GameObject greenEggPrefab;
    public GameObject purpleEggPrefab;
    public GameObject goldenEggPrefab;
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

        SubscriptionTier tier = GameManager.Instance.currentTier;

        int eggsToSpawn = 5;

        List<EggType> eggsToGenerate = new List<EggType>();

        if (tier == SubscriptionTier.None)
        {
            // Ensure at least 1 Red and 1 Green
            eggsToGenerate.Add(EggType.Red);
            eggsToGenerate.Add(EggType.Green);

            // Fill remaining 3 eggs randomly with Red or Green
            for (int i = 2; i < eggsToSpawn; i++)
            {
                eggsToGenerate.Add(UnityEngine.Random.value > 0.5f ? EggType.Red : EggType.Green);
            }
        }
        else // Pro and Premium
        {
            // Guarantee 1 Red, 1 Green, 1 Purple, 1 Golden
            eggsToGenerate.Add(EggType.Red);
            eggsToGenerate.Add(EggType.Green);
            eggsToGenerate.Add(EggType.Purple);
            eggsToGenerate.Add(EggType.Golden);

            // 5th egg: pick any color (Red/Green/Purple/Golden)
            EggType fifthEgg = (EggType)UnityEngine.Random.Range(0, 4);
            eggsToGenerate.Add(fifthEgg);
        }

        // Shuffle eggs so order is random
        for (int i = 0; i < eggsToGenerate.Count; i++)
        {
            int swapIndex = UnityEngine.Random.Range(0, eggsToGenerate.Count);
            EggType temp = eggsToGenerate[i];
            eggsToGenerate[i] = eggsToGenerate[swapIndex];
            eggsToGenerate[swapIndex] = temp;
        }

        // Spawn eggs at random locations
        for (int i = 0; i < eggsToGenerate.Count; i++)
        {
            Vector2d geoPos = eggLocations[UnityEngine.Random.Range(0, eggLocations.Count)];
            EggType type = eggsToGenerate[i];
            GameObject prefab = GetPrefabByType(type);

            GameObject egg = Instantiate(prefab);
            egg.SetActive(true);
            EggBehavior behavior = egg.GetComponent<EggBehavior>();
            behavior.geoPosition = geoPos;
            behavior.map = map;
            behavior.player = player;
            behavior.spawnTime = DateTime.Now;
            behavior.eggType = type;
            behavior.isCollectable = ShouldBeCollectable(type, tier);

            Vector3 worldPos = map.GeoToWorldPosition(geoPos, true);
            egg.transform.position = worldPos;

            Debug.Log("?? Egg spawned: " + type + " at " + worldPos + " Collectable: " + behavior.isCollectable);
        }
    }

    EggType GetEggTypeForTier(SubscriptionTier tier)
    {
        if (tier == SubscriptionTier.None)
        {
            // Only Red or Green
            return UnityEngine.Random.value > 0.5f ? EggType.Red : EggType.Green;
        }

        // Pro and Premium: any of the 4 colors
        return (EggType)UnityEngine.Random.Range(0, 4); // 0=Red,1=Green,2=Purple,3=Golden
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

    bool ShouldBeCollectable(EggType type, SubscriptionTier tier)
    {
        if (tier == SubscriptionTier.None) return type == EggType.Red;      // only 1 collectable
        if (tier == SubscriptionTier.Pro) return type == EggType.Red;       // only 1 collectable
        if (tier == SubscriptionTier.Premium) return true;                  // all collectable
        return false;
    }
}