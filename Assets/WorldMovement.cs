using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMovement : MonoBehaviour
{
    private Transform player;

    private bool done;

    public PlanetInfo[] worldObjects;

    public float cellSize;

    void Start(){
        player = GameObject.FindGameObjectWithTag("Player").transform;
        worldObjects = GameObject.FindObjectsOfType<PlanetInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null){
            HandleDistantObjects();
            HandleWarpZone(player.position.x, Vector3.right);
            HandleWarpZone(player.position.y, Vector3.up);
            HandleWarpZone(player.position.z, Vector3.forward);
        }
    }

    void HandleDistantObjects(){
        foreach(PlanetInfo wObject in worldObjects){
            Vector3 playerToObject = wObject.transform.position - Camera.main.transform.position;
            float distance = playerToObject.magnitude;
            if(distance < wObject.hideDistance){
                wObject.planetIcon.gameObject.SetActive(false);
            }else{
                wObject.planetIcon.gameObject.SetActive(true);
                wObject.planetIcon.position = Camera.main.transform.position + (playerToObject.normalized * (distance / wObject.farDistance));
                float closeRadius = wObject.planetRadius / (wObject.farDistance);
                float scale = wObject.planetRadius / closeRadius;
                wObject.planetIcon.localScale = new Vector3(scale / 10, scale / 10, scale / 10);
            }
        }
    }

    void HandleWarpZone(float distance, Vector3 direction){
        if(Mathf.Abs(distance) > cellSize && !done){
            done = true;
            for (int o = 0; o < transform.childCount; o++){
                transform.GetChild(o).position += direction * -distance;
            }
        }

        if(Mathf.Abs(distance) < cellSize){
            done = false;
        }
    }
}
