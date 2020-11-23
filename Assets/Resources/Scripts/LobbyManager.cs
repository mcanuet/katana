using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Screens")]
    [SerializeField]
    private GameObject ChoosingRoom;
    [SerializeField]
    private GameObject WaitingRoom;

    [Header("Choosing Room Components")]
    [SerializeField]
    private GameObject RoomList;
    [SerializeField]
    private InputField RoomNameInput;
    [SerializeField]
    private Dropdown RoomPlayer;

    [Header("Waiting Room Components")]
    [SerializeField]
    private Text NbPlayerText;
    [SerializeField]
    private Text PlayerList;

    private List<GameObject> InstantiateButton = new List<GameObject>();
    private Launcher launcher;

    void Awake()
    {
        launcher = FindObjectOfType<Launcher>();
    }

    #region Public Methodes

    public void ConnectToRoom(string RoomName)
    {
        launcher.LogOnScreen("Connecting to room " + RoomName, Enums.LogLevel.Log);
        PhotonNetwork.JoinRoom(RoomName);
    }

    public void CreateRoom()
    {
        if (RoomNameInput.GetComponentInChildren<Text>().text == string.Empty)
        {
           launcher.LogOnScreen("Room Name is empty", Enums.LogLevel.Warning);
            return;
        }
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = byte.Parse(RoomPlayer.options[RoomPlayer.value].text);
        PhotonNetwork.CreateRoom(RoomNameInput.text, roomOptions);
    }

    public void QuitCurrentRoom()
    {
        launcher.LogOnScreen("Leaving the room", Enums.LogLevel.Log);
        PhotonNetwork.LeaveRoom(false);
        WaitingRoom.SetActive(false);
        ChoosingRoom.SetActive(true);
    }

    #endregion

    #region Photon Methodes
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
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

    public override void OnCreatedRoom()
    {
        Debug.Log("Create the Room: " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError(returnCode.ToString() + " : " + message);
    }

    public override void OnJoinedRoom()
    {
       launcher.LogOnScreen("Join room: " + PhotonNetwork.CurrentRoom.Name, Enums.LogLevel.Log);

        ChoosingRoom.SetActive(false);
        WaitingRoom.SetActive(true);
        ChangeWaitingRoomDisplay(PhotonNetwork.LocalPlayer);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError(returnCode.ToString() + " : " + message);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        ChangeWaitingRoomDisplay(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ChangeWaitingRoomDisplay(otherPlayer, true);
    }

    #endregion

    #region Private Methodes

    private void ChangeWaitingRoomDisplay(Player concernedPlayer, bool isLeaving = false)
    {
        if (!isLeaving)
            launcher.LogOnScreen(concernedPlayer.NickName + " join the room", Enums.LogLevel.Log);
        else
            launcher.LogOnScreen(concernedPlayer.NickName + " leave the room", Enums.LogLevel.Log);

        string players = string.Empty;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            players += player.NickName + "\n";
        }
        PlayerList.text = players;

        string maxPlayerTxt = PhotonNetwork.CurrentRoom.PlayerCount.ToString() + "/" + PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
        maxPlayerTxt += " player in the room";
        NbPlayerText.text = maxPlayerTxt;
    }

    #endregion
}
