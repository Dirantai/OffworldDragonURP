using System;
using System.Collections.Generic;
using UnityEngine;

public class SimplifiedOrbitSystem : MonoBehaviour
{
    public Transform attractorObject;
    public Transform orbitNormal;
    public Transform body;
    public float orbitAngularSpeed;
    public double orbitDistance;
    public float angularSpeed;
    public double startingAngle;
    public Vector3 orbitVector;

    // Start is called before the first frame update
    void Start()
    {
        if(orbitNormal == null){
            orbitNormal = attractorObject;
        }else{
            orbitNormal.localPosition = Vector3.zero;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Debug.DrawLine(attractorObject.position, attractorObject.position + orbitNormal.forward * (float)orbitDistance);
        startingAngle += orbitAngularSpeed * Time.deltaTime;

        if(startingAngle >= 360){
            startingAngle = startingAngle - 360;
        }else if(startingAngle <= -360){
            startingAngle = startingAngle + 360;
        }

        Vector3d up = new Vector3d(orbitNormal.up);
        Vector3d right = new Vector3d(orbitNormal.right);
        Vector3d forward = new Vector3d(orbitNormal.forward);
        Vector3d attractorPos = new Vector3d(attractorObject.position);

        Vector3d orbitPosition = attractorPos +
                                (right * Math.Sin(startingAngle * Vector3d.Deg2Rad) + forward * Math.Cos(startingAngle * Vector3d.Deg2Rad)) * orbitDistance;
        Debug.DrawLine(attractorObject.position, orbitPosition.toFloat());

        transform.position = orbitPosition.toFloat();

        //Calculate the cross product of radius vector and axis vector, allowing a tangent vector.
        //Also calculates planet's orbital velocity and adds it to tangent vector to get final global vector.
        Debug.DrawLine(attractorObject.position, attractorObject.position + orbitNormal.up, Color.yellow);
        Debug.DrawLine(transform.position, transform.position + orbitVector, Color.blue);

        //Used for handling player movement
        orbitVector = Vector3d.Cross(orbitPosition - attractorPos, up).normalized.toFloat();

        Vector3 rotation = new Vector3(0, angularSpeed, 0);
        body.Rotate(rotation * Time.deltaTime);
    }
}
