using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlanetInfo : MonoBehaviour
{

    public PlanetInfo parentBody;
    public bool sun;
    public float planetRadius;
    public float planetScale;
    public float currentDistance;
    public float hideDistance = 4000;
    public GameObject planet;
    public Transform planetIcon;
}
