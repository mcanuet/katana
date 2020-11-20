using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject RoomList;

    private List<Button> InstantiateButton = new List<Button>();

    void Awake()
    {
        Debug.Log("I'm awake");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(RoomInfo room in roomList)
        {
            int index = InstantiateButton.FindIndex(x => x.GetComponentInChildren<Text>().text == room.Name);
            if (index != -1)
            {
                Destroy(InstantiateButton[index].gameObject);
                InstantiateButton.RemoveAt(index);
            }
            else
            {
                Button btn = new UnityEngineButton();
                btn.GetComponentInChildren<Text>().text = room.Name;
            }
        }
    }

    public void ConnectToRoom(string RoomName)
    {
        FindObjectOfType<Launcher>().LogOnScreen("Connecting to room " + RoomName, Enums.LogLevel.Log);
    }
}
