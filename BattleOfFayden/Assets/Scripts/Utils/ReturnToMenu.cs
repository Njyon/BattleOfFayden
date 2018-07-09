using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenu : MonoBehaviour
{
    public GameObject koth;
    public GameObject networkConnector;
    public GameObject gM;
    public GameObject inputM;

    private void Start()
    {
        koth = GameObject.Find("(singleton) KingOfTheHill");
        networkConnector = GameObject.Find("NetworkManager");
        gM = GameObject.Find("GameManager");
        inputM = GameObject.Find("InputManager");
    }
    public void GammelFix()
    {
        Destroy(koth);
        Destroy(networkConnector);
        Destroy(gM);
        Destroy(inputM);

        PhotonNetwork.Disconnect();

        SceneManager.LoadScene((int)SceneAlias.Lobby);
    }


    public void OnReturnToMenu()
    {
        var photonView = KingOfTheHill.Instance.GetComponent<PhotonView>();
        if (photonView != null)
            PhotonNetwork.Destroy(photonView);
        Destroy(KingOfTheHill.Instance);

        photonView = GameManager.Instance.GetComponent<PhotonView>();
        if (photonView != null)
            PhotonNetwork.Destroy(photonView);
        Destroy(GameManager.Instance);

        Destroy(InputManager.Instance);

        var go = FindObjectOfType<RoomSelectUI>().gameObject;
        if (go != null)
        {
            PhotonNetwork.Disconnect();
            Destroy(go);
        }

        SceneManager.LoadScene((int)SceneAlias.Lobby);
    }
}
