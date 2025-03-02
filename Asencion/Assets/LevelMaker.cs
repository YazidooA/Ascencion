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
        void Start()
        {
            LoadLevels();
            StartCoroutine(LoadSceneAndObjects(1));  // Charge le niveau 1 au lancement
        }
        void LoadLevels()
        {
            string filePath = "/home/xyubot/Documents/Ascencion/Asencion/Assets/StreamingAssets/levels.json";
            if (File.Exists(filePath))
            {
                string jsonContent = File.ReadAllText(filePath);
                levelList = JsonUtility.FromJson<LevelList>(jsonContent);
            }
            else Debug.LogError("❌ Fichier JSON non trouvé : " + filePath);
            
        }
        public IEnumerator LoadSceneAndObjects(int levelId)
        {
            LevelData level = FindLevelById(levelId);
            if (level == null)
            {
                Debug.LogError("Niveau non trouvé !");
                yield break;
            }
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(level.sceneName);
            while (!asyncLoad.isDone) yield return null;
            Debug.Log($"Scène {level.sceneName} chargée !");
            CreateBlocks(level.blocks);
        }
        LevelData FindLevelById(int levelId)
        {
            foreach (var level in levelList.levels)if (level.id == levelId) return level;
            return null;
        }
        void CreateBlocks(Block[] blocks)
        {
            foreach (var block in blocks)
            {
                GameObject blockObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Debug.Log("La case a ete poser");
                blockObject.transform.position = block.position;
                blockObject.transform.localScale = block.scale;

                Texture2D texture = Resources.Load<Texture2D>(block.name.Replace(".jpeg", "").Replace(".png", ""));
                if (texture != null)
                {
                    Renderer renderer = blockObject.GetComponent<Renderer>();
                    renderer.material.mainTexture = texture;
                }
                else Debug.LogWarning($"Texture {block.name} introuvable !");
            }
        }
    }
