using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraAvoidance : MonoBehaviour
{

    public Transform camRig;

    float maxDistance;

    float minDistance;

    float accumulatedTime;
    Vector3 hitPos;

    void Start(){
        maxDistance = camRig.localPosition.z;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float currentDistance = 0;
        float sign = 1;

        hitPos = Vector3.zero;
        if(Physics.Raycast(transform.position, -transform.forward, out RaycastHit hit, 40)){
            hitPos = hit.point;
        }

        if(hitPos == Vector3.zero){
            sign = -1;
        }else{
            sign = 1;
        }

        accumulatedTime = Mathf.Clamp(accumulatedTime + (Time.deltaTime * sign), 0, 1);

        currentDistance = Mathf.Lerp(maxDistance, 0, accumulatedTime / 1);

        camRig.localPosition = new Vector3(camRig.localPosition.x, camRig.localPosition.y, currentDistance);
    }
}
