using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;

public class EggBehavior : MonoBehaviour
{
    public AbstractMap map;
    public Transform player;

    [Header("Egg GPS Position")]
    public Vector2d geoPosition;

    [Header("AR Settings")]
    public float arDistance = 5f;


    [Header("Distance Settings")]
    public float visibleDistance = 50f; // meters
    public float glowDistance = 15f;    // meters

    private Renderer eggRenderer;
    private Material eggMaterial;
    private bool isGlowing = false;

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

            DisableGlow();
            return;
        }

        // 2?? Glow control
        if (distance <= glowDistance)
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
        }

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
}
