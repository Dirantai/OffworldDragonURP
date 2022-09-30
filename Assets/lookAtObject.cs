using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lookAtObject : MonoBehaviour
{

    public Transform lookObject;

    // Update is called once per frame
    void Update()
    {
        if(lookObject != null){
            transform.LookAt(lookObject);
        }
    }
}
