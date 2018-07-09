using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class RoomSelectUI : MonoBehaviour {

    public GameObject roomEntry;
    public Transform content;
    public GameObject roomNameText;

    private List<GameObject> entryList;

    public delegate void PlayerDisconnected(PhotonPlayer player);
    public static PlayerDisconnected onPlayerDisconnected;

    public void Awake()
    {
        entryList = new List<GameObject>();
        DontDestroyOnLoad(this);
        GameObject.Find("nameinput").GetComponent<TMP_InputField>().text = PlayerPrefs.GetString("PlayerName");

        SceneManager.LoadScene((int)SceneAlias.OptionsMenu, LoadSceneMode.Additive);
    }

    public GameObject getChildGameObject(GameObject fromGameObject, string withName)
    {
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
        return null;
    }

    void Start()
    {
        entryList = new List<GameObject>();
        Debug.Log("Connecting to cloud server...");
        PhotonNetwork.ConnectUsingSettings("3");
    }

    public void OnCreatePressed()
    {
        string roomName = roomNameText.GetComponent<TextMeshProUGUI>().text;
        PhotonNetwork.CreateRoom(roomName);
    }

    public void OnRoomJoinPressed()
    {
        GameObject clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        GameObject RoomNameObject = getChildGameObject(clickedButton, "RoomName");
        string roomName = RoomNameObject.GetComponent<TextMeshProUGUI>().text;
        PhotonNetwork.JoinRoom(roomName);
    }

    void OnJoinedLobby()
    {
        Debug.Log("Joined lobby.");
        RefreshRoomList();
        StartCoroutine(waitThenRefresh());
    }

    IEnumerator waitThenRefresh()
    {
        yield return new WaitForSeconds(1f);
        RefreshRoomList();
    }

    public void RefreshRoomList()
    {
        foreach (GameObject entry in entryList)
        {
            Destroy(entry);
        }
        entryList = new List<GameObject>();
        RoomInfo[] roomList = PhotonNetwork.GetRoomList();
        foreach (RoomInfo room in roomList)
        {
            GameObject currentEntry = Instantiate(roomEntry, content);
            entryList.Add(currentEntry);
            GameObject roomNameText = getChildGameObject(currentEntry, "RoomName");
            roomNameText.GetComponent<TextMeshProUGUI>().text = room.Name;

            GameObject playerCountText = getChildGameObject(currentEntry, "PlayerCount");
            playerCountText.GetComponent<TextMeshProUGUI>().text = room.PlayerCount + "/" + room.MaxPlayers;
        }
    }

    void ConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    void OnJoinedRoom()
    {
        Debug.Log("Joined room.");
        SceneManager.LoadScene((int)SceneAlias.ManagerScene);
        if (PhotonNetwork.isMasterClient)
            PhotonNetwork.room.MaxPlayers = 4;
    }

    void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        PhotonNetwork.CloseConnection(PhotonNetwork.player);
        Debug.Log("Player disconnected: " + player.NickName);
    }

    public void OnNameChanged()
    {
        var playername = GameObject.Find("nameinput").GetComponent<TMP_InputField>().text;
        PlayerPrefs.SetString("PlayerName", playername);
    }

    public void OnCreditsPressed()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene((int)SceneAlias.Credits);
    }
}
