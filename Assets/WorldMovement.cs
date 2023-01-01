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
        
        foreach(PlanetInfo wObject in worldObjects){
            Vector3 playerToObject = wObject.transform.position - Camera.main.transform.position;
            float parentDistance = 0;
            float distance = playerToObject.magnitude;
            if (wObject.parentBody == null){
                wObject.currentDistance = 4000;
            }else{
                parentDistance = (wObject.parentBody.transform.position - Camera.main.transform.position).magnitude;
            }

            if(distance > wObject.farHideDistance){
                wObject.planet.SetActive(false);
            }else{
                wObject.planet.SetActive(true);
            }
            MaterialPropertyBlock newMat = new MaterialPropertyBlock();
            Color originalColour = wObject.planetIcon.GetComponentInChildren<Renderer>().material.GetColor("_Color");
            newMat.SetColor("_Color", new Color(originalColour.r, originalColour.g, originalColour.b, 1 - SmoothStep(wObject.hideDistance, wObject.farHideDistance, distance)));
            wObject.planetIcon.GetComponentInChildren<Renderer>().SetPropertyBlock(newMat);
            if(distance < wObject.hideDistance){
                wObject.planetIcon.gameObject.SetActive(false);
            }else{
                wObject.planetIcon.gameObject.SetActive(true);
                if(wObject.parentBody != null){
                    if(parentDistance > distance){
                        wObject.currentDistance = wObject.parentBody.currentDistance - 300;
                    }else{
                        wObject.currentDistance = wObject.parentBody.currentDistance + 300;
                    }
                }
                wObject.planetIcon.position = Camera.main.transform.position + (playerToObject.normalized * wObject.currentDistance);
                float closeRadius = (wObject.planetRadius * wObject.currentDistance) / distance;
                float scale = 1000 * (closeRadius / wObject.planetRadius);
                wObject.planetIcon.localScale = new Vector3(scale, scale, scale);
            }
        }
    }

    float SmoothStep(float start, float end, float input){
        float output = 0;

        float x = (input - end) / (start - end);

        output = x * x * (3 - (2 * x));

        if(input < start) output = 1;

        if(input > end) output = 0;

        return output;
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
