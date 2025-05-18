using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviourPunCallbacks 
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;
    private void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom) SpawnPlayer();
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Salle rejointe, instanciation du joueur...");
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
        {
            Debug.LogError("Tentative d'instanciation sans être dans une salle!");
            return;
        }
        Transform spawnPoint = GetRandomSpawnPoint();
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity);
        Debug.Log("Joueur instancié : " + player.GetPhotonView().ViewID);
    }

    private Transform GetRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return transform;
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }
}