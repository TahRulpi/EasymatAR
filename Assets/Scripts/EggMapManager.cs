using UnityEngine;

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

        /*foreach (var egg in eggs)
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
        }*/
    }
}
