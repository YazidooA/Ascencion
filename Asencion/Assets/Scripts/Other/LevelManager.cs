using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelManager : MonoBehaviour
    {
        private void Start()
        {
            LevelMaker levelManager = FindFirstObjectByType<LevelMaker>();
            Debug.Log("It is good, you moron?");
            if (levelManager is null)
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
                Debug.Log("Trouver");
                if (int.TryParse(numberPart, out int levelId)) return levelId;
            }
            Debug.Log("PAS TROUVER");
            return -1; // Erreur
        }
    }


