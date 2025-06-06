using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Joueur")]
    public GameObject playerPrefab;
    public Transform[] spawnPoints;
    
    [Header("UI")]
    public GameObject gameCanvas;
    public GameObject scoreboardPanel;
    public GameObject disconnectPanel;
    
    private bool isScoreboardActive = false;
    
    void Start()
    {
        // Vérifier si nous sommes connectés à Photon
        if (PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }
        else
        {
            // Si nous ne sommes pas connectés, retourner au menu principal
            Debug.LogError("Non connecté à Photon. Retour au menu principal.");
            PhotonNetwork.LoadLevel(0); // 0 étant l'index de votre scène de menu
        }
    }
    
    void Update()
    {
        // Afficher/masquer le tableau de scores
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isScoreboardActive = !isScoreboardActive;
            scoreboardPanel.SetActive(isScoreboardActive);
        }
        
        // Quitter la partie
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            disconnectPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    
    void SpawnPlayer()
    {
        // Choisir un point d'apparition aléatoire
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        
        // Instancier le joueur sur le réseau
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
        
        Debug.Log("Joueur instancié: " + PhotonNetwork.NickName);
    }
    
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }

}