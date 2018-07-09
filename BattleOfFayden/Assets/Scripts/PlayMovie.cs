using UnityEngine;
using UnityEngine.UI;

public class PlayMovie : MonoBehaviour
{
    public MovieTexture movie;
    int vSyncPrevious;
    
    RawImage image;
    
	void Start ()
    {
        vSyncPrevious = QualitySettings.vSyncCount;
        QualitySettings.vSyncCount = 0;

        image = GetComponent<RawImage>();
        movie.loop = true;
        PlayClip();
	}

    void PlayClip()
    {
        image.texture = movie;
        movie.Play();
    }

    void Update()
    {
        if(!movie.isPlaying)
        {
            QualitySettings.vSyncCount = vSyncPrevious;
        }
    }
}
