using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

    [System.Serializable]
    public class Block
    {
        public string name;
        public Vector3 position;
        public Vector3 scale;
    }

    [System.Serializable]
    public class LevelData
    {
        public int id;
        public string sceneName;
        public Block[] blocks;
    }

    [System.Serializable]
    public class LevelList
    {
        public LevelData[] levels;
    }

public class LevelMaker : MonoBehaviour
{
    private LevelList levelList;
    public string jsonFileName = "levels.json";

    void Start()
    {
        LoadLevels();
    }

    void LoadLevels()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);
            levelList = JsonUtility.FromJson<LevelList>(jsonContent);
            Debug.Log($"✅ {levelList.levels.Length} niveaux chargés !");
        }
        else
        {
            Debug.LogError("❌ Fichier JSON non trouvé : " + filePath);
        }
    }

    public void LoadScene(int levelId)
    {
        LevelData level = FindLevelById(levelId);
        if (level == null)
        {
            Debug.LogError("Niveau non trouvé !");
            return;
        }

        Debug.Log($"🔄 Chargement de la scène : {level.sceneName}...");
        
        // Ajout d'un écouteur pour attendre la fin du chargement de la scène
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Charger la scène
        SceneManager.LoadScene(level.sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"✅ Scène {scene.name} chargée !");

        // Récupérer l'ID du niveau
        int levelId = GetLevelIdFromSceneName(scene.name);
        LevelData level = FindLevelById(levelId);

        if (level != null)
        {
            Debug.Log($"🛠 Création des objets pour {scene.name}...");
            CreateBlocks(level.blocks);
        }

        // Se désabonner pour éviter d'appeler plusieurs fois cette fonction
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private int GetLevelIdFromSceneName(string sceneName)
    {
        if (sceneName.StartsWith("Level"))
        {
            string numberPart = sceneName.Substring(5);
            if (int.TryParse(numberPart, out int levelId))
            {
                return levelId;
            }
        }
        return -1;
    }

    LevelData FindLevelById(int levelId)
    {
        foreach (var level in levelList.levels)
        {
            if (level.id == levelId)
                return level;
        }
        return null;
    }

    void CreateBlocks(Block[] blocks)
    {
        foreach (var block in blocks)
        {
            GameObject blockObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            blockObject.transform.position = block.position;
            blockObject.transform.localScale = block.scale;

            string textureName = "Textures/" + block.name.Replace(".jpeg", "").Replace(".png", "");
            Texture2D texture = Resources.Load<Texture2D>(textureName);

            if (texture != null)
            {
                Renderer renderer = blockObject.GetComponent<Renderer>();
                renderer.material.mainTexture = texture;
            }
            else
            {
                Debug.LogWarning($"⚠️ Texture {block.name} dans {textureName} introuvable !");
            }
        }
    }
}
