using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Audio List", fileName = "new Audio List")]
public class SO_AudioCueList : ScriptableObject
{
    public List<AudioCue> audioCues = new List<AudioCue>();
}
