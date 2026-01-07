using UnityEngine;
using UnityEngine.Android;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.Utilities;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using TMPro;
using System.Collections;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class MapboxGPSController : MonoBehaviour
{
    [Header("Mapbox Reference")]
    public AbstractMap map;

    [Header("UI References")]
    public RectTransform playerMarker;
    public RectTransform accuracyCircle;
    public RectTransform compassImage;
    public TMP_Text debugText;

    [Header("Zoom Settings")]
    public float minZoom = 14f;
    public float maxZoom = 20f;
    public float zoomSensitivity = 0.01f;


    [Header("Debug Label")]
    public TMP_Text gpsLabel; // assign your TMP_Text from Canvas in Inspector

    [Header("Stabilization & Physics")]
    public float gpsSmoothSpeed = 5f;
    [Range(0.01f, 0.2f)] public float dragSensitivityBase = 0.05f;
    public float inertiaDecay = 0.85f;
    public float maxAllowedAccuracy = 30f;

    [Header("Mode")]
    public bool followPlayer = true;

    // Private variables
    private Vector2d gpsLatLon;
    private Vector2d smoothGps;
    private Vector2d mapCenter;
    private float currentZoom;
    private float currentHeading;
    private Vector2 dragVelocity;
    private Vector2 lastTouchPos;
    private bool isDragging = false;
    private bool gpsReady = false;

    void OnEnable() => EnhancedTouchSupport.Enable();
    void OnDisable() => EnhancedTouchSupport.Disable();

    void Start()
    {
        currentZoom = (float)map.Zoom;

        // Default map center (while GPS warms up)
        mapCenter = new Vector2d(40.7128, -74.0060); // NYC
        map.Initialize(mapCenter, (int)currentZoom);

        StartCoroutine(GPSRoutine());
    }

    void Update()
    {
        HandleTouch();

        // Apply inertia after finger is lifted
        if (!isDragging && !followPlayer && dragVelocity.magnitude > 0.01f)
            ApplyInertia();

        if (!gpsReady) return;

        UpdateGPS();
        UpdateVisuals();
        UpdateCompass();
        UpdateDebug();

        Debug.Log($"[COMPASS] TrueHeading: {Input.compass.trueHeading:F1} | MagneticHeading: {Input.compass.magneticHeading:F1}");

    }

    // ================= GPS INITIALIZATION =================
    IEnumerator GPSRoutine()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            yield return new WaitUntil(() => Permission.HasUserAuthorizedPermission(Permission.FineLocation));
        }
