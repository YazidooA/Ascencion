using UnityEngine;
using LevelMaker;


using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    private void Start()
    {
        LevelManager levelManager = FindFirstObjectByType<LevelManager>();
        if (levelManager == null)
        {
            Debug.LogError("⚠️ Aucun LevelManager trouvé dans la scène !");
            return;
        }
        string sceneName = SceneManager.GetActiveScene().name;
        int levelId = GetLevelIdFromSceneName(sceneName);
        if (levelId > 0)
        {
            Debug.Log($"✅ Chargement des objets pour {sceneName} (ID: {levelId})...");
            levelManager.StartCoroutine(levelManager.LoadSceneAndObjects(levelId));
        }
        else Debug.LogWarning($"⚠️ Impossible de trouver un ID de niveau pour {sceneName}");
    }

    private int GetLevelIdFromSceneName(string sceneName)
    {
        // Ex: Convertir "Level1" -> 1, "Level2" -> 2, etc.
        if (sceneName.StartsWith("Level"))
        {
            string numberPart = sceneName.Substring(5); // Enlève "Level"
            if (int.TryParse(numberPart, out int levelId))return levelId;
            
        }
        return -1; // Erreur
    }
}