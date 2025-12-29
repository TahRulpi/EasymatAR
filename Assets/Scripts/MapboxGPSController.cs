using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;

public class MapboxGPSController : MonoBehaviour
{
    public AbstractMap map;

    public float zoomSpeed = 0.01f;
    public int minZoom = 14;
    public int maxZoom = 19;

    private float lastDistance;
    private int currentZoom;

    void Start()
    {
        Input.location.Start();
        currentZoom = Mathf.RoundToInt(map.Zoom); // ? FIX
    }

    void Update()
    {
        // -------- GPS FOLLOW --------
        if (Input.location.status == LocationServiceStatus.Running)
        {
            Vector2d gpsPos = new Vector2d(
                Input.location.lastData.latitude,
                Input.location.lastData.longitude
            );

            map.UpdateMap(gpsPos, currentZoom);
        }

        // -------- PINCH ZOOM --------
        if (Input.touchCount == 2)
        {
            Touch t1 = Input.GetTouch(0);
            Touch t2 = Input.GetTouch(1);

            float dist = Vector2.Distance(t1.position, t2.position);

            if (lastDistance == 0)
            {
                lastDistance = dist;
                return;
            }

            float delta = dist - lastDistance;

            if (Mathf.Abs(delta) > 2f)
            {
                currentZoom += delta > 0 ? 1 : -1;
                currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

                map.UpdateMap(map.CenterLatitudeLongitude, currentZoom);
            }

            lastDistance = dist;
        }

        if (Input.touchCount < 2)
            lastDistance = 0;
    }
}
