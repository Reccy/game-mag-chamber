using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityExtensions.MathfExtensions;

public class SoundManager : MonoBehaviour
{
    public enum SlotState { Empty, Playing, Stopped, Paused }; //Slot state
    public enum SlotType { Normal, OneShot }; //Slot type
    public enum SoundChannel { Music, SFX, UI }; //Sound channels

    public AudioMixer audioMixer; //The audio mixer
    public AudioMixerGroup groupMusic, groupUI, groupSFX; //The audio mixer groups

    private float masterVolume, musicVolume, uiVolume, sfxVolume; //Volumes of each group
    private bool masterMute, musicMute, uiMute, sfxMute; //Is muted of each group

    public AudioClip[] audioClips; //List of audio clips
    private AudioSlot[] audioSlots; //List of audio slots
    public int maxSlots; //The maximum amount of slots to store

    void Awake()
    {
        //Create audio slots
        audioSlots = new AudioSlot[maxSlots];
        for(int i = 0; i < maxSlots; i++)
        {
            audioSlots[i] = new AudioSlot();
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

    //Plays sound
    public AudioSlot Play(string soundName, SoundChannel channel, float volume = 1, bool loop = false, float randomPitchRange = 0, int priority = 128, bool randomStart = false)
    {
        //Get a slot to apply the clip to
        AudioSlot slot = null;

        slot = GetOccupiedSlot(soundName);
        
        if(slot == null)
        {
            slot = GetEmptySlot();
        }

        //Set slot/clip paramaters
        slot.SetClip(GetClip(soundName), SlotType.Normal, channel);
        slot.GetSource().loop = loop;
        slot.GetSource().priority = priority;
        slot.GetSource().pitch = 1;
        slot.GetSource().volume = volume;

        //Apply pitch if required
        if (randomPitchRange != 0)
            slot.GetSource().pitch = Random.Range(slot.GetSource().pitch - randomPitchRange / 2, slot.GetSource().pitch + randomPitchRange / 2);

        //Start at random point in clip if applied
        if (randomStart)
            slot.GetSource().time = Random.Range(0, slot.GetSource().clip.length);

        //Play sound
        slot.Play();

        //Return slot if called class wants to directly access the slot
        return slot;
    }

    //Plays sound one-shot
    public AudioSlot PlayOneShot(string soundName, SoundChannel channel, float volume = 1, bool loop = false, float randomPitchRange = 0, int priority = 128)
    {
        //Get a slot to apply the clip to
        AudioSlot slot = null;

        slot = GetOccupiedSlot(soundName);

        if (slot == null)
        {
            slot = GetEmptySlot();
        }

        //Set slot/clip paramaters
        slot.SetClip(GetClip(soundName), SlotType.OneShot, channel);
        slot.GetSource().loop = loop;
        slot.GetSource().priority = priority;
        slot.GetSource().pitch = 1;
        slot.GetSource().volume = volume;

        //Apply pitch if required
        if (randomPitchRange != 0)
            slot.GetSource().pitch = Random.Range(slot.GetSource().pitch - randomPitchRange/2, slot.GetSource().pitch + randomPitchRange/2);

        //Play sound
        slot.Play();

        //Return slot if called class wants to directly access the slot
        return slot;
    }

    //Resumes from pause
    public void Resume(string soundName)
    {
        AudioSlot slot = GetOccupiedSlot(soundName);
        slot.GetSource().UnPause();
    }

    //Returns if the clip is playing
    public bool IsPlaying(string soundName)
    {
        AudioSlot slot = GetOccupiedSlot(soundName);
        if (slot == null)
            return false;

        return slot.GetSource().isPlaying;
    }

    //Stops sound fully
    public void Stop(string soundName)
    {
        AudioSlot slot = GetOccupiedSlot(soundName);

        if(slot != null)
            slot.Stop();
    }

    //Pauses sound (and all one shots too)
    public void Pause(string soundName)
    {
        AudioSlot slot = GetOccupiedSlot(soundName);

        if(slot != null)
            slot.Pause();
    }

    //Attempts to return the clip in the sounds list
    AudioClip GetClip(string clipName)
    {
        foreach(AudioClip clip in audioClips)
        {
            if(clip.name == clipName)
            {
                return clip;
            }
        }
        return null;
    }

    //Attempts to return an empty slot. If not, returns slot with lowest priority.
    AudioSlot GetEmptySlot()
    {
        AudioSlot lowestPrioritySlot = audioSlots[0];
        int lowestPriority = 0;

        foreach(AudioSlot slot in audioSlots)
        {
            if (slot.GetSource().priority >= lowestPriority)
            {
                lowestPriority = slot.GetSource().priority;
                lowestPrioritySlot = slot;
            }

            if(slot.GetState() == SlotState.Empty || !slot.GetSource().isPlaying)
            {
                return slot;
            }
        }

        Debug.LogError("No free sound slots. Returning lowest priority slot.");

        return lowestPrioritySlot;
    }

    //Attempts to return an occupied slot by name.
    public AudioSlot GetOccupiedSlot(string clipName)
    {
        foreach(AudioSlot slot in audioSlots)
        {
            if(slot.GetClip() != null)
            {
                if (slot.GetClip().name == clipName && slot.GetState() != SlotState.Empty && slot.GetSlotType() == SlotType.Normal)
                {
                    return slot;
                }
            }
        }

        return null;
    }

    //Stops all slots in sound manager
    void StopAllSlots()
    {
        foreach(AudioSlot slot in audioSlots)
        {
            if(slot != null)
                slot.Stop();
        }
    }

    //Clears all slots in sound manager
    void ClearAllSlots()
    {
        foreach(AudioSlot slot in audioSlots)
        {
            if(slot != null)
            {
                slot.Clear();
            }
        }
    }
}

//Stores data for audio slot
public class AudioSlot
{
    private SoundManager soundManager;
    private AudioSource source;

    
    private SoundManager.SlotState state;
    private SoundManager.SlotType type;
    private SoundManager.SoundChannel channel;

    //Constructor
    public AudioSlot()
    {
        soundManager = Object.FindObjectOfType<SoundManager>();
        source = soundManager.gameObject.AddComponent<AudioSource>();
        state = SoundManager.SlotState.Empty;
        type = SoundManager.SlotType.Normal;
    }

    //Sets the clip to play & type
    public void SetClip(AudioClip clip, SoundManager.SlotType type, SoundManager.SoundChannel channel)
    {
        if(type == SoundManager.SlotType.Normal)
        {
            source.Stop();
            state = SoundManager.SlotState.Stopped;
        }

        source.clip = clip;
        this.type = type;
        this.channel = channel;

        switch(channel)
        {
            case SoundManager.SoundChannel.Music:
                source.outputAudioMixerGroup = soundManager.groupMusic;
                break;
            case SoundManager.SoundChannel.SFX:
                source.outputAudioMixerGroup = soundManager.groupSFX;
                break;
            case SoundManager.SoundChannel.UI:
                source.outputAudioMixerGroup = soundManager.groupUI;
                break;
        }
    }

    //Plays the source
    public void Play()
    {
        if (type == SoundManager.SlotType.Normal)
            source.Play();
        else if (type == SoundManager.SlotType.OneShot)
            source.PlayOneShot(source.clip);
        
        state = SoundManager.SlotState.Playing;
    }

    //Pauses the source
    public void Pause()
    {
        source.Pause();
        state = SoundManager.SlotState.Paused;
    }

    //Stops the source
    public void Stop()
    {
        source.Stop();
        state = SoundManager.SlotState.Stopped;
    }

    //Clears slot
    public void Clear()
    {
        source.Stop();
        state = SoundManager.SlotState.Empty;
    }

    //Get the current clip
    public AudioClip GetClip()
    {
        return source.clip;
    }

    public AudioSource GetSource()
    {
        return source;
    }

    //Returns states
    public SoundManager.SlotState GetState()
    {
        return state;
    }

    public SoundManager.SlotType GetSlotType()
    {
        return type;
    }

    public SoundManager.SoundChannel GetChannel()
    {
        return channel;
    }
}