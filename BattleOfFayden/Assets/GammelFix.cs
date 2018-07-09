using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GammelFix : MonoBehaviour {
    
	void Start ()
    {
        DontDestroyOnLoad(this.gameObject);
	}
	
    private void OnLevelWasLoaded(int level)
    {
        if (level == 1)
        {
            Destroy(this.gameObject);
        }
    }
}
