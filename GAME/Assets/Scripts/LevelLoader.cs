using UnityEngine;

[System.Serializable]
public class LevelData
{
    public string levelName;
    public Vector2 playerStart;
    public Obstacle[] obstacles;
    public Enemy[] enemies;
    public Weapon[] weapons;
}

[System.Serializable]
public class Obstacle
{
    public string type;
    public Vector2 position;
}

[System.Serializable]
public class Enemy
{
    public string type;
    public Vector2 position;
    public string behavior;
}

[System.Serializable]
public class Weapon
{
    public string type;
    public Vector2 position;
}

public class LevelLoader : MonoBehaviour
{
    void Start()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("niveau1"); // Pas besoin d'extension .json
        LevelData level = JsonUtility.FromJson<LevelData>(jsonFile.text);
        
        Debug.Log("Niveau chargé : " + level.levelName);
    }
}

