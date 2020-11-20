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
    [SerializeField]
    private InputField RoomNameInput;
    [SerializeField]
    private Dropdown RoomPlayer;

    private List<GameObject> InstantiateButton = new List<GameObject>();

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
                GameObject roomBtn = Instantiate<GameObject>(Resources.Load<GameObject>("UI/RoomButton"));
                roomBtn.transform.SetParent(RoomList.transform);
                roomBtn.GetComponentInChildren<Text>().text = room.Name;
                roomBtn.GetComponentInChildren<Button>().onClick.AddListener(delegate { ConnectToRoom(room.Name); });
                InstantiateButton.Add(roomBtn);
            }
        }
    }

    public void ConnectToRoom(string RoomName)
    {
        FindObjectOfType<Launcher>().LogOnScreen("Connecting to room " + RoomName, Enums.LogLevel.Log);
    }

    public void CreateRoom()
    {
        if (RoomNameInput.GetComponentInChildren<Text>().text == string.Empty)
        {
            FindObjectOfType<Launcher>().LogOnScreen("Room Name is empty", Enums.LogLevel.Warning);
            return;
        }
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = byte.Parse(RoomPlayer.options[RoomPlayer.value].text);
        Debug.Log("Room Size: " + roomOptions.MaxPlayers.ToString());
        PhotonNetwork.CreateRoom(RoomNameInput.GetComponentInChildren<Text>().text, roomOptions);
    }
}
