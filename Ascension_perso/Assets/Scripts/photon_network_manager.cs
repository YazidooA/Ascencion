using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Panneau de connexion")]
    public GameObject loginPanel;
    public TMP_InputField playerNameInput;
    public Button connectButton;

    [Header("Panneau de salon")]
    public GameObject lobbyPanel;
    public TMP_InputField roomNameInput;
    public Button createRoomButton;
    public GameObject roomListContent;
    public GameObject roomListItemPrefab;

    [Header("Panneau de jeu")]
    public GameObject roomPanel;
    public TextMeshProUGUI roomNameText;
    public GameObject playerListContent;
    public GameObject playerListItemPrefab;
    public Button startGameButton;

    [Header("Game")]
    public GameObject playerPrefab;
    public Transform spawnPoint;

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    void Start()
    {
        // Désactiver les panneaux, activer le panneau de connexion
        loginPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(false);

        // S'assurer que nous sommes connectés à Photon Master Server
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.AutomaticallySyncScene = true;
        }
    }

    #region UI Callbacks

    public void OnLoginButtonClicked()
    {
        string playerName = playerNameInput.text;
        if (!string.IsNullOrEmpty(playerName))
        {
            PhotonNetwork.NickName = playerName;
            PhotonNetwork.JoinLobby();
            connectButton.interactable = false;
        }
        else
        {
            Debug.LogError("Le nom du joueur est invalide.");
        }
    }

    public void OnCreateRoomButtonClicked()
    {
        string roomName = roomNameInput.text;
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "Room " + Random.Range(1000, 10000);
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void OnJoinRoomButtonClicked(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnLeaveRoomButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnStartGameButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel("GameScene"); // Assurez-vous d'avoir une scène nommée "GameScene"
        }
    }

    #endregion
    

    #region Private Methods

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
            }
        }
    }

    private void UpdateRoomListView()
    {
        // Détruire les anciens éléments de la liste
        foreach (Transform child in roomListContent.transform)
        {
            Destroy(child.gameObject);
        }

        // Instancier les nouveaux éléments de la liste
        foreach (var roomInfo in cachedRoomList.Values)
        {
            if (roomInfo.IsOpen && roomInfo.IsVisible)
            {
                GameObject roomListItem = Instantiate(roomListItemPrefab, roomListContent.transform);
                roomListItem.transform.Find("RoomNameText").GetComponent<TextMeshProUGUI>().text = roomInfo.Name;
                roomListItem.transform.Find("PlayerCountText").GetComponent<TextMeshProUGUI>().text = roomInfo.PlayerCount + " / " + roomInfo.MaxPlayers;
                
                // Configurer le bouton pour rejoindre cette salle
                Button joinButton = roomListItem.transform.Find("JoinButton").GetComponent<Button>();
                string roomName = roomInfo.Name; // Créer une variable locale pour éviter les problèmes de closure
                joinButton.onClick.AddListener(() => OnJoinRoomButtonClicked(roomName));
            }
        }
    }

    private void UpdatePlayerListView()
    {
        // Détruire les anciens éléments de la liste
        foreach (Transform child in playerListContent.transform)
        {
            Destroy(child.gameObject);
        }

        // Instancier les nouveaux éléments de la liste
        foreach (var player in PhotonNetwork.PlayerList)
        {
            GameObject playerListItem = Instantiate(playerListItemPrefab, playerListContent.transform);
            TextMeshProUGUI playerNameText = playerListItem.GetComponent<TextMeshProUGUI>();
            
            if (player.IsMasterClient)
            {
                playerNameText.text = player.NickName + " (Host)";
            }
            else
            {
                playerNameText.text = player.NickName;
            }
        }
    }

    #endregion
}