using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplifiedOrbitSystem : MonoBehaviour
{
    public Transform attractorObject;
    public Transform orbitNormal;
    public Transform body;
    public float orbitAngularSpeed;
    public float orbitDistance;
    public float angularSpeed;
    public float startingAngle;
    public Vector3 orbitVector;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 objectToAttractor = transform.position - attractorObject.position;
        Vector3 maintainDistance = objectToAttractor.normalized * (orbitDistance - objectToAttractor.magnitude);
        transform.position += maintainDistance;
        if(orbitNormal == null){
            orbitNormal = attractorObject;
        }else{
            orbitNormal.localPosition = Vector3.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {

        Debug.DrawLine(attractorObject.position, attractorObject.position + orbitNormal.forward * orbitDistance);
        startingAngle += orbitAngularSpeed * Time.deltaTime;

        if(startingAngle >= 360){
            startingAngle = 360 - startingAngle;
        }else if(startingAngle <= -360){
            startingAngle = startingAngle + 360;
        }

        Vector3 orbitPosition = attractorObject.position +
                                (orbitNormal.right * Mathf.Sin(startingAngle * Mathf.Deg2Rad) + orbitNormal.forward * Mathf.Cos(startingAngle * Mathf.Deg2Rad)) * orbitDistance;
        Debug.DrawLine(attractorObject.position, orbitPosition);

        transform.position = orbitPosition;

        //Calculate the cross product of radius vector and axis vector, allowing a tangent vector.
        //Also calculates planet's orbital velocity and adds it to tangent vector to get final global vector.
        Debug.DrawLine(attractorObject.position, attractorObject.position + orbitNormal.up, Color.yellow);
        Debug.DrawLine(transform.position, transform.position + orbitVector, Color.blue);

        //Used for handling player movement
        orbitVector = Vector3.Cross(orbitPosition - attractorObject.position, orbitNormal.up).normalized;

        Vector3 rotation = new Vector3(0, angularSpeed, 0);
        body.Rotate(rotation * Time.deltaTime);
    }
}
