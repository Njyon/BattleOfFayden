using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CreditsPlay : MonoBehaviour
{
    public VideoPlayer player;

    private void Start()
    {
        Destroy(FindObjectOfType<GammelFix>().gameObject);
        Destroy(FindObjectOfType<RoomSelectUI>().gameObject);
        StartCoroutine(changeBack());
    }

    IEnumerator changeBack()
    {
        yield return new WaitForSeconds(21.5f);
        SceneManager.LoadScene((int)SceneAlias.Lobby);
    }
}
