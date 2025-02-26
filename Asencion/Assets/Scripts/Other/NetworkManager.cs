using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        Debug.Log("Connexion à Photon...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connecté au serveur Photon !");
        PhotonNetwork.JoinLobby(); // Rejoint automatiquement un lobby
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Connecté au lobby !");
        PhotonNetwork.JoinOrCreateRoom("Salle1", new RoomOptions { MaxPlayers = 4 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joueur rejoint la salle !");
        Vector3 spawnPos = new Vector3(Random.Range(-5f, 5f), 0, 0);
        PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity);
    }
}
