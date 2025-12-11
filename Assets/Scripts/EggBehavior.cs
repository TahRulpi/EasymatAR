using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;

public class EggBehavior : MonoBehaviour
{
    public AbstractMap map;
    public Transform player;

    [Header("Egg GPS Position")]
    public Vector2d geoPosition;

    [Header("Distance Settings")]
    public float activationDistance = 10f; // meters

    private bool isActive = false;

    void Update()
    {
        if (player == null || map == null) return;

        Vector3 worldPos = map.GeoToWorldPosition(geoPosition, true);
        transform.position = worldPos;

        Debug.Log($"[EGG UPDATE] GPS({geoPosition.x}, {geoPosition.y}) ? World({worldPos})");

        float distance = Vector3.Distance(player.position, worldPos);

        // ?? Debug world position + distance
        Debug.Log($"EGG: {geoPosition.x}, {geoPosition.y} | WORLD POS = {worldPos} | DIST = {distance}");

        if (!isActive && distance <= activationDistance)
            ActivateEgg();

        if (isActive && distance > activationDistance)
            DeactivateEgg();

        Debug.Log("Egg world pos: " + worldPos);

    }

    void ActivateEgg()
    {
        isActive = true;
        gameObject.SetActive(true);
        Debug.Log("Egg Activated!");
    }

    void DeactivateEgg()
    {
        isActive = false;
        gameObject.SetActive(false);
        Debug.Log("Egg Deactivated.");
    }
}
