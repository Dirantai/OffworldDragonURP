using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMovement : MonoBehaviour
{
    private Transform player;

    private bool done;

    public PlanetInfo[] worldObjects;

    private PlanetInfo sun;

    public float cellSize;

    void Start(){
        player = GameObject.FindGameObjectWithTag("Player").transform;
        worldObjects = GameObject.FindObjectsOfType<PlanetInfo>();
        foreach(PlanetInfo wObject in worldObjects){
            if(wObject.sun) sun = wObject;
        }
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
        float sunDistance = (sun.transform.position - Camera.main.transform.position).magnitude;
        foreach(PlanetInfo wObject in worldObjects){
            Vector3 playerToObject = wObject.transform.position - Camera.main.transform.position;
            float distance = playerToObject.magnitude;
            float currentMultiplier = wObject.farDistance;

            if(distance > wObject.farHideDistance){
                wObject.planet.SetActive(false);
            }else{
                wObject.planet.SetActive(true);
            }

            if(distance < wObject.hideDistance){
                wObject.planetIcon.gameObject.SetActive(false);
            }else{
                wObject.planetIcon.gameObject.SetActive(true);
                if(sunDistance < distance && wObject != sun){
                    currentMultiplier /= 10;
                }
                wObject.planetIcon.position = Camera.main.transform.position + (playerToObject.normalized * (distance / currentMultiplier));
                float closeRadius = wObject.planetRadius / (currentMultiplier);
                float scale = 1000 * (closeRadius / wObject.planetRadius);
                wObject.planetIcon.localScale = new Vector3(scale, scale, scale);
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
