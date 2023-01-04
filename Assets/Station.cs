using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour
{
    public List<Transform> padPoints = new List<Transform>();
    public Transform player;

    // Start is called before the first frame update
    void Start()
    {
        for (int i=0;i<transform.childCount;i++){
            if(transform.GetChild(i).tag == "LandingPad") padPoints.Add(transform.GetChild(i));
        }
    }

    // Update is called once per frame
    void Update()
    {
        float distanceFromPlayer = (transform.position - player.position).magnitude;
        float closestDistance = distanceFromPlayer;
        Transform closestPad = null;
        if(distanceFromPlayer <= 500){
            foreach (Transform t in padPoints){
                distanceFromPlayer = (t.position - player.position).magnitude;
                if (distanceFromPlayer < closestDistance){
                    closestDistance = distanceFromPlayer;
                    closestPad = t;
                }
            }
            player.GetComponent<ShipSystem2>().SetLandingPoint(closestPad);
        }
    }
}
