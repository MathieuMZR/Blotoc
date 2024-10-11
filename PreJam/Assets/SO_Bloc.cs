using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Scriptable Objects/Bloc", fileName = "new Bloc")]
public class SO_Bloc : ScriptableObject
{
    public string name;
    public string description;
    public Sprite sprite;
    public GameObject prefab;
}
