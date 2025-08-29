using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour
{
    private bool inputDetected;
    [SerializeField] private TextMeshProUGUI highestScoreText;

    private void Awake()
    {
        inputDetected = false;

        // PlayerPrefs'ten kayýtlý en yüksek skoru çek
        int highestScore = PlayerPrefs.GetInt("HighScore", 0);
        if (highestScoreText != null)
        {
            highestScoreText.text = "High Score: " + highestScore.ToString();
        }
    }

    private void Update()
    {
        HandleInputs();
    }

    private void HandleInputs()
    {
        // Mouse input (PC)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            inputDetected = true;
        }

        // Touch input (Android)
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            inputDetected = true;
        }

        if (inputDetected)
        {
            inputDetected = false;
            SceneManager.LoadScene(1); // 1 indexli sahneyi yükler
        }
    }
}
