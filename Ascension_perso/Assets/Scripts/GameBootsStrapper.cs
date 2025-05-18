using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private string menuSceneName;
    [SerializeField] private string gameSceneName;
    
    private void Awake()
    {
        // S'assurer que ce GameObject persiste entre les scènes
        DontDestroyOnLoad(gameObject);
        
        // Configurer Photon pour qu'il ne se déconnecte pas en changeant de scène
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    
    private void Start()
    {
        // Charger la scène de menu
        SceneManager.LoadScene(menuSceneName);
    }
    
    // Méthode pour quitter la partie actuelle et revenir au menu
    public void LeaveGame()
    {
        // Quitter la salle Photon actuelle
        PhotonNetwork.LeaveRoom();
    }
    
    // Appelé quand on quitte la salle
    public void OnLeftRoom()
    {
        // Charger la scène de menu
        SceneManager.LoadScene(menuSceneName);
    }
}