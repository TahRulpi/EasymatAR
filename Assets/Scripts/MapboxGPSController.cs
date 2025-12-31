using UnityEngine;
using UnityEngine.UI;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using System.Collections;

public class MapboxGPSController : MonoBehaviour
{
    [Header("Mapbox")]
    public AbstractMap map;

    [Header("Player Marker (UI Image)")]
    public RectTransform playerMarker; // Fixed at center

    [Header("Zoom Settings")]
    public float zoomSpeed = 0.2f;
    public int minZoom = 14;
    public int maxZoom = 18;

    [Header("Rotation Settings")]
    public bool rotateMapWithCompass = true; // true = map rotates under player
    public bool rotatePlayerMarker = false;  // true = player icon rotates with heading

    private Vector2d currentGPS;
    private bool gpsReady = false;
    private int currentZoom;

    void Start()
    {
        if (!Application.isMobilePlatform)
        {
            Debug.LogError("? This script works ONLY on real phone");
            return;
        }

        if (playerMarker != null)
            playerMarker.GetComponent<Image>().raycastTarget = false;

        StartCoroutine(StartLocationService());
    }

    IEnumerator StartLocationService()
    {

        Input.compass.enabled = true;
        Input.gyro.enabled = true; // optional, helps smooth rotation

        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("? GPS not enabled by user");
            yield break;
        }

        Input.location.Start(1f, 1f);
        Input.compass.enabled = true;
        Input.gyro.enabled = true;

        int wait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && wait > 0)
        {
            yield return new WaitForSeconds(1);
            wait--;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            Debug.LogError("? GPS failed to start");
            yield break;
        }

        currentGPS = new Vector2d(
            Input.location.lastData.latitude,
            Input.location.lastData.longitude
        );

        currentZoom = Mathf.RoundToInt(map.Zoom);
        gpsReady = true;

        map.Initialize(currentGPS, currentZoom);
        Debug.Log($"?? GPS READY: {currentGPS.x}, {currentGPS.y}");
    }

    void Update()
    {
        if (!gpsReady) return;

        UpdateGPSPosition();
        HandlePinchZoom();
        HandleMapRotation();
    }

    void UpdateGPSPosition()
    {
        Vector2d newGPS = new Vector2d(
            Input.location.lastData.latitude,
            Input.location.lastData.longitude
        );

        if (newGPS != currentGPS)
        {
            currentGPS = newGPS;
            map.UpdateMap(currentGPS, currentZoom);
            Debug.Log($"?? Map moved to GPS: {currentGPS.x}, {currentGPS.y}");
        }
    }

    void HandlePinchZoom()
    {
        if (Input.touchCount != 2) return;

        Touch t0 = Input.GetTouch(0);
        Touch t1 = Input.GetTouch(1);

        Vector2 t0Prev = t0.position - t0.deltaPosition;
        Vector2 t1Prev = t1.position - t1.deltaPosition;

        float prevDist = (t0Prev - t1Prev).magnitude;
        float currDist = (t0.position - t1.position).magnitude;

        float delta = currDist - prevDist;

        int newZoom = currentZoom + (delta > 0 ? 1 : -1);
        newZoom = Mathf.Clamp(newZoom, minZoom, maxZoom);

        if (newZoom != currentZoom)
        {
            currentZoom = newZoom;
            map.UpdateMap(currentGPS, currentZoom);
            Debug.Log("?? Zoom updated: " + currentZoom);
        }
    }

    void HandleMapRotation()
    {

        float heading = Input.compass.trueHeading;

        // Smooth rotation of the map root
        map.Root.rotation = Quaternion.Lerp(
            map.Root.rotation,
            Quaternion.Euler(0, -heading, 0), // negative because we rotate the map under player
            Time.deltaTime * 5f // smoothing factor
        );

        if (!rotateMapWithCompass || map == null || !Input.compass.enabled) return;

       // float heading = Input.compass.trueHeading;

        // Smooth rotation
        map.Root.rotation = Quaternion.Lerp(
            map.Root.rotation,
            Quaternion.Euler(0, -heading, 0),
            Time.deltaTime * 5f
        );

        // Optional: rotate player icon
        if (rotatePlayerMarker && playerMarker != null)
        {
            playerMarker.localRotation = Quaternion.Euler(0, 0, heading);
        }

        Debug.Log($"?? Map rotated to heading: {heading}");
    }
}
