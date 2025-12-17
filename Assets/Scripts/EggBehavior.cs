using Mapbox.Unity.Map;
using Mapbox.Utils;
using System;
using UnityEngine;

public class EggBehavior : MonoBehaviour
{
    [Header("Mapbox")]
    public AbstractMap map;
    public Transform player;

    public DateTime spawnTime; // Set this when spawning eggs


    [Header("Egg GPS Position")]
    public Vector2d geoPosition;

    [Header("AR Settings")]
    public float arDistance = 5f;


    [Header("Distance Settings")]
    public float visibleDistance = 50f; // meters
                                        //  public float glowDistance = 15f;    // meters

    private Renderer eggRenderer;
    private Material eggMaterial;
    private bool isGlowing = false;
    [Header("Egg Info")]
    public EggType eggType;
    public Vector2d gpsPosition;
    public bool isCollectable = true;




    // runtime states
    [HideInInspector] public bool isVisibleOnMap;
    [HideInInspector] public bool isCollected;
    void Start()
    {
        eggRenderer = GetComponentInChildren<Renderer>();
        eggMaterial = eggRenderer.material;

        // Start with glow OFF
        eggMaterial.DisableKeyword("_EMISSION");
        // gameObject.SetActive(false);
    }

    void Update()
    {

        if (player == null || map == null)
            return;

        // ?? Subscription visibility
        if (!CanSeeEgg())
        {
            if (gameObject.activeSelf)
                gameObject.SetActive(false);
            return;
        }

        // ?? Map position update
        Vector3 worldPos = map.GeoToWorldPosition(geoPosition, true);
        transform.position = worldPos;

        float distance = Vector3.Distance(player.position, worldPos);

        // ??? Distance-based visibility
        bool shouldBeVisible = distance <= visibleDistance;

        if (gameObject.activeSelf != shouldBeVisible)
            gameObject.SetActive(shouldBeVisible);

    }


    private bool CanSeeEgg()
    {
        SubscriptionTier tier = GameManager.Instance.currentTier;
        Debug.Log($"EGG {eggType} | Tier: {GameManager.Instance.currentTier} | Spawn: {spawnTime}");
        // ? Normal spawn check
        if (tier != SubscriptionTier.Premium)
        {
            if (DateTime.Now < spawnTime)
                return false;
        }

        switch (tier)
        {
            case SubscriptionTier.None:
                return eggType == EggType.Red || eggType == EggType.Green;

            case SubscriptionTier.Pro:
                return true;

            case SubscriptionTier.Premium:
                DateTime premiumTime = spawnTime.AddMinutes(-30);
                return DateTime.Now >= premiumTime;

            default:
                return false;
        }

       


    }
}
