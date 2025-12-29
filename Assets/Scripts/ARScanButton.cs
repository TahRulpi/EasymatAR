using UnityEngine;
using UnityEngine.SceneManagement;
using static EggSpawner;

public class ARScanButton : MonoBehaviour
{
    public EggSpawner mapEggSpawner;

    /*public void OnGoARButtonPressed()
    {
        Debug.Log("AR Button Pressed!");

        if (EggManager.Instance == null)
        {
            Debug.LogError("EggManager.Instance is NULL!");
            return;
        }

        if (mapEggSpawner == null)
        {
            Debug.LogError("Map EggSpawner reference missing!");
            return;
        }

        if (mapEggSpawner.spawnedEggs == null || mapEggSpawner.spawnedEggs.Count == 0)
        {
            Debug.LogError("No eggs found to send to AR!");
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

        Debug.Log("Eggs sent to AR: " + EggManager.Instance.eggsToSpawn.Count);
        SceneManager.LoadScene("ARScene");
    }*/

    public void OnGoARButtonPressed()
    {
        Debug.Log("AR Button Pressed!");

        EggManager.Instance.playerLatitude = Input.location.lastData.latitude;
        EggManager.Instance.playerLongitude = Input.location.lastData.longitude;

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

        SceneManager.LoadScene("ARScene");
    }

}
