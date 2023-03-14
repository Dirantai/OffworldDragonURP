using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public Transform skyCamera;
    public Transform playerCam;

    // Update is called once per frame
    void LateUpdate()
    {
        OnUpdate();
    }

    public virtual void OnUpdate(){
        if(player != null){
            transform.position = player.position;
        }
        if(skyCamera != null && playerCam != null){
            skyCamera.rotation = playerCam.rotation;
        }
    }
}
