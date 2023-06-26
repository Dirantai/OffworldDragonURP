using UnityEngine;

public class RadarSystem : MonoBehaviour
{

    public GameObject[] markers;
    public GameObject markerPrefab;
    public Transform cam;
    public Transform player;
    public float RadarSize;

    void LateUpdate()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (player != null)
        {

            if (markers.Length != enemies.Length)
            {
                foreach (GameObject marker in markers)
                {
                    Destroy(marker);
                }
                markers = new GameObject[enemies.Length];
            }

            for (int enemy = 0; enemy < enemies.Length; enemy++)
            {
                if (markers[enemy] == null)
                {
                    markers[enemy] = Instantiate(markerPrefab, transform.position, transform.rotation, transform);
                }
                Vector3 playerToEnemy = enemies[enemy].transform.position - player.position;
                float dotFront = Vector3.Dot(player.forward, playerToEnemy);
                float dotRight = Vector3.Dot(player.right, playerToEnemy.normalized);
                float dotUp = Vector3.Dot(player.up, playerToEnemy.normalized);

                float radarDistance = Mathf.Clamp(playerToEnemy.magnitude, 0, RadarSize);
                Vector3 radarPosition = (transform.right * (dotRight * 3)) + (transform.up * (dotUp * 3));
                float distance = radarPosition.magnitude;

                if (dotFront < 0)
                {
                    distance = RadarSize;
                }

                Vector3 updatePosition = transform.position + (radarPosition.normalized * Mathf.Clamp(distance, 0, RadarSize));

                markers[enemy].transform.position = updatePosition;
                markers[enemy].transform.GetChild(0).rotation = enemies[enemy].transform.rotation;
                markers[enemy].transform.GetChild(1).LookAt(cam);
            }
        }
        else
        {
            foreach (GameObject marker in markers)
            {
                Destroy(marker);
            }
        }
    }
}
