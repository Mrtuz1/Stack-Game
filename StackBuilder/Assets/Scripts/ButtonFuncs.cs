using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFuncs : MonoBehaviour
{
    public void StopTime()
    {
        Time.timeScale = 0f;
    }

    public void SetTimeNormal()
    {
        Time.timeScale = 1f;
    }

    public void InputDetected()
    {
        BlockSpawner.instance.SetInputDetected(true);
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(0);
    }

    public void ResetHighScore()
    {
        BlockSpawner.instance.ResetHighScore();
    }
    
}
