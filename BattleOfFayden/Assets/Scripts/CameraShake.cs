using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float duration;
    public float magnitude;
    public Camera camera;
   
    Vector3 originalCamPos;

    void Start ()
    {
        camera = GetComponent<Camera>();
        originalCamPos = camera.transform.localPosition;
    }
	
	void Update ()
    {
		if(Input.GetKeyDown("f"))
        {
            StartCoroutine(Shake());
        }
	}

    IEnumerator Shake()
    {
        float elapsed = 0.0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float percentComplete = elapsed / duration;
            
            float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);
            float x = originalCamPos.x + ((Random.value * 2.0f - 1.0f) * damper * magnitude);
            float y = originalCamPos.y + ((Random.value * 2.0f - 1.0f) * damper * magnitude);

            camera.transform.localPosition = new Vector3(x, y, originalCamPos.z);

            yield return null;
        }

        camera.transform.position = originalCamPos;
    }
}
