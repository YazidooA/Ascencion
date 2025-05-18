using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button joinRoomButton;
    [SerializeField] private Transform roomListContent;
    [SerializeField] private GameObject roomItemPrefab;
    [SerializeField] private Transform playerListContent;
    [SerializeField] private GameObject playerItemPrefab;
    [SerializeField] private Button startGameButton;

    private void Start()
    {
        // Désactiver le bouton de démarrage jusqu'à ce qu'on soit le MasterClient
        startGameButton.gameObject.SetActive(false);
        
        createRoomButton.onClick.AddListener(CreateRoom);
        joinRoomButton.onClick.AddListener(JoinRoom);
        startGameButton.onClick.AddListener(StartGame);
        
        // Se connecter au serveur Photon s'il n'est pas déjà connecté
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connecté au serveur Photon");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Lobby rejoint");
        // Définir un nom de joueur aléatoire si vide
        if (string.IsNullOrEmpty(PhotonNetwork.NickName))
        {
            PhotonNetwork.NickName = "Joueur " + Random.Range(1000, 9999);
            playerNameInput.text = PhotonNetwork.NickName;
        }
    }

    private void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInput.text))
            return;
            
        // Définir le nom du joueur
        if (!string.IsNullOrEmpty(playerNameInput.text))
        {
            PhotonNetwork.NickName = playerNameInput.text;
        }
        
        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 4,
            IsVisible = true,
            IsOpen = true
        };
        
        PhotonNetwork.CreateRoom(roomNameInput.text, options);
    }
    
    private void JoinRoom()
    {
        if (string.IsNullOrEmpty(roomNameInput.text))
            return;
            
        // Définir le nom du joueur
        if (!string.IsNullOrEmpty(playerNameInput.text))
        {
            PhotonNetwork.NickName = playerNameInput.text;
        }
        
        PhotonNetwork.JoinRoom(roomNameInput.text);
    }
    
    private void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Charger la scène de jeu (assurez-vous qu'elle est dans les Build Settings)
            PhotonNetwork.LoadLevel("GameScene");
        }
    }
    
    public override void OnJoinedRoom()
    {
        Debug.Log("Salle rejointe");
        
        // Nettoyer la liste des joueurs
        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }
        
        // Ajouter tous les joueurs à la liste
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            AddPlayerItem(player);
        }
        
        // Activer le bouton de démarrage uniquement pour le MasterClient
        startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }
    
    public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        AddPlayerItem(newPlayer);
    }
    
    public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        UpdatePlayerList();
    }
    
    public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }
    
    private void AddPlayerItem(Photon.Realtime.Player player)
    {
        GameObject playerItem = Instantiate(playerItemPrefab, playerListContent);
        playerItem.transform.Find("PlayerNameText").GetComponent<TextMeshProUGUI>().text = player.NickName;
        if (player.IsMasterClient) playerItem.transform.Find("MasterIcon").gameObject.SetActive(true);
        
    }
    
    private void UpdatePlayerList()
    {
        foreach (Transform child in playerListContent) Destroy(child.gameObject);
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList) AddPlayerItem(player);
        
    }
}