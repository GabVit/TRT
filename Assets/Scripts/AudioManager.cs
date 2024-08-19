using UnityEngine.Audio;
using System;
using UnityEngine;

[Serializable]
public class Sound {
    public string name;
    public AudioClip clip;
    [Range(0, 1)] public float volume = 1;
    [Range(-3, 3)] public float pitch = 1;
    public bool loop = false;
    public bool playOnAwake = false;
    [HideInInspector] public AudioSource source;
}

public class AudioManager : MonoBehaviour {
    public static AudioManager instance;

    public Sound[] sounds;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (var s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.playOnAwake = s.playOnAwake;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name) {
        var s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }

        s.source.Play();
    }

    public void Stop(string name) {
        var s = Array.Find(sounds, sound => sound.name == name);

        s?.source.Stop();
    }
}