using UnityEngine;
using UnityEngine.Audio;

public class GeneralMusic : MonoBehaviour
{
    public static GeneralMusic instance;
    public AudioMixer mainMixer; // Inspector'dan baðlayacaðýmýz mixer
    private const string VolumePref = "Volume";
    private const float muteThreshold = -20f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // PlayerPrefs'den ses deðerini al
            float savedVolume = PlayerPrefs.HasKey(VolumePref)
                ? PlayerPrefs.GetFloat(VolumePref)
                : muteThreshold / 2;

            // AudioMixer'e uygula
            SetVolume(savedVolume);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetVolume(float volume)
    {
        if (mainMixer != null)
        {
            if (volume <= muteThreshold)
                mainMixer.SetFloat(VolumePref, -80f); // tamamen sessiz
            else
                mainMixer.SetFloat(VolumePref, volume);
        }

        // Kaydet
        PlayerPrefs.SetFloat(VolumePref, volume);
        PlayerPrefs.Save();
    }
}
