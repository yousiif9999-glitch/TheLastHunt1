using System.Collections;
using UnityEngine;

public class SoftLightning : MonoBehaviour
{
    public Light lightningLight;
    public float minWait = 5f;
    public float maxWait = 9f;

    void Start()
    {
        if (lightningLight != null)
            lightningLight.enabled = false;

        StartCoroutine(LightningRoutine());
    }

    IEnumerator LightningRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWait, maxWait));

            yield return StartCoroutine(Flash(0.06f));
            yield return new WaitForSeconds(0.15f);
            yield return StartCoroutine(Flash(0.04f));
        }
    }

    IEnumerator Flash(float time)
    {
        if (lightningLight != null)
            lightningLight.enabled = true;

        yield return new WaitForSeconds(time);

        if (lightningLight != null)
            lightningLight.enabled = false;
    }
}