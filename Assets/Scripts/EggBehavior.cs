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
        gameObject.SetActive(false);
    }

    void Update()
    {

        if (!CanSeeEgg())
        {
            gameObject.SetActive(false); // Hide egg if not visible for current tier
            return;
        }

        gameObject.SetActive(true); // Make sure it’s visible if allowed


        if (player == null || map == null) return;

        Vector3 worldPos = map.GeoToWorldPosition(geoPosition, true);
        transform.position = worldPos;

        float distance = Vector3.Distance(player.position, worldPos);

        // 1?? Visibility control
        if (distance <= visibleDistance)
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
        }
        else
        {
            if (gameObject.activeSelf)
                gameObject.SetActive(false);

          //  DisableGlow();
            return;
        }

        // 2?? Glow control
        /*if (distance <= glowDistance)
        {
            EnableGlow();
        }
        else
        {
            DisableGlow();
        }

        if (distance <= arDistance)
        {
            // Egg can be collected in AR
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
        else
        {
            DisableGlow();
        }*/

    }

    void EnableGlow()
    {
        if (isGlowing) return;

        eggMaterial.EnableKeyword("_EMISSION");
        eggMaterial.SetColor("_EmissionColor", Color.yellow * 2f);
        isGlowing = true;
    }

    void DisableGlow()
    {
        if (!isGlowing) return;

        eggMaterial.SetColor("_EmissionColor", Color.black);
        eggMaterial.DisableKeyword("_EMISSION");
        isGlowing = false;
    }
    private bool CanSeeEgg()
    {
        SubscriptionTier tier = GameManager.Instance.currentTier;

        switch (tier)
        {
            case SubscriptionTier.None:
                // Normal users only see Red and Green
                return eggType == EggType.Red || eggType == EggType.Green;

            case SubscriptionTier.Pro:
                // Pro sees all eggs, but no early access
                return true;

            case SubscriptionTier.Premium:
                // Premium sees all eggs + early access (spawn 30 min earlier)
                DateTime premiumVisibleTime = spawnTime.AddMinutes(-30);
                return DateTime.Now >= premiumVisibleTime;

            default:
                return false;
        }
    }

}
