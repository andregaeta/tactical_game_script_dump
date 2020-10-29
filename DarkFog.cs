using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DarkFog : MonoBehaviour
{
    private static DarkFog _instance;

    public static DarkFog Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

    }

    [SerializeField] float fadeDuration;
    [SerializeField] float maxFade;
    float fadeTime;
    bool fadingIn;
    bool fadingOut;
    Image img;
    public void FadeIn(List<GameObject> highlightedObjects)
    {
        foreach(GameObject obj in highlightedObjects)
        {
            if (obj == null) continue;
            obj.GetComponent<SpriteRenderer>().sortingLayerName = "Highlights";
        }

        fadeTime = fadeDuration;
        fadingIn = true;
        fadingOut = false;
    }
    public void FadeOut(List<GameObject> highlightedObjects)
    {
        fadeTime = fadeDuration;
        fadingIn = false;
        fadingOut = true;

        foreach (GameObject obj in highlightedObjects)
        {
            if (obj == null) continue;
            obj.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
        }
    }

    private void Start()
    {
        img = GetComponent<Image>();
    }
    private void Update()
    {
        if (fadingIn)
        {
            if(fadeTime > 0)
            {
                fadeTime -= Time.deltaTime;
                Color color = img.color;
                color.a = Mathf.MoveTowards(img.color.a, maxFade, maxFade/fadeDuration * Time.deltaTime);
                img.color = color;
            }
            else
            {
                fadingIn = false;
            }
        }
        else if (fadingOut)
        {
            if (fadeTime > 0)
            {
                fadeTime -= Time.deltaTime;
                Color color = img.color;
                color.a = Mathf.MoveTowards(img.color.a, 0f, maxFade / fadeDuration * Time.deltaTime);
                img.color = color;
            }
            else
            {
                fadingOut = false;
            }
        }
    }
}
