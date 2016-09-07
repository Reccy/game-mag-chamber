using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityExtensions.MathfExtensions;

public class Sound_Test : MonoBehaviour 
{
    SoundManager soundManager;

    void Start()
    {
        soundManager = Object.FindObjectOfType<SoundManager>();
    }

    //Play sounds
    public void PlaySound(Text soundName)
    {
        soundManager.SetLooped(soundName.text, false);
        soundManager.PlaySound(soundName.text);
    }

    public void PlayLoopedSound(Text soundName)
    {
        soundManager.SetLooped(soundName.text, true);
        soundManager.PlaySound(soundName.text);
    }

    public void PlayOneShot(Text soundName)
    {
        soundManager.PlayOneShot(soundName.text);
    }

    public void StopSound(Text soundName)
    {
        soundManager.StopSound(soundName.text);
    }

    public void PauseSound(Text soundName)
    {
        soundManager.PauseSound(soundName.text);
    }

    //Volume
    public void ChangeMasterVolume(Slider slider)
    {
        soundManager.SetVolume("Master", MathfExtensions.LinearToDecibel(slider.value));
    }

    public void ChangeMusicVolume(Slider slider)
    {
        soundManager.SetVolume("Music", MathfExtensions.LinearToDecibel(slider.value));
    }

    public void ChangeSFXVolume(Slider slider)
    {
        soundManager.SetVolume("SFX", MathfExtensions.LinearToDecibel(slider.value));
    }

    public void ChangeUIVolume(Slider slider)
    {
        soundManager.SetVolume("UI", MathfExtensions.LinearToDecibel(slider.value));
    }

    //Mute
    public void ToggleMasterMute(Toggle tog)
    {
        soundManager.SetMute("Master", tog.isOn);
    }

    public void ToggleMusicMute(Toggle tog)
    {
        soundManager.SetMute("Music", tog.isOn);
    }

    public void ToggleSFXMute(Toggle tog)
    {
        soundManager.SetMute("SFX", tog.isOn);
    }

    public void ToggleUIMute(Toggle tog)
    {
        soundManager.SetMute("UI", tog.isOn);
    }
}
