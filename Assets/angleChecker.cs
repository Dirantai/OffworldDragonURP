using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class angleChecker : MonoBehaviour
{
    public float startingAngle;
    public Transform point;

    // Update is called once per frame
    void Update()
    {
        startingAngle = Vector3.SignedAngle(transform.forward, transform.position - point.position, transform.up);
        if(startingAngle > 360){
            startingAngle = startingAngle - 360;
        }else if(startingAngle < 0){
            startingAngle = startingAngle + 360;
        }
    }
}
