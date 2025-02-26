using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
        DisplayLevel(1); // Exemple de chargement du niveau 1
    }

    void LoadLevels()
    {
        string filePath = "Assets/Resources/levels.json"; // Assurez-vous que le fichier JSON se trouve dans le dossier Resources
        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);
            levelList = JsonUtility.FromJson<LevelList>(jsonContent);
            Debug.Log("Niveaux chargés avec succès.");
        }
        else
        {
            Debug.LogError("Fichier JSON introuvable !");
        }
    }

    public void DisplayLevel(int levelId)
    {
        LevelData level = levelList.levels.Find(l => l.id == levelId);

        if (level != null)
        {
            Debug.Log($"Niveau: {level.name}");
            Debug.Log($"Description: {level.description}");

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

    void ProcessEvent(EventData levelEvent)
    {
        switch (levelEvent.event_type)
        {
            case "message":
                Debug.Log($"Message: {levelEvent.message}");
                break;

            case "item_acquisition":
                Debug.Log($"L'objet {levelEvent.item} a été acquis !");
                // Vous pouvez ajouter du code pour faire apparaître l'objet dans le jeu
                break;

            case "enemy_spawn":
                Debug.Log($"Des ennemis {levelEvent.enemy} apparaissent ! Quantité: {levelEvent.quantity}");
                // Vous pouvez ajouter du code pour faire apparaître des ennemis dans le jeu
                break;

            case "special_action":
                Debug.Log($"Action spéciale: {levelEvent.action}");
                // Vous pouvez ajouter du code pour effectuer l'action spéciale
                break;

            case "environment_change":
                Debug.Log($"Changement d'environnement: {levelEvent.condition} - Effet: {levelEvent.effect}");
                // Vous pouvez ajouter du code pour gérer les changements d'environnement (comme le sol glissant)
                break;

            case "event_trigger":
                Debug.Log($"Déclenchement d'événement: {levelEvent.trigger} - Effet: {levelEvent.effect}");
                // Vous pouvez ajouter du code pour déclencher des événements spécifiques dans le jeu
                break;

            default:
                Debug.LogWarning($"Événement inconnu: {levelEvent.event_type}");
                break;
        }
    }
}

