using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothSpawn : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    bool fadingIn;
    bool fadingOut;
    Color color;
    [SerializeField] float fadeSpeed;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        color = spriteRenderer.color;
        color.a = 0;
        spriteRenderer.color = color;
        fadingIn = true;
    }


    void Update()
    {
        if (fadingIn)
        {
            color.a += Time.deltaTime * fadeSpeed;

            if(color.a >= 1f)
            {
                color.a = 1f;
                fadingIn = false;
            }

            spriteRenderer.color = color;
        }
        else if (fadingOut)
        {
            color.a -= Time.deltaTime * fadeSpeed;
            if (color.a <= 0f)
            {
                color.a = 0f;
                fadingOut = false;
            }
            spriteRenderer.color = color;
            if(color.a == 0f)
                Destroy(this.gameObject);
        }
    }

    public void FadeOut()
    {
        fadingIn = false;
        fadingOut = true;
    }
}
