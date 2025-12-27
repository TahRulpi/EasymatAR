using System.Collections;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;

public class PlayerMarker : MonoBehaviour
{
    [Header("Mapbox")]
    public AbstractMap map;

    [Header("Player Icon")]
    public GameObject iconPrefab;   // Quad / Sprite / 3D icon
    private GameObject iconInstance;

    private bool gpsReady = false;

    IEnumerator Start()
    {
        if (map == null)
        {
            Debug.LogError("? AbstractMap not assigned!");
            yield break;
        }

        // 1?? Check GPS permission
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("? GPS disabled by user");
            yield break;
        }

        // 2?? Start GPS
        Input.location.Start(1f, 1f);

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            Debug.LogError("? GPS failed to start");
            yield break;
        }

        Debug.Log("? GPS STARTED");
        gpsReady = true;

        // 3?? Spawn player icon
        if (iconPrefab != null)
        {
            iconInstance = Instantiate(iconPrefab, transform);
            iconInstance.transform.localPosition = Vector3.zero;
        }
    }

    void Update()
    {
        if (!gpsReady || map == null)
            return;

        // 4?? Read real GPS
        Vector2d gpsPos = new Vector2d(
            Input.location.lastData.latitude,
            Input.location.lastData.longitude
        );

        // 5?? Move the map with the player (CRITICAL)
        map.UpdateMap(gpsPos);

        // 6?? Place player in map world
        Vector3 worldPos = map.GeoToWorldPosition(gpsPos, true);
        worldPos.y = 1f;
        transform.position = worldPos;

        // 7?? Face camera (optional but nice)
        if (iconInstance != null && Camera.main != null)
        {
            iconInstance.transform.LookAt(Camera.main.transform);
            iconInstance.transform.rotation = Quaternion.Euler(
                0,
                iconInstance.transform.rotation.eulerAngles.y,
                0
            );
        }


        Debug.Log(
    $"?? GPS STATUS: {Input.location.status} | " +
    $"Lat: {Input.location.lastData.latitude}, " +
    $"Lon: {Input.location.lastData.longitude}, " +
    $"Acc: {Input.location.lastData.horizontalAccuracy}"
);

    }


}
