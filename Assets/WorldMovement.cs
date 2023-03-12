using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMovement : MonoBehaviour
{
    private Transform player;
    public Transform startingPoint;
    public UIElementHandler uiHandler;

    private bool done;

    public PlanetInfo[] worldObjects;

    private PlanetInfo sun;

    public float cellSize;

    void Start(){
        player = GameObject.FindGameObjectWithTag("Player").transform;
        player.position = startingPoint.position;
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
    float closeRadius = 0;
    void HandleDistantObjects(){
        
        foreach(PlanetInfo wObject in worldObjects){
            Vector3 playerToObject = wObject.transform.position - Camera.main.transform.position;
            float parentDistance = 50;
            float distance = playerToObject.magnitude;
            float fakeDistance = wObject.currentDistance;

            if (wObject.parentBody == null){
                wObject.currentDistance = 50;
            }else{
                parentDistance = (wObject.parentBody.transform.position - Camera.main.transform.position).magnitude;
            }

            MaterialPropertyBlock newMat = new MaterialPropertyBlock();
            Color originalColour = wObject.planetIcon.GetComponentInChildren<Renderer>().material.GetColor("_Color");
            newMat.SetColor("_Color", new Color(originalColour.r, originalColour.g, originalColour.b, 1 - SmoothStep(2400, 2490, distance)));
            wObject.planetIcon.GetComponentInChildren<Renderer>().SetPropertyBlock(newMat);

            if(distance > 2400){
                wObject.planetIcon.gameObject.SetActive(true);
                if(distance > 2500){
                    wObject.planetIcon.gameObject.layer = LayerMask.NameToLayer("WorldIcon");
                    wObject.planet.SetActive(false);
                }else{
                    fakeDistance = distance - 100;
                    wObject.planetIcon.gameObject.layer = LayerMask.NameToLayer("Default");
                    wObject.planet.SetActive(true);
                }
            }else{
                wObject.planetIcon.gameObject.SetActive(false);
            }

            if(wObject.parentBody != null){
                if(parentDistance > distance){
                    wObject.currentDistance = wObject.parentBody.currentDistance - 10;
                }else{
                    wObject.currentDistance = wObject.parentBody.currentDistance + (10 + closeRadius);
                }
            }
            
            closeRadius = (wObject.planetRadius * fakeDistance) / distance;
            wObject.planetIcon.position = Camera.main.transform.position + (playerToObject.normalized * fakeDistance);

            float scale = 1000 * (closeRadius / wObject.planetRadius);
            wObject.planetIcon.localScale = new Vector3(scale, scale, scale);
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
            uiHandler.MoveElements(direction * -distance);
            for (int o = 0; o < transform.childCount; o++){
                transform.GetChild(o).position += direction * -distance;
            }
        }else if(Mathf.Abs(distance) < cellSize){
            done = false;
        }
    }
}
