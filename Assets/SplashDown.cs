using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashDown : MonoBehaviour
{
    public float magnitude;
    public float speed = 6;

    private float elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(1, 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += speed * Time.deltaTime;
        transform.localScale = Vector3.Lerp(new Vector3(1, 1, 0), new Vector3(1, 1, magnitude), Mathf.Sin(elapsedTime));

        if(Mathf.Sin(elapsedTime) < 0){
            Destroy(gameObject);
        }
    }
}
