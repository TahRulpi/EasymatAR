using UnityEngine;

public class AREggAutoSpawn : MonoBehaviour
{
    public GameObject eggPrefab;
    public float distanceFromCamera = 1.0f;

    private bool spawned = false;

    void Start()
    {
        if (spawned) return;

        Camera cam = Camera.main;
        Vector3 spawnPos = cam.transform.position + cam.transform.forward * distanceFromCamera;

        Instantiate(eggPrefab, spawnPos, Quaternion.identity);

        spawned = true;
    }
}
