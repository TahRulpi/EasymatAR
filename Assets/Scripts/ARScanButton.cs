using UnityEngine;
using UnityEngine.SceneManagement;

public class ARScanButton : MonoBehaviour
{
    public GameObject ARScanPanel;  // UI panel for AR scanning
    public GameObject MapPanel;     // UI panel for map view
    public GameObject ARCamera;     // AR Camera object

    public void ScanNearestEgg()
    {
        EggMarkerUI nearest = null;
        float minDist = float.MaxValue;





        foreach (EggMarkerUI egg in FindObjectsOfType<EggMarkerUI>())
        {
            if (!egg.isScannable) continue;

            float d = Vector3.Distance(Vector3.zero, egg.transform.localPosition);
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

        PlayerPrefs.SetString("SCAN_EGG_LAT", nearest.gpsPosition.x.ToString());
        PlayerPrefs.SetString("SCAN_EGG_LON", nearest.gpsPosition.y.ToString());
        PlayerPrefs.SetInt("SCAN_EGG_TYPE", (int)nearest.eggType);

        SceneManager.LoadScene("ARScene");
    }
    public void OnClickARScan()
    {
        // Switch cameras/UI
        MapPanel.SetActive(false);
        ARScanPanel.SetActive(true);
        ARCamera.SetActive(true);

        // Optional: Start AR scanning logic
        Debug.Log("AR Scan Started!");
    }
}
