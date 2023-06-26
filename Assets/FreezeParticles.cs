using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeParticles : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PauseStar());
    }

    IEnumerator PauseStar(){
        ParticleSystem particles = gameObject.GetComponent<ParticleSystem>();
        yield return new WaitForSeconds(0.1f);
        particles.Pause();
    }
}
