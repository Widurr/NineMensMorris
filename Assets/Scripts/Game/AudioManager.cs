using UnityEngine.Audio;
using System;
using UnityEngine.SceneManagement;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public AudioMixerGroup audiomix;
    // Start is called before the first frame update
    void Awake()
    {
        foreach(Sound s in sounds){
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.outputAudioMixerGroup = audiomix;
            s.source.loop = s.loop;
        }
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "StartMenu")
            Play("MenuTheme");
        else
            Play("PlayTheme");
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {

            return;
        }
        s.source.Play();
    }
}
