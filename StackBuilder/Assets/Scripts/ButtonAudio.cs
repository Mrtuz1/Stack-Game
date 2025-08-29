using UnityEngine;

public class ButtonAudio : MonoBehaviour
{
    public static ButtonAudio instance;

    private AudioSource m_AudioSource;

    private void Awake()
    {
        // Singleton kontrol�
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // sahne de�i�se de yok olmas�n
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        m_AudioSource = GetComponent<AudioSource>();
    }

    public void PlayAudio()
    {
        if (m_AudioSource != null)
        {
            m_AudioSource.Play();
        }
    }
}
