using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    public class GameObjectData
    {
        public string prefabName;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        // Add custom variables here
        public string customData;
    }

    [System.Serializable]
    public class LevelData
    {
        public List<GameObjectData> gameObjects = new List<GameObjectData>();
    }

    public string saveFileName = "level.json";
    public Transform parentForLoadedObjects;

    // Save level to JSON
    public void SaveLevel()
    {
        LevelData levelData = new LevelData();
        foreach (Transform obj in transform)
        {
            GameObjectData data = new GameObjectData
            {
                prefabName = obj.gameObject.name, // You can modify this to map prefab names
                position = obj.position,
                rotation = obj.rotation,
                scale = obj.localScale,
                customData = "Add any extra data here" // Custom data placeholder
            };

            levelData.gameObjects.Add(data);
        }

        string json = JsonUtility.ToJson(levelData, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, saveFileName), json);
        Debug.Log("Level saved to " + saveFileName);
    }

    // Load level from JSON
    public void LoadLevel()
    {
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        if (!File.Exists(path)) return;

        string json = File.ReadAllText(path);
        LevelData levelData = JsonUtility.FromJson<LevelData>(json);

        foreach (var data in levelData.gameObjects)
        {
            GameObject prefab = Resources.Load<GameObject>(data.prefabName); // Ensure your prefabs are in a Resources folder
            if (prefab == null) continue;

            GameObject obj = Instantiate(prefab, data.position, data.rotation, parentForLoadedObjects);
            obj.transform.localScale = data.scale;

            // Load custom data if necessary
            Debug.Log("Loaded object with custom data: " + data.customData);
        }

        Debug.Log("Level loaded from " + saveFileName);
    }
}
