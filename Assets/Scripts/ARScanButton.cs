using UnityEngine;
using UnityEngine.SceneManagement;

public class ARScanButton : MonoBehaviour
{
    public EggSpawner mapEggSpawner;

    public void OnGoARButtonPressed()
    {

        AppModeManager.Instance.currentMode = AppMode.AR;
        Debug.Log("AR Button Pressed!");

        if (EggManager.Instance == null)
        {
            Debug.LogError("EggManager.Instance is NULL!");
            return;
        }

        // Set player GPS
        EggManager.Instance.playerLatitude = Input.location.lastData.latitude;
        EggManager.Instance.playerLongitude = Input.location.lastData.longitude;

        // Clear previous eggs
        EggManager.Instance.eggsToSpawn.Clear();

        bool hasEggs = false;

        // Add eggs from map spawner
        if (mapEggSpawner != null && mapEggSpawner.spawnedEggs != null)
        {
            foreach (var egg in mapEggSpawner.spawnedEggs)
            {
                if (egg == null) continue;

                EggData data = new EggData
                {
                    eggType = egg.eggType,
                    latitude = egg.geoPosition.x,
                    longitude = egg.geoPosition.y
                };

                EggManager.Instance.eggsToSpawn.Add(data);
                hasEggs = true;
            }
        }

        // If no eggs, add default red egg at player location
        if (!hasEggs)
        {
            EggData defaultEgg = new EggData
            {
                eggType = EggType.Red,
                latitude = EggManager.Instance.playerLatitude,
                longitude = EggManager.Instance.playerLongitude
            };
            EggManager.Instance.eggsToSpawn.Add(defaultEgg);
            Debug.Log("No eggs found, added default red egg at player location.");
        }

        Debug.Log("Eggs sent to AR: " + EggManager.Instance.eggsToSpawn.Count);

        // Load AR scene
        SceneManager.LoadScene("ARScene");
    }
}
