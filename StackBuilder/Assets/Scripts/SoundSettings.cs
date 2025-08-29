using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider volumeSlider; // UI Slider referansý
    private const string VolumePref = "Volume";
    private const float muteThreshold = -20f; // mute olarak kabul edilecek deðer

    private void Start()
    {
        float savedVolume = PlayerPrefs.HasKey(VolumePref)
            ? PlayerPrefs.GetFloat(VolumePref)
            : muteThreshold / 2;

        // AudioMixer'e uygula
        SetVolume(savedVolume);

        // Slider varsa deðerini ayarla
        if (volumeSlider != null)
        {
            volumeSlider.minValue = muteThreshold;
            volumeSlider.maxValue = 0f;
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(SetVolume); // Slider hareket ettikçe güncelle
        }
    }

    public void SetVolume(float volume)
    {
        // Mute kontrolü
        if (volume <= muteThreshold)
        {
            audioMixer.SetFloat("Volume", -80f); // tamamen sessiz
        }
        else
        {
            audioMixer.SetFloat("Volume", volume);
        }

        // Kaydet
        PlayerPrefs.SetFloat(VolumePref, volume);
        PlayerPrefs.Save();
    }

    public void UpdateSliderValue()
    {
        if (volumeSlider != null)
        {
            float savedVolume = PlayerPrefs.HasKey(VolumePref)
                ? PlayerPrefs.GetFloat(VolumePref)
                : muteThreshold / 2;

            volumeSlider.value = savedVolume;
        }
    }
}
