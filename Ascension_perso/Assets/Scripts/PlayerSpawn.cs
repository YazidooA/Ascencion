using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        // Choisir un point de spawn aléatoire
        Transform spawnPoint = GetRandomSpawnPoint();
        
        // Instancier le joueur sur le réseau
        GameObject player = PhotonNetwork.Instantiate(
            playerPrefab.name, 
            spawnPoint.position, 
            Quaternion.identity
        );
        
        Debug.Log("Joueur instancié : " + player.GetPhotonView().ViewID);
    }

    private Transform GetRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            return transform;
            
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }
}