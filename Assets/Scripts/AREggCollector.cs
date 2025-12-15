using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class AREggCollector : MonoBehaviour
{
    private ARRaycastManager raycastManager;
    private Camera arCamera;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Start()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();
        arCamera = GetComponent<Camera>();
    }

    /*void Update()
    {
        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began)
            return;

        Ray ray = arCamera.ScreenPointToRay(touch.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Egg"))
            {
                CollectEgg(hit.collider.gameObject);
            }
        }
    }*/


    void Update()
    {
        // ?? REAL DEVICE TOUCH
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.CompareTag("Egg"))
                    {
                        CollectEgg(hit.collider.gameObject);
                    }
                }
            }
        }

        // ??? EDITOR MOUSE CLICK (for testing)
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = arCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Egg"))
                {
                    CollectEgg(hit.collider.gameObject);
                }
            }
        }
#endif
    }
    void CollectEgg(GameObject egg)
    {
        Debug.Log("?? Egg Collected!");
        //GameManager.Instance.AddScore(1);
        Destroy(egg);
    }
}
