using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogSetter : MonoBehaviour
{

    private bool done;
    public Transform player;
    public float atmosphericStart;
    public float atmosphericEnd;
    public float atmosphericDensity;
    public Color atmosphericColour;
    public float waterStart;
    public float waterEnd;
    public float density;
    public float layerTop;
    public float layerBottom;
    public Color waterColour;
    public GameObject topLayer;
    public GameObject bottomLayer;

    public GameObject splashEffect;

    private int previousObjectCount;
    private Transform world;

    private GameObject[] enemies;

    void Start(){
        enemies = new GameObject[0];
        world = GameObject.FindGameObjectWithTag("World").transform;
        previousObjectCount = world.childCount;
        player.GetComponent<ShipSystem2>().SetOrbittingBody(transform, 10);
    }

    void Update(){
        if(player != null){
            float distance = (player.position - transform.position).magnitude;
            if(distance <= atmosphericStart && distance >= atmosphericEnd){
                if(done){
                    RenderSettings.fogColor = atmosphericColour;
                    RenderSettings.fogEndDistance = 0;
                }   
                done = false;
                RenderSettings.fogEndDistance = Mathf.Lerp(RenderSettings.fogEndDistance, atmosphericDensity, 30 * Time.deltaTime);
            }else if (distance <= atmosphericEnd){
                done = true;
                RenderSettings.fogColor = waterColour;
                float smoothLerp = SmoothStep(waterEnd, waterStart, distance);
                RenderSettings.fogEndDistance = Mathf.Lerp(30000, density, smoothLerp);
            }else{
                if(!done){
                    RenderSettings.fogEndDistance = 30000;
                    done = true;
                }
            }

            if(distance > atmosphericStart){
                topLayer.SetActive(true);
                bottomLayer.SetActive(false);
            }else{

                if(distance >= waterEnd){
                    topLayer.SetActive(true);
                }else{
                    topLayer.SetActive(false);
                }

                if(distance <= waterStart){
                    bottomLayer.SetActive(true);
                }else{
                    bottomLayer.SetActive(false);
                }

            }

            ObjectChecker(distance, player);
        }

        if(previousObjectCount != world.childCount){
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            previousObjectCount = world.childCount;
        }

        if(enemies.Length >= 1){
            for (int i=0; i < enemies.Length;i++){
                if(enemies[i] != null){
                    float distance = (enemies[i].transform.position - transform.position).magnitude;
                    ObjectChecker(distance, enemies[i].transform);
                }
            }
        }
    }

    void ObjectChecker(float distance, Transform rigidObject){
        Rigidbody rigid = rigidObject.GetComponent<Rigidbody>();
        ShipSystem2 splashTracker = rigidObject.GetComponent<ShipSystem2>();
        if(rigid != null && distance < atmosphericEnd){
            if((distance < waterStart && !splashTracker.GetTracker()) || (distance > waterStart && splashTracker.GetTracker())){
                splashTracker.SetTracker(!splashTracker.GetTracker());
                Vector3 velocity = rigid.velocity;
                if(velocity.magnitude > 20){
                    Vector3 vectorToObject = (rigidObject.position - transform.position).normalized;
                    GameObject g = Instantiate(splashEffect, transform.position + (vectorToObject * waterEnd), Quaternion.LookRotation(vectorToObject)) as GameObject;
                    g.transform.parent = transform;
                }
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
}
