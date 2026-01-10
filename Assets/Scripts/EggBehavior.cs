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

    [Header("AR Mode")]
    public bool isARMode = false; // NEW: stop Mapbox movement in AR

    void Start()
    {
        // AR Camera = Player
        if (player == null && Camera.main != null)
        {
            player = Camera.main.transform;
        }

        // Map reference (only if really needed)
        if (map == null)
        {
            map = FindObjectOfType<AbstractMap>();
        }

        // Renderer & material
        eggRenderer = GetComponentInChildren<Renderer>();
        if (eggRenderer != null)
        {
            eggMaterial = eggRenderer.material;
            eggMaterial.DisableKeyword("_EMISSION");
        }

        gameObject.SetActive(true);
    }

    void Update()
    {
        // ?? In AR mode, AR spawner controls position — do nothing
        if (isARMode)
            return;

        // ?? Non-AR (Map mode) behavior
        if (player == null || map == null)
            return;

        Vector3 worldPos = map.GeoToWorldPosition(geoPosition, true);
        worldPos.y += 0.5f;
        transform.position = worldPos;
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
            case SubscriptionTier.Premium:
                return true;
            default:
                return false;
        }
    }
}


/*using Mapbox.Unity.Map;
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
    [Header("AR Mode")]
    public bool isARMode = false; // true = spawned in AR, ignore map updates

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
        // AR Camera = Main Camera
        if (player == null && Camera.main != null)
        {
            player = Camera.main.transform;
        }

        // Map reference (only if really needed)
        if (map == null)
        {
            map = FindObjectOfType<AbstractMap>();
        }

        // Renderer & material
        eggRenderer = GetComponentInChildren<Renderer>();
        if (eggRenderer != null)
        {
            eggMaterial = eggRenderer.material;
            eggMaterial.DisableKeyword("_EMISSION");
        }

        gameObject.SetActive(true);
    }


    void Update()
    {
        // ?? In AR mode, AR spawner controls position — do nothing
        if (isARMode)
            return;

        // ?? Non-AR (map mode) behavior stays SAME
        if (player == null || map == null)
            return;

        Vector3 worldPos = map.GeoToWorldPosition(geoPosition, true);
        worldPos.y += 0.5f;
        transform.position = worldPos;
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



*/