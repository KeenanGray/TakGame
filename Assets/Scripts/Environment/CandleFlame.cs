using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleFlame : MonoBehaviour
{
    int seed;
    public float intenstityMin;
    public float intensityMax;

    public float minWaitTime;
    public float maxWaitTime;

    float waitTime;

    Light myLight;

    // Start is called before the first frame update
    void Start()
    {
        myLight = GetComponent<Light>();
        seed = transform.parent.gameObject.GetInstanceID();
        Random.InitState(seed);
        waitTime = Random.Range(minWaitTime, maxWaitTime);
        StartCoroutine(FlickerFlame());
    }

    IEnumerator FlickerFlame()
    {

        float intensity = 0;
        while (true)
        {
            intensity = Random.Range(intenstityMin, intensityMax);
            var start = myLight.intensity;
            myLight.intensity = intensity;

            //Lerp the light intensity
            //Speed of lerp is controled by that number in the loop
            for (int i = 0; i < waitTime; i++)
            {
                float t = (float)i / (float)waitTime;

                myLight.intensity = Mathf.Lerp(start, intensity, t);
                yield return new WaitForEndOfFrame();
            }
            waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return null;
        }
    }

}
