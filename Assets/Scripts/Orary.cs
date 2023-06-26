using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Orary : MonoBehaviour
{

    private bool open;

    public GameObject planetPrefab;
    public Planet[] planets;

    [System.Serializable]
    public struct Planet{
        public SimplifiedOrbitSystem orbitData;
        public Transform planetIcon;
    }


    // Start is called before the first frame update
    void Start()
    {
        SimplifiedOrbitSystem[] orbittingBodies = FindObjectsOfType<SimplifiedOrbitSystem>();
        planets = new Planet[orbittingBodies.Length];
        GenerateMap(orbittingBodies);
    }

    // Update is called once per frame
    void Update()
    {
        MovePlanets();
    }

    void GenerateMap(SimplifiedOrbitSystem[] orbittingBodies){
        int count = 0;
        foreach(SimplifiedOrbitSystem orbitData in orbittingBodies){
            GameObject p = Instantiate(planetPrefab, transform.position, Quaternion.Euler(0,0,0)) as GameObject;
            Transform Pt = p.transform;
            planets[count].orbitData = orbitData;
            planets[count].planetIcon = Pt;

            bool hasParent = false;
            int parentIndex = 0;

            for (int i = 0;i < planets.Length; i++){
                if(planets[i].orbitData != null){
                    if(orbitData.attractorObject == planets[i].orbitData.transform){
                        hasParent = true;
                        parentIndex = i;
                    }
                }
            }

            if(hasParent){
                Pt.parent = planets[parentIndex].planetIcon.GetChild(0);
                Pt.localPosition = Vector3.zero;
            }else{
                Pt.parent = transform;
            }

            count++;
        }
        MovePlanets();
    }

    void MovePlanets(){
        foreach(Planet planet in planets){
            planet.planetIcon.GetChild(0).localPosition = new Vector3(0, (float)planet.orbitData.orbitDistance / 10000, 0);
            planet.planetIcon.localRotation = Quaternion.Euler(0, 0, -(float)planet.orbitData.startingAngle);
        }
    }

    void PlanetDataDisplay(PlanetData planet)
    {

    }

    void HandleTransitions()
    {

    }

    void PlanetSelection()
    {

    }

    void HoverPlanet(PlanetData planet)
    {

    }
}
