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


        // --- NEW: Automatic Reference Finding ---
        if (map == null)
        {
            map = FindObjectOfType<AbstractMap>();
        }

        if (player == null)
        {
            // Make sure your Player object in the hierarchy is tagged "Player"
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        // Existing logic
        eggRenderer = GetComponentInChildren<Renderer>();
        if (eggRenderer != null)
        {
            eggMaterial = eggRenderer.material;
            eggMaterial.DisableKeyword("_EMISSION");
        }

        gameObject.SetActive(true);

        if (map == null) map = FindObjectOfType<Mapbox.Unity.Map.AbstractMap>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;

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







/*using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using System;

public class EggBehavior : MonoBehaviour
{
    [Header("Data")]
    public Vector2d geoPosition;
    public AbstractMap map;
    public Transform player;
    public DateTime spawnTime;
    public EggType eggType;
    public bool isCollectable;

    [Header("Settings")]
    public float collectionDistance = 10f; // Distance in Unity meters

    void Update()
    {
        // 1. Keep the egg anchored to the map's GPS position
        if (map != null)
        {
            Vector3 worldPos = map.GeoToWorldPosition(geoPosition, true);
            worldPos.y = transform.position.y; // Maintain current height/bounce
            transform.position = worldPos;
        }

        // 2. Simple distance check (Optional visual cue)
        if (player != null)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            // You could change the egg's color or glow here if dist < collectionDistance
        }
    }

    // This is called when the user clicks/taps the egg
    private void OnMouseDown()
    {
        CheckCollection();
    }

    public void CheckCollection()
    {
        if (!isCollectable) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= collectionDistance)
        {
            Collect();
        }
        else
        {
            Debug.Log($"Too far! You are {distance:F1}m away. Need to be within {collectionDistance}m.");
        }
    }

    private void Collect()
    {
        isCollectable = false;
        Debug.Log($"Collected {eggType} egg!");

        // Add logic here to update your inventory or score

        // Destroy the egg or play a collection effect
        Destroy(gameObject);
    }
}*/