#endif

        if (!Input.location.isEnabledByUser)
        {
            if (debugText) debugText.text = "GPS Error: Location Disabled";
            yield break;
        }

        Input.location.Start(1f, 1f); 
        Input.compass.enabled = true;

        int compassWait = 10;
        while (Input.compass.headingAccuracy <= 0 && compassWait > 0)
        {
            if (debugText) debugText.text = $"Calibrating Compass... {compassWait}";
            yield return new WaitForSeconds(1);
            compassWait--;
        }


        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            if (debugText) debugText.text = $"Searching GPS... {maxWait}";
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (Input.location.status == LocationServiceStatus.Failed || maxWait <= 0)
        {
            if (debugText) debugText.text = "GPS Failed: No Signal";
            yield break;
        }

        // GPS ready
        gpsLatLon = new Vector2d(Input.location.lastData.latitude, Input.location.lastData.longitude);
        smoothGps = gpsLatLon;
        mapCenter = gpsLatLon;
        map.UpdateMap(mapCenter, currentZoom);
        gpsReady = true;
    }

    

    void UpdateGPS()
    {
        // 1. Get latest raw GPS
        Vector2d latestRaw = new Vector2d(Input.location.lastData.latitude, Input.location.lastData.longitude);
        float horizontalAccuracy = Input.location.lastData.horizontalAccuracy;

        // 2. Ignore poor GPS signals
        if (horizontalAccuracy > maxAllowedAccuracy) return;

        // 3. Calculate distance moved from last stable GPS position (in meters)
        double distance = Vector2d.Distance(latestRaw, gpsLatLon) * 111000.0;

        // 4. Update gpsLatLon only if distance > threshold or horizontal accuracy improved
        float minMovementThreshold = 0.3f; // can be very small, but we’ll filter jitter
        if (distance > minMovementThreshold || horizontalAccuracy < Input.location.lastData.horizontalAccuracy)
        {
            gpsLatLon = latestRaw;
        }

        // 5. Smoothly interpolate position for natural movement
        float smoothFactor = gpsSmoothSpeed * Time.deltaTime;

        // Extra smoothing: smaller movement ? slower interpolation, larger ? faster
        double lerpFactor = Mathf.Clamp01(smoothFactor * (float)(distance / 10.0));
        smoothGps = Vector2d.Lerp(smoothGps, gpsLatLon, (float)lerpFactor);

        // 6. Update map center only if following player
        if (followPlayer)
        {
            mapCenter = smoothGps;
            map.UpdateMap(mapCenter, currentZoom);
        }

        // 7. Update debug info and GPS label
        Debug.Log($"[GPS DEBUG] SmoothGPS: {smoothGps.x:F6}, {smoothGps.y:F6} | Accuracy: {horizontalAccuracy:F1}m | Moved: {distance:F2}m");
        UpdateGPSLabel();
    }




    // ================= TOUCH INPUT =================
    void HandleTouch()
    {
        var touches = Touch.activeTouches;

        if (touches.Count == 1)
            HandleDrag(touches[0]);
        else if (touches.Count == 2)
            HandleZoom(touches);
        else
            isDragging = false;
    }

    void HandleDrag(Touch t)
    {
        if (t.phase == UnityEngine.InputSystem.TouchPhase.Began)
        {
            isDragging = true;
            followPlayer = false;
            dragVelocity = Vector2.zero;
            lastTouchPos = t.screenPosition;
        }

        if (t.phase == UnityEngine.InputSystem.TouchPhase.Moved)
        {
            Vector2 delta = t.screenPosition - lastTouchPos;
            lastTouchPos = t.screenPosition;

            float zoomFactor = Mathf.Pow(2, currentZoom);
            double latStep = (delta.y * dragSensitivityBase) / zoomFactor;
            double lonStep = (delta.x * dragSensitivityBase) / zoomFactor;

            mapCenter.x -= latStep;
            mapCenter.y -= lonStep;

            dragVelocity = delta;
            map.UpdateMap(mapCenter, currentZoom);
        }

        if (t.phase == UnityEngine.InputSystem.TouchPhase.Ended)
            isDragging = false;
    }

    void ApplyInertia()
    {
        dragVelocity *= inertiaDecay;
        float zoomFactor = Mathf.Pow(2, currentZoom);
        mapCenter.x -= (dragVelocity.y * dragSensitivityBase) / zoomFactor;
        mapCenter.y -= (dragVelocity.x * dragSensitivityBase) / zoomFactor;
        map.UpdateMap(mapCenter, currentZoom);
    }

    void HandleZoom(ReadOnlyArray<Touch> touches)
    {
        followPlayer = false;
        float currDist = Vector2.Distance(touches[0].screenPosition, touches[1].screenPosition);
        float prevDist = Vector2.Distance(touches[0].screenPosition - touches[0].delta, touches[1].screenPosition - touches[1].delta);
        float delta = currDist - prevDist;
        currentZoom = Mathf.Clamp(currentZoom + (delta * zoomSensitivity), minZoom, maxZoom);
        map.UpdateMap(mapCenter, currentZoom);
    }

    

    void UpdateVisuals()
    {
        if (playerMarker == null || map == null || Camera.main == null) return;

        // 1. Get world position from GPS
        Vector3 worldPos = map.GeoToWorldPosition(smoothGps, false);

        // 2. Convert to screen position
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        // 3. Adjust for canvas type
        Canvas canvas = playerMarker.GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                // Directly use screenPos
                playerMarker.position = screenPos;
            }
            else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                // Convert to Canvas space
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    screenPos,
                    canvas.worldCamera,
                    out Vector2 localPoint
                );
                playerMarker.localPosition = localPoint;
            }
            else if (canvas.renderMode == RenderMode.WorldSpace)
            {
                playerMarker.position = worldPos;
            }
        }

        // ================= ACCURACY CIRCLE =================
        if (accuracyCircle != null)
        {
            accuracyCircle.position = playerMarker.position;

            float metersPerPixel = (float)(40075016.686 * Mathf.Cos((float)smoothGps.x * Mathf.Deg2Rad) / Mathf.Pow(2, currentZoom + 8));
            float pixelSize = Input.location.lastData.horizontalAccuracy / metersPerPixel;

            accuracyCircle.sizeDelta = Vector2.Lerp(
                accuracyCircle.sizeDelta,
                new Vector2(pixelSize * 2, pixelSize * 2),
                Time.deltaTime * 5f
            );

            var img = accuracyCircle.GetComponent<UnityEngine.UI.Image>();
            if (img != null)
            {
                img.color = Input.location.lastData.horizontalAccuracy > 20 ?
                            new Color(1, 0, 0, 0.2f) : new Color(0, 0.5f, 1, 0.2f);
            }
        }

        // ================= GPS DEBUG LABEL =================
        UpdateGPSLabel();
    }


    void UpdateCompass()
    {
        if (!gpsReady) return;

        // 1. Get the heading (fallback to magnetic if trueHeading is 0)
        float rawHeading = Input.compass.trueHeading;
        if (rawHeading == 0 && Input.compass.magneticHeading != 0)
            rawHeading = Input.compass.magneticHeading;

        // 2. Filter out jitters with Lerp
        currentHeading = Mathf.LerpAngle(currentHeading, rawHeading, Time.deltaTime * 5f);

        // 3. Apply to Player Marker (Points the arrow toward the physical North)
        if (playerMarker != null)
        {
            // For most Mapbox setups, North is Up (0 deg). 
            // We rotate the marker by the heading value.
            playerMarker.localRotation = Quaternion.Euler(0, 0, -currentHeading);
        }

        // 4. Apply to UI Compass Image
        if (compassImage != null)
        {
            // If the image is a compass dial (N, S, E, W), rotate it 
            // by the positive heading so North stays at the top of the screen.
            compassImage.localRotation = Quaternion.Euler(0, 0, currentHeading);
        }
    }



    void UpdateDebug()
    {
        debugText.text = $"<b>STATUS:</b> {Input.location.status}\n" +
                 $"<b>MODE:</b> {(followPlayer ? "FOLLOW" : "FREE")}\n" +
                 $"<b>ZOOM:</b> {currentZoom:F1}\n" +
                 $"<b>GPS:</b> {smoothGps.x:F6}, {smoothGps.y:F6}\n" +
                 $"<b>ACC:</b> {Input.location.lastData.horizontalAccuracy:F1}m";

    }
    void UpdateGPSLabel()
    {
        if (gpsLabel == null || playerMarker == null) return;

        // Show smooth GPS coordinates
        gpsLabel.text = $"Lat: {smoothGps.x:F6}\nLon: {smoothGps.y:F6}\nAcc: {Input.location.lastData.horizontalAccuracy:F1}m";

        // Position label above player marker
        RectTransform labelRect = gpsLabel.GetComponent<RectTransform>();
        if (labelRect != null)
        {
            // 50 pixels above marker (adjust if needed)
            labelRect.position = playerMarker.position + new Vector3(0, 50f, 0);
        }
    }



    // Call this to toggle follow mode
    public void ToggleFollow()
    {
        followPlayer = true;
        dragVelocity = Vector2.zero;
        if (gpsReady)
            map.UpdateMap(smoothGps, currentZoom);
    }
}
