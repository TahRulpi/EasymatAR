/*using UnityEngine;
using UnityEngine.UI;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using System.Collections;
using TMPro;

public class MapboxGPSController : MonoBehaviour
{
    [Header("Mapbox References")]
    public AbstractMap map;
    public Camera mapCamera;

    [Header("Player UI Marker")]
    public RectTransform playerMarker;
    public Canvas canvas;

    [Header("Zoom Settings")]
    public float zoomSpeed = 0.3f;
    public float minZoom = 14f;
    public float maxZoom = 18f;

    [Header("Rotation Settings")]
    public float rotationSmoothing = 10f;

    [Header("Compass UI")]
    public RectTransform compassNeedle;

    [Header("Debug")]
    public TMP_Text debugText;

    // INTERNAL
    private Vector2d currentGPS;
    private Vector2d lastUpdatedGPS;
    private float currentZoomLevel;
    private bool gpsReady = false;

    void Start()
    {
        if (mapCamera == null)
            mapCamera = Camera.main;

        Input.location.Start(5f, 1f);
        Input.compass.enabled = true;

        StartCoroutine(StartLocationService());
    }

    IEnumerator StartLocationService()
    {
        int wait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && wait > 0)
        {
            yield return new WaitForSeconds(1);
            wait--;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            Debug.LogError("GPS not running");
            yield break;
        }

        currentGPS = new Vector2d(
            Input.location.lastData.latitude,
            Input.location.lastData.longitude
        );

        currentZoomLevel = map.Zoom;
        lastUpdatedGPS = currentGPS;

        map.Initialize(currentGPS, (int)currentZoomLevel);
        gpsReady = true;
    }

    void Update()
    {
        if (!gpsReady) return;

        HandleGPSMovement();
        HandlePinchZoom();
        HandleRotation();
        UpdatePlayerMarker();
        UpdateDebugText();
    }

    // ========================= GPS =========================
    *//*void HandleGPSMovement()
    {
        if (Input.location.status != LocationServiceStatus.Running) return;

        currentGPS = new Vector2d(
            Input.location.lastData.latitude,
            Input.location.lastData.longitude
        );

        // ~1–2 meters movement threshold
        if (Vector2d.Distance(currentGPS, lastUpdatedGPS) > 0.00002)
        {
            map.UpdateMap(currentGPS, currentZoomLevel);
            lastUpdatedGPS = currentGPS;
        }
    }*//*

    void HandleGPSMovement()
    {
        if (Input.location.status != LocationServiceStatus.Running) return;

        // Get fresh data
        Vector2d newGPS = new Vector2d(
            Input.location.lastData.latitude,
            Input.location.lastData.longitude
        );

        // Using a smaller threshold or checking if coordinates changed at all
        if (newGPS.x != currentGPS.x || newGPS.y != currentGPS.y)
        {
            currentGPS = newGPS;

            // Check distance in meters (approximate) to avoid jitter
            // 0.00001 is roughly 1.1 meters
            if (Vector2d.Distance(currentGPS, lastUpdatedGPS) > 0.00001)
            {
                // This moves the MAP to center on you
                map.UpdateMap(currentGPS, currentZoomLevel);
                lastUpdatedGPS = currentGPS;
            }
        }
    }

    // ========================= PINCH ZOOM =========================
    void HandlePinchZoom()
    {
        if (Input.touchCount != 2) return;

        Touch t0 = Input.GetTouch(0);
        Touch t1 = Input.GetTouch(1);

        if (t0.phase != TouchPhase.Moved && t1.phase != TouchPhase.Moved) return;

        float currDist = Vector2.Distance(t0.position, t1.position);
        float prevDist = Vector2.Distance(
            t0.position - t0.deltaPosition,
            t1.position - t1.deltaPosition
        );

        float delta = currDist - prevDist;

        // Deadzone
        if (Mathf.Abs(delta) < 0.5f) return;

        float pinchAmount = (delta / Screen.width) * zoomSpeed * 50f;

        float newZoom = Mathf.Clamp(
            currentZoomLevel + pinchAmount,
            minZoom,
            maxZoom
        );

        if (Mathf.Abs(newZoom - currentZoomLevel) > 0.001f)
        {
            currentZoomLevel = newZoom;
            map.UpdateMap(currentGPS, currentZoomLevel);
        }
    }

    // ========================= ROTATION =========================
    void HandleRotation()
    {
        if (!Input.compass.enabled) return;

        float heading = Input.compass.trueHeading;

        if (playerMarker != null)
        {
            Quaternion targetRot = Quaternion.Euler(0, 0, -heading);
            playerMarker.rotation = Quaternion.Slerp(
                playerMarker.rotation,
                targetRot,
                Time.deltaTime * rotationSmoothing
            );
        }

        if (compassNeedle != null)
        {
            compassNeedle.rotation = Quaternion.Euler(0, 0, heading);
        }
    }

    // ========================= PLAYER ICON =========================
    void UpdatePlayerMarker()
    {
        if (playerMarker == null || canvas == null) return;

        // IMPORTANT: If map.UpdateMap is centering the map on your GPS, 
        // the 'WorldPosition' of your GPS will ALWAYS be (0,0,0).
        // To see the icon move, we force the icon to the center of the screen 
        // OR use the relative position.

        // If you want the map to FOLLOW the player:
        // Just set the icon to the center of the screen once and let the map move.

        // If you want to CALCULATE it anyway:
        Vector3 worldPos = map.GeoToWorldPosition(currentGPS, true);
        Vector3 screenPos = mapCamera.WorldToScreenPoint(worldPos);

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            canvas.worldCamera,
            out Vector2 localPoint))
        {
            playerMarker.anchoredPosition = localPoint;
        }
    }

    // ========================= DEBUG =========================
    void UpdateDebugText()
    {
        if (debugText == null) return;

        debugText.text =
            "GPS: " + Input.location.status + "\n" +
            "Lat: " + currentGPS.x.ToString("F5") + "\n" +
            "Lon: " + currentGPS.y.ToString("F5") + "\n" +
            "Heading: " + Input.compass.trueHeading.ToString("F0") + "°\n" +
            "Zoom: " + currentZoomLevel.ToString("F2");
    }
}
*/


