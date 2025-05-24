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
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        SetupButtons();
        audioSource = FindObjectOfType<AudioSource>();
        if (volumeSlider != null && audioSource != null)
        {
            volumeSlider.value = audioSource.volume;
            volumeSlider.onValueChanged.AddListener(ChangeVolume);
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }
    
    void SetupButtons()
    {
        if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);
        if (settingsButton != null) settingsButton.onClick.AddListener(OpenSettings);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(LoadMainMenu);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);
        if (backButton != null) backButton.onClick.AddListener(CloseSettings);
    }
    
    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; 
        isPaused = true;
        DisablePlayerControls();
    }
    
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; 
        isPaused = false;
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
        Time.timeScale = 1f; 
        SceneManager.LoadScene("Menu"); 
    }
    
    public void QuitGame()
    {
        Debug.Log("Quitter le jeu...");
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    
    public void ChangeVolume(float volume)
    {
        if (audioSource != null) audioSource.volume = volume;
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }
    
    void DisablePlayerControls()
    {
        Player_movements playerController = FindObjectOfType<Player_movements>();
        if (playerController != null) playerController.enabled = false;
        MonoBehaviour[] playerScripts = FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour script in playerScripts)
            if (script.CompareTag("Player"))
                script.enabled = false;
    }
    
    void EnablePlayerControls()
    {
        Player_movements playerController = FindObjectOfType<Player_movements>();
        if (playerController != null) playerController.enabled = true;
        MonoBehaviour[] playerScripts = FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour script in playerScripts)
            if (script.CompareTag("Player"))
                script.enabled = true;
            
        
    }
    public bool IsPaused
    {
        get { return isPaused; }
    }
    
    void OnDestroy()
    {
        if (volumeSlider != null) volumeSlider.onValueChanged.RemoveAllListeners();
    }
}