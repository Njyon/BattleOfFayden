using UnityEngine;

public class NetworkManager : Photon.MonoBehaviour
{
    public delegate void PlayerDisconnected(PhotonPlayer player);
    public static PlayerDisconnected onPlayerDisconnected;

    public void Awake()
    {
        DontDestroyOnLoad(this);
    }
    
    void Start()
    {
        // EventSystem.onPlayerSpawn += this.PlayerSpawned;
        Debug.Log("Connecting to cloud server...");
        PhotonNetwork.ConnectUsingSettings("2");
    }
    
    void OnJoinedLobby()
    {
        Debug.Log("Joined lobby.");
        PhotonNetwork.JoinRandomRoom();
    }

    void ConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    
    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("Failed to connect to room.");
        PhotonNetwork.CreateRoom(null);
    }

    void OnJoinedRoom()
    {
        Debug.Log("Joined room.");
        if (PhotonNetwork.isMasterClient)
            PhotonNetwork.room.MaxPlayers = 4;
    }

    void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        PhotonNetwork.CloseConnection(PhotonNetwork.player);
        Debug.Log("Player disconnected: " + player.NickName);
    }
}
