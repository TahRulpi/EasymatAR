/*using UnityEngine;

public class AREggSpawner : MonoBehaviour
{
    public GameObject redEggPrefab;
    public GameObject greenEggPrefab;
    public GameObject purpleEggPrefab;
    public GameObject goldenEggPrefab;

    void Start()
    {
        float lat = float.Parse(PlayerPrefs.GetString("SCAN_EGG_LAT"));
        float lon = float.Parse(PlayerPrefs.GetString("SCAN_EGG_LON"));
        EggType type = (EggType)PlayerPrefs.GetInt("SCAN_EGG_TYPE");

        GameObject prefabToUse = GetPrefabByType(type);

        Vector3 spawnPos = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;

        GameObject egg = Instantiate(prefabToUse, spawnPos, Quaternion.identity);
        egg.GetComponent<EggBehavior>().eggType = type;

        // Decide collectable based on subscription tier
        SubscriptionTier tier = (SubscriptionTier)PlayerPrefs.GetInt("USER_TIER", 0);
        egg.GetComponent<EggBehavior>().isCollectable = ShouldBeCollectable(type, tier);
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
        if (tier == SubscriptionTier.None) return type == EggType.Red; // only 1 collectable
        if (tier == SubscriptionTier.Pro) return type == EggType.Green; // example: only 1 collectable
        if (tier == SubscriptionTier.Premium) return true; // all collectable
        return false;
    }
}
*/


using UnityEngine;

public class AREggSpawner : MonoBehaviour
{
    public GameObject redEggPrefab;
    public GameObject greenEggPrefab;
    public GameObject purpleEggPrefab;
    public GameObject goldenEggPrefab;

    void Start()
    {
        EggType type = (EggType)PlayerPrefs.GetInt("SCAN_EGG_TYPE", 0);

        GameObject prefab = GetPrefabByType(type);

        Camera cam = Camera.main;

        // Spawn 1.5 meters in front of camera
        Vector3 spawnPos = cam.transform.position + cam.transform.forward * 1.5f;

        GameObject egg = Instantiate(prefab, spawnPos, Quaternion.identity);

        egg.tag = "Egg";

        EggBehavior behavior = egg.GetComponent<EggBehavior>();
        behavior.eggType = type;

        Debug.Log("?? Egg spawned in AR");
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
