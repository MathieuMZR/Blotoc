using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : GenericSingletonClass<AudioManager>
{
    [SerializeField] private AudioCueObject audioCueObject;
    [SerializeField] private SO_AudioCueList sfxList;

    [SerializeField] private AudioSource musicSource;

    public void PlaySound(string cueName, Transform parent = null)
    {
        var c = GetCueByString(cueName);
        
        if(IsTooMuchSoundPlaying(cueName)) return;
            
        var cue = Instantiate(audioCueObject, Vector3.zero, Quaternion.identity, parent);
        cue.SetAudioClip(c);
        cue.GetComponent<AudioSource>().outputAudioMixerGroup = c.audioMixerGroup;
    }
    
    public AudioCueObject PlaySoundOut(string cueName, Transform parent = null)
    {
        var c = GetCueByString(cueName);
        
        if(IsTooMuchSoundPlaying(cueName)) return null;
            
        var cue = Instantiate(audioCueObject, Vector3.zero, Quaternion.identity, parent);
        cue.SetAudioClip(c);
        cue.GetComponent<AudioSource>().outputAudioMixerGroup = c.audioMixerGroup;

        return cue;
    }
    
    public void PlaySoundWithPitch(string cueName, Transform parent = null, float pitch = 1f)
    {
        var c = GetCueByString(cueName);
        
        if(IsTooMuchSoundPlaying(cueName)) return;
            
        var cue = Instantiate(audioCueObject, Vector3.zero, Quaternion.identity, parent);
        
        c.minMaxPitch = new Vector2(pitch, pitch);
        
        cue.SetAudioClip(c);
    }

    bool IsTooMuchSoundPlaying(string cueName)
    {
        var audioCues = FindObjectsOfType<AudioCueObject>().ToList();
        var amount = 0;

        foreach (var ac in audioCues)
        {
            if (ac.audioCue.cueName == cueName) amount++;
        }

        return amount >= GetCueByString(cueName).maxSoundPlaying;
    }

    public void LevelLost()
    {
        musicSource.DOKill();
        DOTween.To(() => musicSource.pitch, x => musicSource.pitch = x, 0.2f, 1f);
    }

    public AudioCue GetCueByString(string cueName) => sfxList.audioCues.Find(x => x.cueName == cueName);
}

[Serializable]
public class AudioCue
{
    public string cueName;
    public float volume = 1f;
    public bool loop;
    public Vector2 minMaxPitch = new(1,1);
    public AudioMixerGroup audioMixerGroup;
    
    public int maxSoundPlaying;
    
    public AudioClip audioClip;
}
