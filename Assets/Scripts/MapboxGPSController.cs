/*using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using System.Collections;
using TMPro;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class MapboxGPSController : MonoBehaviour
{
    [Header("Mapbox")]
    public AbstractMap map;

    [Header("UI Player Marker (Centered)")]
    public RectTransform playerMarker;

    [Header("Zoom Settings")]
    public int minZoom = 14;
    public int maxZoom = 18;

    [Header("GPS Settings")]
    [Tooltip("~0.00005 ? 5 meters")]
    public float gpsMoveThreshold = 0.00005f;

    [Header("Pan Settings")]
    public float dragSensitivity = 0.000002f;

    [Header("Debug UI")]
    public TMP_Text debugText;

    // ================= INTERNAL =================
    Vector2d currentGPS;
    Vector2d mapCenterGPS;
    int currentZoom;
    bool gpsReady = false;

    Vector2 lastTouchPos;

    string pinchDebug = "None";
    float pinchDelta = 0f;

    // ================= UNITY =================
    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Start()
    {
        Input.location.Start(5f, 1f);
        Input.compass.enabled = true;

        StartCoroutine(InitGPS());
    }

    *//*IEnumerator InitGPS()
    {
        int wait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && wait > 0)
        {
            yield return new WaitForSeconds(1);
            wait--;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            Debug.LogError("? GPS FAILED");
            yield break;
        }

        currentGPS = new Vector2d(
            Input.location.lastData.latitude,
            Input.location.lastData.longitude
        );

        mapCenterGPS = currentGPS;
        currentZoom = Mathf.Clamp((int)map.Zoom, minZoom, maxZoom);

        map.Initialize(mapCenterGPS, currentZoom);
        gpsReady = true;

        if (playerMarker)
            playerMarker.anchoredPosition = Vector2.zero;
    }*//*


    IEnumerator InitGPS()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("GPS: User has not enabled GPS on device settings.");
            yield break;
        }

        Input.location.Start(5f, 1f);

        int wait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && wait > 0)
        {
            yield return new WaitForSeconds(1);
            wait--;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("GPS: Failed to connect to location service.");
            yield break;
        }

        if (wait <= 0)
        {
            Debug.LogError("GPS: Timed out waiting for signal.");
            yield break;
        }

        // ... rest of your initialization code
        gpsReady = true;
    }

    void Update()
    {
        // Run touch controls regardless of GPS
        HandleDragPan();
        HandlePinchZoom();
        UpdateDebug();

        // Only run GPS follow if the signal is actually ready
        if (gpsReady)
        {
            HandleGPSFollow();
        }
    }

    // ================= GPS FOLLOW =================
    void HandleGPSFollow()
    {
        Vector2d newGPS = new Vector2d(
            Input.location.lastData.latitude,
            Input.location.lastData.longitude
        );

        if (Vector2d.Distance(newGPS, currentGPS) > gpsMoveThreshold)
        {
            currentGPS = newGPS;
            mapCenterGPS = currentGPS;
            map.UpdateMap(mapCenterGPS, currentZoom);
        }
    }

    // ================= DRAG PAN =================
    void HandleDragPan()
    {
        var touches = Touch.activeTouches;

        if (touches.Count != 1)
            return;

        var t = touches[0];

        if (t.phase == UnityEngine.InputSystem.TouchPhase.Began)
        {
            lastTouchPos = t.screenPosition;
        }
        else if (t.phase == UnityEngine.InputSystem.TouchPhase.Moved)
        {
            Vector2 delta = t.screenPosition - lastTouchPos;
            lastTouchPos = t.screenPosition;

            mapCenterGPS.x -= delta.y * dragSensitivity;
            mapCenterGPS.y -= delta.x * dragSensitivity;

            map.UpdateMap(mapCenterGPS, currentZoom);
        }
    }

    // ================= PINCH ZOOM =================
    void HandlePinchZoom()
    {
        var touches = Touch.activeTouches;

        if (touches.Count != 2)
        {
            pinchDebug = "No Pinch";
            return;
        }

        var t0 = touches[0];
        var t1 = touches[1];

        float currDist = Vector2.Distance(t0.screenPosition, t1.screenPosition);
        float prevDist = Vector2.Distance(
            t0.screenPosition - t0.delta,
            t1.screenPosition - t1.delta
        );

        float delta = currDist - prevDist;
        pinchDelta = delta;

        if (Mathf.Abs(delta) < 5f)
        {
            pinchDebug = "Pinch Too Small";
            return;
        }

        int oldZoom = currentZoom;

        currentZoom += delta > 0 ? 1 : -1;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        pinchDebug = delta > 0 ? "ZOOM IN" : "ZOOM OUT";

        if (oldZoom != currentZoom)
        {
            Debug.Log($"? {pinchDebug} | {oldZoom} ? {currentZoom}");
            map.UpdateMap(mapCenterGPS, currentZoom);
        }
    }

    // ================= DEBUG =================
    void UpdateDebug()
    {
        if (!debugText) return;

        debugText.text =
            $"GPS: {Input.location.status}\n" +
            $"Lat: {mapCenterGPS.x:F6}\n" +
            $"Lon: {mapCenterGPS.y:F6}\n" +
            $"Zoom: {currentZoom}\n\n" +
            $"Pinch: {pinchDebug}\n" +
            $"?: {pinchDelta:F2}";
    }
}
*/


