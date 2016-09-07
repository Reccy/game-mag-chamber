using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityExtensions.MathfExtensions;

public class SoundManager : MonoBehaviour
{
    public AudioMixer audioMixer; //The audio mixer
    public AudioMixerGroup groupMusic, groupUI, groupSFX; //The audio mixer groups

    private float masterVolume, musicVolume, uiVolume, sfxVolume; //Volumes of each group
    private bool masterMute, musicMute, uiMute, sfxMute; //Is muted of each group

    public AudioClip[] audioClips; //List of audio clips
    private Dictionary<string, AudioSource> audioSources; //Dictionary of audio sources

    void Awake()
    {
        audioSources = new Dictionary<string,AudioSource>();

        //Add all clips to audio sources
        foreach(AudioClip clip in audioClips)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            
            string clipName = clip.name;

            //Route source to audio mixer
            if(clipName.StartsWith("mus_")) //Music channel
            {
                clipName = clipName.Remove(0, 4);
                source.outputAudioMixerGroup = groupMusic;
                Debug.Log(clipName + " to Music");
            }
            else if(clipName.StartsWith("ui_")) //UI channel
            {
                clipName = clipName.Remove(0, 3);
                source.outputAudioMixerGroup = groupUI;
                Debug.Log(clipName + " to UI");
            }
            else if(clipName.StartsWith("sfx_")) //SFX channel
            {
                clipName = clipName.Remove(0, 4);
                source.outputAudioMixerGroup = groupSFX;
                Debug.Log(clipName + " to SFX");
            }
            else
            {
                //Error with prefix
                Debug.Log("SYNTAX ERROR: " + clipName);
            }

            source.clip = clip;
            audioSources.Add(clipName, source);
        }
    }

    //
    //Code for Audio Mixer groups
    //

    //Change mixer volume
    public void SetVolume(string mixerName, float mixerVolume)
    {
        switch(mixerName)
        {
            case "Master":
                audioMixer.SetFloat("MasterVolume", mixerVolume);
                masterVolume = mixerVolume;
                break;
            case "Music":
                audioMixer.SetFloat("MusicVolume", mixerVolume);
                musicVolume = mixerVolume;
                break;
            case "UI":
                audioMixer.SetFloat("UIVolume", mixerVolume);
                uiVolume = mixerVolume;
                break;
            case "SFX":
                audioMixer.SetFloat("SFXVolume", mixerVolume);
                sfxVolume = mixerVolume;
                break;
            default:
                Debug.LogError("No exposed variable of value " + mixerName);
                break;
        }
    }

    //Toggles the mute of chosen channel
    public void SetMute(string mixerName, bool isMuted)
    {
        switch(mixerName)
        {
            case "Master":
                if(masterMute)
                {
                    audioMixer.SetFloat("MasterVolume", masterVolume);
                    masterMute = false;
                }
                else
                {
                    audioMixer.SetFloat("MasterVolume", MathfExtensions.LinearToDecibel(0));
                    masterMute = true;
                }
                break;
            case "Music":
                if(musicMute)
                {
                    audioMixer.SetFloat("MusicVolume", musicVolume);
                    musicMute = false;
                }
                else
                {
                    audioMixer.SetFloat("MusicVolume", MathfExtensions.LinearToDecibel(0));
                    musicMute = true;
                }
                break;
            case "UI":
                if(uiMute)
                {
                    audioMixer.SetFloat("UIVolume", uiVolume);
                    uiMute = false;
                }
                else
                {
                    audioMixer.SetFloat("UIVolume", MathfExtensions.LinearToDecibel(0));
                    uiMute = true;
                }
                break;
            case "SFX":
                if(sfxMute)
                {
                    audioMixer.SetFloat("SFXVolume", sfxVolume);
                    sfxMute = false;
                }
                else
                {
                    audioMixer.SetFloat("SFXVolume", MathfExtensions.LinearToDecibel(0));
                    sfxMute = true;
                }
                break;
            default:
                Debug.LogError("No exposed variable of value " + mixerName);
                break;
        }
    }

    //
    //Code for individual sounds
    //

    //Plays sound normally
    public void PlaySound(string soundName)
    {
        if (audioSources.ContainsKey(soundName))
        {
            audioSources[soundName].Play();
        }
    }

    //Plays sound while allowing overlapping
    public void PlayOneShot(string soundName)
    {
        if (audioSources.ContainsKey(soundName))
        {
            audioSources[soundName].PlayOneShot(audioSources[soundName].clip);
        }
    }

    //Whether or not to loop the sound
    public void SetLooped(string soundName, bool loop)
    {
        if (audioSources.ContainsKey(soundName))
        {
            audioSources[soundName].loop = loop;
        }
    }

    //Stops sound fully
    public void StopSound(string soundName)
    {
        if (audioSources.ContainsKey(soundName))
        {
            audioSources[soundName].Stop();
        }
    }

    //Pauses sound (and all one shots too)
    public void PauseSound(string soundName)
    {
        if (audioSources.ContainsKey(soundName))
        {
            audioSources[soundName].Pause();
        }
    }
}
