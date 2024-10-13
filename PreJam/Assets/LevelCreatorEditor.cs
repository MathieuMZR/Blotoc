using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelCreator))]
public class LevelCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector (shows regular variables and fields)
        DrawDefaultInspector();

        // Get a reference to the LevelCreator script
        LevelCreator levelCreator = (LevelCreator)target;

        // Add a space in the inspector
        EditorGUILayout.Space();

        // Add a button that runs the AnalyseTexture method
        if (GUILayout.Button("Create Level"))
        {
            if (levelCreator.tex is null)
                throw new Exception("Please reference the texture in the level creator's inspector.");
            // Call the AnalyseTexture function in the LevelCreator script
            levelCreator.RunGeneration();
        }
        
        // Add a button that runs the AnalyseTexture method
        if (GUILayout.Button("Delete children"))
        {
            // Call the AnalyseTexture function in the LevelCreator script
            levelCreator.KillChildren();
        }
    }
}
