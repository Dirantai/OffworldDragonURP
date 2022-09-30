using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchVelocitySystem : MonoBehaviour
{
    public Transform player;
    public Transform world;
    public float velocityMatchDistance;
    public float rotationMatchDistance;
    private bool toggle;

    void Start(){
        player = GameObject.FindGameObjectWithTag("Player").transform;
        world = GameObject.FindGameObjectWithTag("World").transform;
    }

    void Update()
    {
        if(player != null){
            float distance = (player.position - transform.position).magnitude;
            if(distance < velocityMatchDistance){
                toggle = true;
                if(distance < rotationMatchDistance){
                    player.parent = transform;
                }else{
                    player.parent = transform.parent;
                }
            }else{
                if(toggle){
                    toggle = false;
                    player.parent = world;
                }
            }
        }
    }
}
