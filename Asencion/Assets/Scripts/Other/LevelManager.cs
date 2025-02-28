using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Nécessaire pour charger les scènes

[System.Serializable]
public class EventData
{
    public string event_type;
    public string message;
    public string item;
    public string action;
    public string trigger;
    public string condition;
    public string effect;
    public string enemy;
    public int quantity;
}

[System.Serializable]
public class LevelData
{
    public int id;
    public string name;
    public string description;
    public List<EventData> events;
    public string sceneName; // Nom de la scène à charger
}

[System.Serializable]
public class LevelList
{
    public List<LevelData> levels;
}

public class LevelManager : MonoBehaviour
{
    private LevelList levelList;

    void Start()
    {
        LoadLevels();
        CreateSceneForLevel(1); // Charger le niveau 1 par défaut
    }

    void LoadLevels()
    {
        string filePath = "Assets/Resources/levels.json"; // Assurez-vous que le fichier JSON est dans Resources
        if (System.IO.File.Exists(filePath))
        {
            string jsonContent = System.IO.File.ReadAllText(filePath);
            levelList = JsonUtility.FromJson<LevelList>(jsonContent);
            Debug.Log("Niveaux chargés avec succès.");
        }
        else
        {
            Debug.LogError("Fichier JSON introuvable !");
        }
    }

    public void CreateSceneForLevel(int levelId)
    {
        LevelData level = levelList.levels.Find(l => l.id == levelId);

        if (level != null)
        {
            Debug.Log($"Chargement de la scène pour le niveau: {level.name}");
            Debug.Log($"Description: {level.description}");

            // Charger la scène correspondante à ce niveau
            LoadScene(level.sceneName);

            // Traiter les événements associés au niveau
            foreach (var levelEvent in level.events)
            {
                ProcessEvent(levelEvent);
            }
        }
        else
        {
            Debug.LogWarning($"Le niveau {levelId} n'existe pas !");
        }
    }

    void LoadScene(string sceneName)
    {
        try
        {
            SceneManager.LoadScene(sceneName);
        }
        catch(Exception)
        {
            Debug.LogError($"La scène {sceneName} n'a pas été trouvée.");
        }
    }

    void ProcessEvent(EventData levelEvent)
    {
        switch (levelEvent.event_type)
        {
            case "message":
                Debug.Log($"Message: {levelEvent.message}");
                break;
            case "item_acquisition":
                Debug.Log($"L'objet {levelEvent.item} a été acquis !");
                break;
            case "enemy_spawn":
                Debug.Log($"Des ennemis {levelEvent.enemy} apparaissent ! Quantité: {levelEvent.quantity}");
                break;
            default:
                Debug.LogWarning($"Événement inconnu: {levelEvent.event_type}");
                break;
        }
    }
}
