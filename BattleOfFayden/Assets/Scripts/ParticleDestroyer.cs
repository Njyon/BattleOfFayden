using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDestroyer : MonoBehaviour {

    [SerializeField]
    private float time;

	// Use this for initialization
	void Start ()
    {
        Destroy(gameObject, time);
	}
   
}