using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using System.Collections;
using TMPro;

public class MapboxGPSController : MonoBehaviour
{
    [Header("Mapbox")]
    public AbstractMap map;

    [Header("Player Marker (centered UI)")]
    public RectTransform playerMarker;

    [Header("Zoom Settings")]
    public int minZoom = 14;
    public int maxZoom = 18;

    [Header("GPS Settings")]
    [Tooltip("~0.00005 ? 5 meters")]
    public float gpsMoveThreshold = 0.00005f;

    [Header("Debug")]
    public TMP_Text debugText;

    // INTERNAL
    private Vector2d currentGPS;
    private Vector2d lastMapCenterGPS;
    private int currentZoom;
    private bool gpsReady = false;

    void Start()
    {
        Input.location.Start(5f, 1f);
        Input.compass.enabled = true;

        StartCoroutine(InitGPS());
    }

    IEnumerator InitGPS()
    {
        int wait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && wait > 0)
        {
            yield return new WaitForSeconds(1);
            wait--;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            Debug.LogError("GPS NOT RUNNING");
            yield break;
        }

        currentGPS = new Vector2d(
            Input.location.lastData.latitude,
            Input.location.lastData.longitude
        );

        lastMapCenterGPS = currentGPS;

        // ?? FORCE INT (important)
        currentZoom = (int)map.Zoom;

        map.Initialize(currentGPS, currentZoom);
        gpsReady = true;

        if (playerMarker != null)
            playerMarker.anchoredPosition = Vector2.zero;
    }

    void Update()
    {
        if (!gpsReady) return;

        HandleGPSMovement();
        HandlePinchZoom();
        UpdateDebug();
    }

    // ================= GPS =================
    void HandleGPSMovement()
    {
        if (Input.location.status != LocationServiceStatus.Running) return;

        Vector2d newGPS = new Vector2d(
            Input.location.lastData.latitude,
            Input.location.lastData.longitude
        );

        if (Vector2d.Distance(newGPS, lastMapCenterGPS) > gpsMoveThreshold)
        {
            currentGPS = newGPS;
            lastMapCenterGPS = newGPS;

            // ?? FORCE INT
            map.UpdateMap(currentGPS, (int)currentZoom);
        }
    }

    // ================= PINCH ZOOM =================
    void HandlePinchZoom()
    {
        if (Input.touchCount != 2) return;

        Touch t0 = Input.GetTouch(0);
        Touch t1 = Input.GetTouch(1);

        if (t0.phase != TouchPhase.Moved || t1.phase != TouchPhase.Moved) return;

        float currDist = Vector2.Distance(t0.position, t1.position);
        float prevDist = Vector2.Distance(
            t0.position - t0.deltaPosition,
            t1.position - t1.deltaPosition
        );

        float delta = currDist - prevDist;

        if (Mathf.Abs(delta) < 2f) return;

        int step = delta > 0 ? 1 : -1;

        // ?? Clamp using FLOAT, then cast to INT
        currentZoom = (int)Mathf.Clamp(
            currentZoom + step,
            minZoom,
            maxZoom
        );

        map.UpdateMap(currentGPS, (int)currentZoom);
    }

    // ================= DEBUG =================
    void UpdateDebug()
    {
        if (debugText == null) return;

        debugText.text =
            "GPS: " + Input.location.status + "\n" +
            "Lat: " + currentGPS.x.ToString("F6") + "\n" +
            "Lon: " + currentGPS.y.ToString("F6") + "\n" +
            "Zoom: " + currentZoom;
    }
}
