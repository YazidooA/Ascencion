using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Block
{
    [SerializeField]
    private string name;
    [SerializeField]
    private Vector3 position;
    [SerializeField]
    private Vector3 scale;
    [SerializeField]
    private bool repeat;

    public string Name
    {
        get => name;
        set => name = value;
    }

    public Vector3 Position
    {
        get => position;
        set => position = value;
    }

    public Vector3 Scale
    {
        get => scale;
        set => scale = value;
    }

    public bool Repeat
    {
        get => repeat;
        set => repeat = value;
    }
}

[System.Serializable]
public class LevelData
{
    [SerializeField]
    private int id;
    [SerializeField]
    private string sceneName;
    [SerializeField]
    private Block[] blocks;

    public int Id
    {
        get => id;
        set => id = value;
    }

    public string SceneName
    {
        get => sceneName;
        set => sceneName = value;
    }

    public Block[] Blocks
    {
        get => blocks;
        set => blocks = value;
    }
}

[System.Serializable]
public class LevelList
{
    [SerializeField]
    private LevelData[] levels;

    public LevelData[] Levels
    {
        get => levels;
        set => levels = value;
    }
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

            if (levelList == null)
            {
                Debug.LogError("❌ Le chargement du JSON a échoué.");
                return;
            }

            Debug.Log($"✅ {levelList.Levels.Length} niveaux chargés !");
            foreach (var level in levelList.Levels)
            {
                Debug.Log($"Niveau ID : {level.Id}, Nom de la scène : {level.SceneName}");
            }
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
            Debug.LogError("❌ Niveau non trouvé !");
            return;
        }

        Debug.Log($"🔄 Chargement de la scène : {level.SceneName}...");
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(level.SceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"✅ Scène {scene.name} chargée !");
        int levelId = GetLevelIdFromSceneName(scene.name);
        LevelData level = FindLevelById(levelId);

        if (level != null)
        {
            Debug.Log($"🛠 Création des objets pour {scene.name}...");
            CreateBlocks(Repeat0(level.Blocks));
        }

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
        Debug.Log($"🔍 Recherche du niveau avec ID : {levelId}");
        foreach (var level in levelList.Levels)
        {
            Debug.Log($"Vérification du niveau avec ID : {level.Id}");
            if (level.Id == levelId)
            {
                Debug.Log($"✅ Niveau trouvé : {level.SceneName}");
                return level;
            }
        }
        Debug.LogWarning($"⚠️ Aucun niveau trouvé avec l'ID : {levelId}");
        return null;
    }

   private Block[] Repeat0(Block[] blocks)
        {
            List<Block> res = new List<Block>();

            foreach (var elt in blocks)
            {
                if (elt.Repeat)
                {
                    float positionX = elt.Position.x;
                    for (float i = 1f; i <= 20f; i++)
                    {
                        Block no = new Block();
                        no.Name=elt.Name;
                        no.Position=new Vector3(positionX + i, elt.Position.y, elt.Position.z);
                        no.Scale=elt.Scale;
                        res.Add(no); 
                    }
                }
                res.Add(elt); 
            }
            Debug.Log($"✅ {res.Count} blocs générés !");
            return res.ToArray();
        }

    void CreateBlocks(Block[] blocks)
    {
        Dictionary<GameObject, Texture2D> blockTextures = new Dictionary<GameObject, Texture2D>();

        foreach (var block in blocks)
        {
            GameObject blockObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            blockObject.transform.position = block.Position;
            blockObject.transform.localScale = block.Scale;

            // ✅ Ajoute un Collider
            if (blockObject.GetComponent<BoxCollider>() == null)
            {
                blockObject.AddComponent<BoxCollider>();
            }

            // ✅ Ajoute un Rigidbody (collision détectable mais pas de chute)
            Rigidbody rb = blockObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;

            // ✅ Charge la texture
            string textureName = block.Name.Replace(".jpeg", "").Replace(".jpg", "").Replace(".png", "");
            Texture2D texture = Resources.Load<Texture2D>("Textures/" + textureName);

            if (texture != null)
            {
                Renderer renderer = blockObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    // ✅ Appliquer un shader URP pour éviter le rose
                    Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));

                    if (material != null)
                    {
                        material.mainTexture = texture;
                        material.SetFloat("_Smoothness", 1f);
                        material.SetFloat("_Metallic", 1f);
                        renderer.material = material;

                        // ✅ Stocke l'objet et sa texture
                        blockTextures[blockObject] = texture;

                        Debug.Log($"✅ Texture {texture.name} appliquée sur {blockObject.name}");

                        // ✅ Tourner la texture de 180 degrés sur l'axe Y
                        blockObject.transform.Rotate(0, 180, 0);
                    }
                }
            }
            else
            {
                Debug.LogWarning($"⚠️ Texture introuvable : {textureName}");
            }
        }

        Debug.Log($"🎨 {blockTextures.Count} blocs créés avec des textures.");
    }
}
