using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Menu Pause UI")]
    public GameObject pauseMenuUI;
    public Button resumeButton;
    public Button settingsButton;
    public Button mainMenuButton;
    public Button quitButton;
    
    [Header("Settings Panel (Optionnel)")]
    public GameObject settingsPanel;
    public Slider volumeSlider;
    public Button backButton;
    
    private bool isPaused = false;
    private AudioSource audioSource;
    
    void Start()
    {
        // S'assurer que le menu est désactivé au début
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
            
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        
        // Configuration des boutons
        SetupButtons();
        
        // Récupérer l'AudioSource si présent
        audioSource = FindObjectOfType<AudioSource>();
        
        // Initialiser le volume slider
        if (volumeSlider != null && audioSource != null)
        {
            volumeSlider.value = audioSource.volume;
            volumeSlider.onValueChanged.AddListener(ChangeVolume);
        }
    }
    
    void Update()
    {
        // Détecter la touche Escape ou P pour pause
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }
    
    void SetupButtons()
    {
        // Configurer les événements des boutons
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
            
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);
            
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(LoadMainMenu);
            
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
            
        if (backButton != null)
            backButton.onClick.AddListener(CloseSettings);
    }
    
    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Arrêter le temps du jeu
        isPaused = true;
        
        // Désactiver les contrôles du joueur si nécessaire
        DisablePlayerControls();
    }
    
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Reprendre le temps normal
        isPaused = false;
        
        // Réactiver les contrôles du joueur
        EnablePlayerControls();
    }
    
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            pauseMenuUI.SetActive(false);
            settingsPanel.SetActive(true);
        }
    }
    
    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            pauseMenuUI.SetActive(true);
        }
    }
    
    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Remettre le temps normal avant de changer de scène
        SceneManager.LoadScene("Menu"); // Remplacez par le nom de votre scène de menu principal
    }
    
    public void QuitGame()
    {
        Debug.Log("Quitter le jeu...");
        Application.Quit();
        
        // Pour tester dans l'éditeur Unity
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    
    public void ChangeVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
        
        // Vous pouvez aussi sauvegarder cette valeur
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }
    
    void DisablePlayerControls()
    {
        // Désactiver le script de contrôle du joueur
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
            playerController.enabled = false;
            
        // Ou désactiver tous les scripts de mouvement
        MonoBehaviour[] playerScripts = FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour script in playerScripts)
        {
            if (script.CompareTag("Player"))
            {
                script.enabled = false;
            }
        }
    }
    
    void EnablePlayerControls()
    {
        // Réactiver le script de contrôle du joueur
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
            playerController.enabled = true;
            
        // Ou réactiver tous les scripts de mouvement
        MonoBehaviour[] playerScripts = FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour script in playerScripts)
        {
            if (script.CompareTag("Player"))
            {
                script.enabled = true;
            }
        }
    }
    
    // Propriété publique pour vérifier l'état de pause
    public bool IsPaused
    {
        get { return isPaused; }
    }
    
    void OnDestroy()
    {
        // Nettoyer les listeners
        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveAllListeners();
    }
}