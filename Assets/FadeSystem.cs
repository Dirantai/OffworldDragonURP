using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeSystem : MonoBehaviour
{
    private Image fade;
    private float timer;
    public float fadeDuration;
    // Start is called before the first frame update
    void Start()
    {
        fade = GetComponent<Image>();
        Color fadeColour = new Color(0, 0, 0, 1);
        fade.color = fadeColour;
        timer = fadeDuration + 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if(timer > 0){
            timer -= Time.deltaTime;
        }else{
            Destroy(gameObject);
        }
        Color fadeColour = new Color(0, 0, 0, timer / fadeDuration);
        fade.color = fadeColour;
    }
}
