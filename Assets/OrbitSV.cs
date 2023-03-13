using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitSV : MonoBehaviour
{
    public Transform player;
    //public Transform planetVelocity;
    public SimpleKeplerOrbits.KeplerOrbitMover orbitData;
    public float angularSpeed;
    public float SOF = 2500;

    // Update is called once per frame
    void Update()
    {
        Vector3 centreToPlayer = (player.position - transform.position);
        if(player != null && centreToPlayer.magnitude <= SOF){
            //calculate the player's position along the axis of rotation of the planet
            
            float top = (centreToPlayer.x * transform.up.x) + 
                        (centreToPlayer.y * transform.up.y) +
                        (centreToPlayer.z * transform.up.z);

            //A bit of calculation to get the direction and position
            Vector3 axisVector = (transform.up.normalized * -top);
            Vector3 axisPosition = transform.position - axisVector;

            //Ensure the cross product always follow the same direction. Angular velocity should change it.
            float mult = 1;
            if(top > 0){
                mult = -1;
            }

            //Calculate the cross product of radius vector and axis vector, allowing a tangent vector.
            //Also calculates planet's orbital velocity and adds it to tangent vector to get final global vector.
            Vector3 crossProd = Vector3.Cross(axisPosition - player.position, axisVector);
            //Vector3 pVelocity = planetVelocity.position - transform.position;

            Vector3 radiusVector = player.position - axisPosition;
            float radiansSpeed = angularSpeed * Time.deltaTime * Mathf.Deg2Rad;
            float linearSpeed = radiansSpeed * radiusVector.magnitude;

            //Since the player is never at the exact orbit distance as the planet, we need to calculate the player's orbital velocity from the planet's orbital velocity
            Vector3 planetVelocity = new Vector3((float)orbitData.OrbitData.Velocity.x, (float)orbitData.OrbitData.Velocity.y, (float)orbitData.OrbitData.Velocity.z);
            float planetDistance = (transform.position - orbitData.AttractorSettings.AttractorObject.position).magnitude;
            float attractorToPlayer = (player.position - orbitData.AttractorSettings.AttractorObject.position).magnitude;
            float planetAngular = planetVelocity.magnitude / planetDistance;
            Vector3 playerVector = planetVelocity.normalized * (planetAngular * attractorToPlayer) * Time.deltaTime;

            //Calculate the final vector
            Vector3 finalVector = ((playerVector * orbitData.TimeScale) + (crossProd.normalized * linearSpeed * mult));

            //Apply the position appropriately
            player.position += finalVector;
            //Rotate the player relative to the planet instead.
            player.Rotate(transform.up * angularSpeed * Time.deltaTime);
        }

        Vector3 rotation = new Vector3(0, angularSpeed, 0);
        transform.Rotate(rotation * Time.deltaTime);
    }
}
