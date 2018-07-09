using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomListEntry : MonoBehaviour {

	public void JoinRoom()
    {
        GameObject networkManager = GameObject.Find("NetworkManager");
        RoomSelectUI room = networkManager.GetComponent<RoomSelectUI>();
        room.OnRoomJoinPressed();
    }
}
