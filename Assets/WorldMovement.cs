using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMovement : MonoBehaviour
{
    private Transform player;

    private bool done;

    public float cellSize;

    void Start(){
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null){
            HandleWarpZone(player.position.x, Vector3.right);
            HandleWarpZone(player.position.y, Vector3.up);
            HandleWarpZone(player.position.z, Vector3.forward);
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
