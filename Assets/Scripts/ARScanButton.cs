using UnityEngine;
using UnityEngine.SceneManagement;
using static EggSpawner;

public class ARScanButton : MonoBehaviour
{
    public EggSpawner mapEggSpawner;

    public void OnGoARButtonPressed()
    {


        Debug.Log("AR Button Pressed!"); // Mobile debug
        if (mapEggSpawner == null)
        {
            Debug.LogError("Map EggSpawner reference missing!");
            return;
        }

        EggManager.Instance.eggsToSpawn.Clear();

        foreach (EggBehavior egg in mapEggSpawner.spawnedEggs)
        {
            EggData data = new EggData()
            {
                eggType = egg.eggType,
                latitude = egg.geoPosition.x,
                longitude = egg.geoPosition.y
            };
            EggManager.Instance.eggsToSpawn.Add(data);
        }

        Debug.Log("Loading AR Scene");
        SceneManager.LoadScene("ARScene");
        /*if (EggManager.Instance == null)
        {
            Debug.LogError("? EggManager not found in scene!");
            return;
        }

        if (mapEggSpawner == null)
        {
            Debug.LogError("? Map EggSpawner not assigned!");
            return;
        }

        if (mapEggSpawner.spawnedEggs.Count == 0)
        {
            Debug.LogError("? No eggs spawned on map!");
            return;
        }

        EggManager.Instance.eggsToSpawn.Clear();

        foreach (EggBehavior egg in mapEggSpawner.spawnedEggs)
        {
            if (egg == null) continue;

            EggData data = new EggData
            {
                eggType = egg.eggType,
                latitude = egg.geoPosition.x,
                longitude = egg.geoPosition.y
            };

            EggManager.Instance.eggsToSpawn.Add(data);
        }

        Debug.Log("? Eggs sent to AR Scene: " + EggManager.Instance.eggsToSpawn.Count);
        SceneManager.LoadScene("ARScene");*/
    }
}
