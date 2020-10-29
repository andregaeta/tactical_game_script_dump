using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private static CameraShake _instance;

    public static CameraShake Instance { get { return _instance; } }

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

    private float shakeTimeLeft, shakeStrenght, shakeFadeTime;

    public void ShakeCamera(float duration, float power)
    {
        shakeTimeLeft = duration;
        shakeStrenght = power;
        shakeFadeTime = power / duration;
    }

    private void Update()
    {
        if (shakeTimeLeft > 0)
        {
            shakeTimeLeft -= Time.deltaTime;

            float xAmount = Random.Range(-1f, 1f) * shakeStrenght;
            float yAmount = Random.Range(-1f, 1f) * shakeStrenght;

            transform.position += new Vector3(xAmount, yAmount, 0f);

            shakeStrenght = Mathf.MoveTowards(shakeStrenght, 0f, shakeFadeTime * Time.deltaTime);
        }
    }
}
