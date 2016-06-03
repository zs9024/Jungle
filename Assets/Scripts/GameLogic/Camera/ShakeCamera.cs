using UnityEngine;
using System.Collections;
public class ShakeCamera : MonoBehaviour
{
    public float ShakeTime = 1f;
    public float ShakeStrength = 0.2f;
    private Vector3 ShakeOffset = Vector3.zero;
    private Vector3 preShakeOffset = Vector3.zero;

    void Start()
    {
        Shake(2f);
    }

    void LateUpdate()
    {
        transform.position = transform.position - preShakeOffset + ShakeOffset;
    }

    public void Shake()
    {
        StopCoroutine("ShakeCoroutine");
        StartCoroutine("ShakeCoroutine");
    }

    public void Shake(float time)
    {
        ShakeTime = time;
        Shake();
    }

    IEnumerator ShakeCoroutine()
    {
        float endTime = Time.time + ShakeTime;
        while (Time.time < endTime)
        {
            ShakeOffset = Random.insideUnitSphere * ShakeStrength;
            yield return null;
        }
        ShakeOffset = Vector3.zero;
    }

}
