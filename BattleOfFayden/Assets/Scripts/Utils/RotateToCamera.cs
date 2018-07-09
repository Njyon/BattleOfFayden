using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToCamera : MonoBehaviour
{
    void Update ()
    {
        Camera cam = Camera.main;
        this.transform.LookAt(cam.transform.position);
    }
}
