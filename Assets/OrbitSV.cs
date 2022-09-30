using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitSV : MonoBehaviour
{
    public Transform orbitPoint;
    public Transform orbittingBody;
    public float speed;
    public Vector3 orbitalVelocity = new Vector3(1, 0, 0);
    public float altitude;

    void Start(){
        if(orbitPoint != null){
            float currentVSDesired = altitude - (orbitPoint.position - orbittingBody.position).magnitude;
            transform.position += ((transform.position - orbittingBody.position).normalized * currentVSDesired);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if(orbittingBody != null && orbitPoint != null){
            orbitPoint.Rotate(orbitalVelocity * Time.deltaTime);
            transform.Translate(orbitPoint.forward * orbitalVelocity.magnitude);
            float currentVSDesired = altitude - (orbitPoint.position - orbittingBody.position).magnitude;
            transform.position += ((transform.position - orbittingBody.position).normalized * currentVSDesired);
        }else{
            transform.Rotate(orbitalVelocity * Time.deltaTime);
        }
    }
}
