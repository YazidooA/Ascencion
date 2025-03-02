using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private void Start()
    {
        LevelMaker levelManager = FindFirstObjectByType<LevelMaker>();

        if (levelManager == null)
        {
            Debug.LogError("⚠️ Aucun LevelManager trouvé dans la scène !");
            return;
        }

        // Récupérer le nom de la scène actuelle
        string sceneName = SceneManager.GetActiveScene().name;
        
        // Extraire le numéro du niveau depuis "LevelX"
        int levelId = GetLevelIdFromSceneName(sceneName);
        
        if (levelId > 0)
        {
            Debug.Log($"✅ Chargement des objets pour {sceneName} (ID: {levelId})...");
            
            // Utilisation de StartCoroutine pour appeler la méthode LoadSceneAndObjects
            levelManager.LoadScene(levelId);
        }
        else
        {
            Debug.LogWarning($"⚠️ Impossible de trouver un ID de niveau pour {sceneName}");
        }
    }

private int GetLevelIdFromSceneName(string sceneName)
{
    Debug.Log($"🛠 Extraction de l'ID à partir du nom de la scène : {sceneName}");
    if (sceneName.StartsWith("Level"))
    {
        string numberPart = sceneName.Substring(5); // Enlève "Level"
        if (int.TryParse(numberPart, out int levelId))
        {
            Debug.Log($"✅ ID extrait : {levelId}");
            return levelId;
        }
    }
    Debug.LogWarning("⚠️ Erreur lors de l'extraction de l'ID.");
    return -1; // Retourne -1 en cas d'erreur
}


}
