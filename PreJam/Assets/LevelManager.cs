using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelData
    {
        public List<GameObject> gameObjects = new List<GameObject>();
    }

    public TMP_InputField saveFileNameInput;

    // Save level to JSON
    public void SaveLevel()
    {
        LevelData levelData = new LevelData();
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            levelData.gameObjects.Add(obj);
        }

        string json = JsonUtility.ToJson(levelData, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, saveFileNameInput.text), json);
        Debug.Log("Level saved to " + saveFileNameInput.text);
    }

    // Load level from JSON
    public void LoadLevel()
    {
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.transform.parent != null) return;
            Destroy(obj);
        }
        
        string path = Path.Combine(Application.persistentDataPath, saveFileNameInput.text);
        if (!File.Exists(path)) return;

        string json = File.ReadAllText(path);
        LevelData levelData = JsonUtility.FromJson<LevelData>(json);

        foreach (var data in levelData.gameObjects)
        {
            GameObject obj = Instantiate(data, data.transform.position, data.transform.rotation);
            obj.transform.localScale = data.transform.localScale;
        }

        Debug.Log("Level loaded from " + saveFileNameInput.text);
    }
}
