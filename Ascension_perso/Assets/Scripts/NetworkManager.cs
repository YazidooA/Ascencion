using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private string gameVersion = "1.0";
    [SerializeField] private string roomName = "Room1";
    [SerializeField] private int maxPlayers = 4;

    private void Start()
    {
        // Cette ligne permet d'éviter certains problèmes de déconnexion
        PhotonNetwork.AutomaticallySyncScene = true;
        
        // Se connecter au serveur Photon
        ConnectToPhoton();
    }

    private void ConnectToPhoton()
    {
        // Vérifier si on est déjà connecté
        if (PhotonNetwork.IsConnected)
            return;

        Debug.Log("Connexion au serveur Photon...");
        // Se connecter avec la version du jeu
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = gameVersion;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connecté au serveur Photon !");
        JoinRoom();
    }

    private void JoinRoom()
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = (byte)maxPlayers,
            IsVisible = true,
            IsOpen = true
        };

        // Essayer de rejoindre la salle, sinon la créer
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Salle rejointe : " + PhotonNetwork.CurrentRoom.Name);
        
        // Si on est le premier joueur (hôte), charger la scène de jeu
        if (PhotonNetwork.IsMasterClient)
        {
            // Chargez votre scène de jeu ici
            // PhotonNetwork.LoadLevel("GameScene");
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning("Déconnecté du serveur Photon : " + cause);
        // Reconnexion automatique
        ConnectToPhoton();
    }
}