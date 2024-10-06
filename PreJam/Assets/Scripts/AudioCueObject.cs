using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioCueObject : MonoBehaviour
{
    public AudioCue audioCue;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void SetAudioClip(AudioCue ac)
    {
        audioCue = ac;
        
        _audioSource.clip = ac.audioClip;
        _audioSource.volume = ac.volume;
        _audioSource.pitch = Random.Range(ac.minMaxPitch.x, ac.minMaxPitch.y);

        _audioSource.loop = ac.loop;
        
        _audioSource.Play();
        
        if (ac.loop) return;
        
        Destroy(gameObject, ac.audioClip.length);
    }
}
