using Mapbox.Unity.Map;
using Mapbox.Utils;
using System;
using UnityEngine;

public class EggBehavior : MonoBehaviour
{
    [Header("Mapbox")]
    public AbstractMap map;
    public Transform player;

    public DateTime spawnTime;

    [Header("Egg GPS Position")]
    public Vector2d geoPosition;

    [Header("AR Settings")]
    public float arDistance = 5f;

    [Header("Distance Settings")]
    public float visibleDistance = 50f;

    private Renderer eggRenderer;
    private Material eggMaterial;

    [Header("Egg Info")]
    public EggType eggType;
    public bool isCollectable = true;

    [HideInInspector] public bool isCollected;

    void Start()
    {
        eggRenderer = GetComponentInChildren<Renderer>();
        eggMaterial = eggRenderer.material;

        // Start with glow OFF
        eggMaterial.DisableKeyword("_EMISSION");

        // Force visible on map
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (player == null || map == null)
            return;

        // Update egg position on map
        Vector3 worldPos = map.GeoToWorldPosition(geoPosition, true);
        worldPos.y += 0.5f; // optional: above map
        transform.position = worldPos;

        // Optionally, hide if very far from player
        // Comment out if you want always visible
        //float distance = Vector3.Distance(player.position, worldPos);
        //gameObject.SetActive(distance <= visibleDistance);
    }

    // Optional: CanSeeEgg logic simplified
    public bool CanSeeEgg()
    {
        SubscriptionTier tier = GameManager.Instance.currentTier;
        switch (tier)
        {
            case SubscriptionTier.None:
                return eggType == EggType.Red || eggType == EggType.Green;
            case SubscriptionTier.Pro:
                return true;
            case SubscriptionTier.Premium:
                return true;
            default:
                return false;
        }
    }
}





