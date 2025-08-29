using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class BGAudio : MonoBehaviour
{
    public static BGAudio instance;
    public AudioMixer bgMixer;
    private const string VolumePref = "BGVolume";
    private const float muteThreshold = -20f;

    private Slider slider; // ayný objede bulunan Slider

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            slider = GetComponent<Slider>();

            float savedVolume = PlayerPrefs.HasKey(VolumePref)
                ? PlayerPrefs.GetFloat(VolumePref)
                : muteThreshold / 2;

            SetVolume(savedVolume);

            UpdateSliderValue(); // açýlýþta da güncelle
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetVolume(float volume)
    {
        if (bgMixer != null)
        {
            if (volume <= muteThreshold)
                bgMixer.SetFloat(VolumePref, -80f); // tamamen sessiz
            else
                bgMixer.SetFloat(VolumePref, volume);
        }

        PlayerPrefs.SetFloat(VolumePref, volume);
        PlayerPrefs.Save();
    }

    public void UpdateSliderValue()
    {
        if (slider != null)
        {
            float savedVolume = PlayerPrefs.HasKey(VolumePref)
                ? PlayerPrefs.GetFloat(VolumePref)
                : muteThreshold / 2;

            slider.value = savedVolume;
        }
    }
}
