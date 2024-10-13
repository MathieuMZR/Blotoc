using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelCreator : MonoBehaviour
{
    public Texture2D tex;
    private SO_Bloc[,] _gridSize;
    [SerializeField] private float colorDistanceThreshold = 0.5f;

    public GameObject basePlane;
    public List<SO_Bloc> availableBlocs = new List<SO_Bloc>();

    public void RunGeneration()
    {
        DefineGridSize();
        KillChildren();
        SpawnPlane();
        AnalyseTexture();
    }

    private void DefineGridSize()
    {
        var sizeX = tex.width;
        var sizeY = tex.height;
        
        _gridSize = new SO_Bloc[sizeX, sizeY];
    }

    public void KillChildren()
    {
        foreach (var t in GetComponentsInChildren<Transform>())
        {
            if(t && t != transform) DestroyImmediate(t.gameObject);
        }
    }

    private void SpawnPlane()
    {
        var obj = Instantiate(basePlane, transform);

        float sizeX = _gridSize.GetLength(0);
        if (sizeX % 2 != 0) sizeX += 1f;
        
        float sizeY = _gridSize.GetLength(1);
        if (sizeY % 2 != 0) sizeY += 1f;
        
        obj.transform.localScale = new Vector3(sizeX, obj.transform.localScale.y, sizeY);
    }

    private void AnalyseTexture()
    {
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                var currentPixel = tex.GetPixel(x, y);
                
                var analyse = FindNearestBlocFromColor(currentPixel, availableBlocs.ToArray());
                var bloc = analyse.Item1;
            
                if (currentPixel == Color.white || bloc is null) continue;

                var offsetX = Mathf.RoundToInt(_gridSize.GetLength(0) / 2f - 1);
                var offsetY = Mathf.RoundToInt(_gridSize.GetLength(0) / 2f);
                
                Instantiate(bloc.prefab, new Vector3(x-offsetX, 0, y-offsetY), Quaternion.identity, transform);
            }
        }
    }
    
    (SO_Bloc, Color) FindNearestBlocFromColor(Color target, SO_Bloc[] blocs)
    {
        List<Color> colors = new List<Color>();
        foreach (var b in blocs) colors.Add(b.texColor);
        SO_Bloc foundBloc = null;

        Color nearestColor = Color.black; // Start with the first color
        float minDistance = ColorDistance(target, nearestColor);

        int i = 0;
        // Check each color in the list
        foreach (Color color in colors)
        {
            float distance = ColorDistance(target, color);
        
            // If the distance is within the threshold, consider it a match
            if (distance < minDistance && distance < colorDistanceThreshold)
            {
                minDistance = distance;
                nearestColor = color;
                foundBloc = blocs[i];
            }
            i++;
        }

        return (foundBloc, nearestColor);
    }
    
    float ColorDistance(Color c1, Color c2)
    {
        float rDiff = c1.r - c2.r;
        float gDiff = c1.g - c2.g;
        float bDiff = c1.b - c2.b;

        return Mathf.Sqrt(rDiff * rDiff + gDiff * gDiff + bDiff * bDiff);
    }
}
