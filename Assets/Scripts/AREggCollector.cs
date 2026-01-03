using UnityEngine;
using UnityEngine.InputSystem; // Fixes the InvalidOperationException
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

public class AREggCollector : MonoBehaviour
{
    private Camera arCamera;
    private static int totalCollectedThisSession = 0;

    void Start()
    {
        // Use the AR Camera found in your XR Origin
        arCamera = Camera.main;
    }

    void Update()
    {
        // New Input System check for mobile touch
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            Vector2 touchPosition = Pointer.current.position.ReadValue();
            Debug.Log("?? Touch Detected at: " + touchPosition);
            PerformRaycast(touchPosition);
        }
    }

    void PerformRaycast(Vector2 screenPos)
    {
        Ray ray = arCamera.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.collider.CompareTag("Egg"))
            {
                Debug.Log("?? Hit: " + hit.collider.name);
                EggBehavior eggScript = hit.collider.GetComponent<EggBehavior>();

                if (eggScript != null)
                {
                    CheckCollectionEligibility(eggScript); // This is the method that was missing
                }
            }
        }
    }

    // --- YOUR SUBSCRIPTION LOGIC ---
    void CheckCollectionEligibility(EggBehavior egg)
    {
        SubscriptionTier tier = GameManager.Instance.currentTier;

        switch (tier)
        {
            case SubscriptionTier.None:
                // Only Red/Green allowed AND only if zero eggs collected yet
                if ((egg.eggType == EggType.Red || egg.eggType == EggType.Green) && totalCollectedThisSession < 1)
                {
                    CollectEgg(egg.gameObject);
                }
                else if (totalCollectedThisSession >= 1)
                {
                    Debug.Log("None Tier: Limit reached (1 egg max).");
                }
                break;

            case SubscriptionTier.Pro:
                // All colors allowed BUT only if zero eggs collected yet
                if (totalCollectedThisSession < 1)
                {
                    CollectEgg(egg.gameObject);
                }
                else
                {
                    Debug.Log("Pro Tier: Limit reached (1 egg max).");
                }
                break;

            case SubscriptionTier.Premium:
                // All colors allowed, no limit on count
                CollectEgg(egg.gameObject);
                break;
        }
    }

    void CollectEgg(GameObject egg)
    {
        totalCollectedThisSession++;
        Debug.Log($"?? Egg Collected! Total this session: {totalCollectedThisSession}");
        Destroy(egg);
    }

    public static void ResetCollectionCount()
    {
        totalCollectedThisSession = 0;
    }
}