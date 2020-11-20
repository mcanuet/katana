using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Enums;

public class Launcher : MonoBehaviourPunCallbacks
{
    [Header("Fields")]
    [SerializeField]
    private TMP_Text LogText;
    [SerializeField]
    private InputField NameField;

    [Header("Screens")]
    [SerializeField]
    private GameObject ConnectionScreen;
    [SerializeField]
    private GameObject LobbyScreen;

    private string GameVersion = "1.0";
    private const string PlayerNameKey = "PlayerName";
    private string PlayerName = string.Empty;

    #region Unity Function

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NetworkingClient.EnableLobbyStatistics = true;
    }

    void Start()
    {
        if (PlayerPrefs.HasKey(PlayerNameKey))
        {
            PlayerName = PlayerPrefs.GetString(PlayerNameKey);
            NameField.text = PlayerName;
        };
    }

    #endregion

    #region public Methode

    public void Connect()
    {
        if (NameField.text == string.Empty)
        {
            LogOnScreen("Name is empty", LogLevel.Error);
            return;
        }

        PlayerPrefs.SetString(PlayerNameKey, NameField.text);
        PhotonNetwork.NickName = NameField.text;

        LogOnScreen("Trying to connect to photon", LogLevel.Log);
        PhotonNetwork.ConnectUsingSettings();
        LogOnScreen("Game Version is: " + GameVersion.ToString(), LogLevel.Log);
        PhotonNetwork.GameVersion = GameVersion;

        ConnectionScreen.SetActive(false);
    }
    public void LogOnScreen(string Log, LogLevel Level)
    {
        string color = "black";
        if (Level == LogLevel.Warning)
        {
            color = "yellow";
            Debug.LogWarning(Log);
        }
        else if (Level == LogLevel.Error)
        {
            color = "red";
            Debug.LogError(Log);
        }
        else
        {
            Debug.Log(Log);
        }

        LogText.text += "<color=" + color + ">" + Log + "</color>\n";
    }

    #endregion

    #region Private Methode


    #endregion

    #region MonoBehaviourPunCallbacks Callbacks

    public override void OnConnectedToMaster()
    {
        LogOnScreen("Connect to the master server", LogLevel.Log);
        LogOnScreen("Region of the master Server: " + PhotonNetwork.CloudRegion, LogLevel.Log);
        PhotonNetwork.JoinLobby();
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        LogOnScreen("Disconet from master reason: " + cause.ToString() , LogLevel.Warning);
    }

    public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
        Debug.LogError(lobbyStatistics.Count);
        //base.OnLobbyStatisticsUpdate(lobbyStatistics);
        foreach (TypedLobbyInfo LI in lobbyStatistics)
        {
            Debug.Log(LI.ToString());
        }
    }

    public override void OnJoinedLobby()
    {
        LogOnScreen("Connected to lobby", LogLevel.Log);
        LobbyScreen.SetActive(true);
    }

    #endregion
}
