using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;

public class RealMapController : MonoBehaviour
{
    public AbstractMap map;

    public float panSpeed = 0.0005f;
    public int minZoom = 14;
    public int maxZoom = 19;

    private int currentZoom;
    private Vector2d center;
    private float lastPinchDistance;
    private bool userInteracting;

    void Start()
    {
        Input.location.Start();
        currentZoom = Mathf.RoundToInt(map.Zoom);
        center = map.CenterLatitudeLongitude;
    }

    void Update()
    {
        HandleTouch();
        HandleGPS();
    }

    // ---------------- TOUCH ----------------
    void HandleTouch()
    {
        if (Input.touchCount == 1)
        {
            userInteracting = true;

            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Moved)
            {
                Vector2 delta = t.deltaPosition;
                center.x -= delta.y * panSpeed;
                center.y -= delta.x * panSpeed;

                map.UpdateMap(center, currentZoom);
            }
        }
        else if (Input.touchCount == 2)
        {
            userInteracting = true;

            Touch t1 = Input.GetTouch(0);
            Touch t2 = Input.GetTouch(1);

            float dist = Vector2.Distance(t1.position, t2.position);

            if (lastPinchDistance == 0)
            {
                lastPinchDistance = dist;
                return;
            }

            float delta = dist - lastPinchDistance;

            if (Mathf.Abs(delta) > 15f)
            {
                currentZoom += delta > 0 ? 1 : -1;
                currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
                map.UpdateMap(center, currentZoom);
                lastPinchDistance = dist;
            }
        }
        else
        {
            lastPinchDistance = 0;
            userInteracting = false;
        }
    }

    // ---------------- GPS ----------------
    void HandleGPS()
    {
        if (userInteracting)
            return;

        if (Input.location.status != LocationServiceStatus.Running)
            return;

        Vector2d gps = new Vector2d(
            Input.location.lastData.latitude,
            Input.location.lastData.longitude
        );

        center = gps;
        map.UpdateMap(center, currentZoom);
    }
}
