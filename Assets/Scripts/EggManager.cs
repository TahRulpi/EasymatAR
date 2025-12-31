/*using UnityEngine;

public class EggMapManager : MonoBehaviour
{
   // public EggData[] eggs;
    public RectTransform mapParent;
    public RectTransform playerIcon;
    public GameObject eggMarkerPrefab;

    void Update()
    {
        Vector2 playerPos = new Vector2(
            (float)LocationManager.Instance.latitude,
            (float)LocationManager.Instance.longitude);

        *//*foreach (var egg in eggs)
        {
            Vector2 eggPos = GPSToUnity.GPSDistance(
                playerPos.x, playerPos.y,
                (float)egg.latitude, (float)egg.longitude
            );

            Transform marker = mapParent.Find(egg.id);
            if (marker == null)
            {
                marker = Instantiate(eggMarkerPrefab, mapParent).transform;
                marker.name = egg.id;
            }

            marker.GetComponent<RectTransform>().anchoredPosition = eggPos;
        }*//*
    }
}
*/

using System.Collections.Generic;
using UnityEngine;

public class EggManager : MonoBehaviour
{
    public static EggManager Instance;

    public List<EggData> eggsToSpawn = new List<EggData>();
    public double playerLatitude;
    public double playerLongitude;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("EggManager initialized and persistent.");
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

