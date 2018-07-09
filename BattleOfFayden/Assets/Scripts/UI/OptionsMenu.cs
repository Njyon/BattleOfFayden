using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {
    public AudioMixer mixerMusic;
    public AudioMixer mixerSFX;
    public Slider fxVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider LightnessSlider;
    public Slider contrastSlider;

    private void Start()
    {
        fxVolumeSlider.onValueChanged.AddListener(delegate { ValueChangedFXVolume(); });
        musicVolumeSlider.onValueChanged.AddListener(delegate { ValueChangedMusicVolume(); });
        LightnessSlider.onValueChanged.AddListener(delegate { ValueChangeLightness(); });
        contrastSlider.onValueChanged.AddListener(delegate { ValueChangedContrast(); });

        fxVolumeSlider.value = .5f;
        musicVolumeSlider.value = .5f;
        LightnessSlider.value = .5f;
        contrastSlider.value = .5f;

        mixerSFX.SetFloat("Master", fxVolumeSlider.value * 60 - 40);
        mixerMusic.SetFloat("Master", musicVolumeSlider.value * 60 - 40);
    }

    public void ValueChangedFXVolume()
    {
        PlayerPrefs.SetFloat("FXVolume", fxVolumeSlider.value * 60 -40);
        mixerSFX.SetFloat("Master", fxVolumeSlider.value * 60 - 40);
    }
    public void ValueChangedMusicVolume()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value * 60 - 40);
        mixerMusic.SetFloat("Master", musicVolumeSlider.value * 60 - 40);
    }
    public void ValueChangeLightness()
    {
        PlayerPrefs.SetFloat("Lightness", LightnessSlider.value);
        RenderSettings.ambientLight = new Color(LightnessSlider.value, LightnessSlider.value, LightnessSlider.value, 1);
    }
    public void ValueChangedContrast()
    {
        PlayerPrefs.SetFloat("Contrast", contrastSlider.value);
    }
}