using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using System.Collections;
using TMPro;
using UnityEngine.Android; // Required for Android Permissions

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class MapboxGPSController : MonoBehaviour
{
    [Header("References")]
    public AbstractMap map;
    public RectTransform playerMarker;
    public TMP_Text debugText;

    [Header("Zoom Settings")]
    public float minZoom = 14f;
    public float maxZoom = 20f;
    public float zoomSensitivity = 0.01f;

    [Header("Pan Settings")]
    public float dragSensitivity = 0.000002f;

    // Internal State
    private Vector2d currentGPS;
    private Vector2d mapCenterGPS;
    private float currentZoom;
    private bool gpsReady = false;
    private bool isUserInteracting = false;
    private Vector2 lastTouchPos;
    private string statusMessage = "Initializing...";

    void OnEnable() { EnhancedTouchSupport.Enable(); }
    void OnDisable() { EnhancedTouchSupport.Disable(); }

    void Start()
    {
        // 1. Immediately sync variables with Mapbox Inspector settings
        // This ensures the map doesn't teleport to (0,0) if GPS is slow.
        currentZoom = (float)map.Zoom;
        mapCenterGPS = map.CenterLatitudeLongitude;

        // 2. Start Android permission and GPS sequence
        StartCoroutine(ContinuousGPSCheck());
    }

    void Update()
    {
        // Safety: Only run map controls if we are in 'Map' mode
        if (AppModeManager.Instance != null && AppModeManager.Instance.currentMode != AppMode.Map)
            return;

        // --- MANUAL TOUCH CONTROLS ---
        int touchCount = Touch.activeTouches.Count;
        if (touchCount > 0)
        {
            isUserInteracting = true;
            HandleDragPan();
            HandlePinchZoom();
        }
        else
        {
            isUserInteracting = false;
        }

        // --- GPS AUTO-FOLLOW ---
        // Only follows GPS if the user is NOT touching the screen
        if (gpsReady && !isUserInteracting)
        {
            UpdateGPSFollow();
        }

        UpdateDebugUI();
    }

    IEnumerator ContinuousGPSCheck()
    {
#if UNITY_ANDROID
        // Step 1: Request Permission (The popup you need to see on your phone)
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            statusMessage = "Awaiting Permission...";
            Permission.RequestUserPermission(Permission.FineLocation);
            yield return new WaitUntil(() => Permission.HasUserAuthorizedPermission(Permission.FineLocation));
        }
#endif

        // Step 2: Loop to keep trying the GPS if it fails
        while (true)
        {
            if (!Input.location.isEnabledByUser)
            {
                statusMessage = "GPS Disabled on Device";
            }
            else if (Input.location.status == LocationServiceStatus.Stopped || Input.location.status == LocationServiceStatus.Failed)
            {
                statusMessage = "Starting GPS...";
                Input.location.Start(5f, 1f);
            }
            else if (Input.location.status == LocationServiceStatus.Running)
            {
                gpsReady = true;
                statusMessage = "GPS Active";

                // If this is the first signal, snap the map to the player
                if (currentGPS.x == 0)
                {
                    currentGPS = new Vector2d(Input.location.lastData.latitude, Input.location.lastData.longitude);
                    mapCenterGPS = currentGPS;
                    map.UpdateMap(mapCenterGPS, currentZoom);
                }
            }

            yield return new WaitForSeconds(3); // Re-check every 3 seconds
        }
    }

    void UpdateGPSFollow()
    {
        Vector2d latest = new Vector2d(Input.location.lastData.latitude, Input.location.lastData.longitude);

        // Only move the map if player moved more than ~5 meters
        if (Vector2d.Distance(latest, currentGPS) > 0.00005f)
        {
            currentGPS = latest;
            mapCenterGPS = latest;
            map.UpdateMap(mapCenterGPS, currentZoom);
        }

        // Keep marker centered
        if (playerMarker) playerMarker.anchoredPosition = Vector2.zero;
    }

    void HandleDragPan()
    {
        var touches = Touch.activeTouches;
        if (touches.Count != 1) return;

        var t = touches[0];
        if (t.phase == UnityEngine.InputSystem.TouchPhase.Began)
            lastTouchPos = t.screenPosition;
        else if (t.phase == UnityEngine.InputSystem.TouchPhase.Moved)
        {
            Vector2 delta = t.screenPosition - lastTouchPos;
            lastTouchPos = t.screenPosition;

            // Shift the center based on finger drag
            mapCenterGPS.x -= delta.y * dragSensitivity;
            mapCenterGPS.y -= delta.x * dragSensitivity;
            map.UpdateMap(mapCenterGPS, currentZoom);
        }
    }

    void HandlePinchZoom()
    {
        var touches = Touch.activeTouches;
        if (touches.Count != 2) return;

        float currDist = Vector2.Distance(touches[0].screenPosition, touches[1].screenPosition);
        float prevDist = Vector2.Distance(touches[0].screenPosition - touches[0].delta, touches[1].screenPosition - touches[1].delta);
        float delta = currDist - prevDist;

        if (Mathf.Abs(delta) < 1.5f) return;

        currentZoom = Mathf.Clamp(currentZoom + (delta * zoomSensitivity), minZoom, maxZoom);
        map.UpdateMap(mapCenterGPS, currentZoom);
    }

    void UpdateDebugUI()
    {
        if (debugText == null) return;
        debugText.text = $"GPS: {statusMessage}\n" +
                         $"Lat: {mapCenterGPS.x:F5}\n" +
                         $"Lon: {mapCenterGPS.y:F5}\n" +
                         $"Zoom: {currentZoom:F1}";
    }

    // Link this to a UI Button to snap the map back to your GPS position
    public void Recenter()
    {
        if (gpsReady)
        {
            currentGPS = new Vector2d(Input.location.lastData.latitude, Input.location.lastData.longitude);
            mapCenterGPS = currentGPS;
            map.UpdateMap(mapCenterGPS, currentZoom);
        }
    }
}