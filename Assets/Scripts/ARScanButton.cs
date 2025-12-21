using UnityEngine;
using UnityEngine.SceneManagement;

public class ARScanButton : MonoBehaviour
{
    public void ScanNearestEgg()
    {
        EggMarkerUI nearest = null;
        float minDist = float.MaxValue;

        foreach (EggMarkerUI egg in FindObjectsOfType<EggMarkerUI>())
        {
            if (!egg.isScannable)
                continue;

            float d = egg.transform.localPosition.magnitude;
            if (d < minDist)
            {
                minDist = d;
                nearest = egg;
            }
        }

        if (nearest == null)
        {
            Debug.Log("? No egg close enough to scan");
            return;
        }

        // Save egg info for AR scene
        PlayerPrefs.SetString("SCAN_EGG_LAT", nearest.gpsPosition.x.ToString());
        PlayerPrefs.SetString("SCAN_EGG_LON", nearest.gpsPosition.y.ToString());
        PlayerPrefs.SetInt("SCAN_EGG_TYPE", (int)nearest.eggType);

        Debug.Log("? Loading AR Scene");
        SceneManager.LoadScene("ARScene");
    }
}
