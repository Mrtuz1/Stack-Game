using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using Unity.Mathematics;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class BlockSpawner : MonoBehaviour
{
    [SerializeField] private GameObject pulseEffect;
    public static BlockSpawner instance { get; private set; }

    [SerializeField] private float perfectPlacementLimit = 0.1f;

    private bool isGameFinished;

    [SerializeField] GameObject firstBlock;
    [SerializeField] CameraController cameraController;
    [SerializeField] GameObject block;
    [SerializeField] GameObject fallingBlockObj;
    private Vector3 spawnpoint;
    private bool inputDetected;
    private GameObject currentBlock;
    private bool isInverted;
    private Block blockScript;
    public float blockY;
    private Vector3 prevBlockPos;
    private GameObject prevBlock;

    [SerializeField] private TextMeshProUGUI pointText;
    [SerializeField] private TextMeshProUGUI streakText;
    private int point;
    private int streak;
    private int highScore;

    private Color curBlockColor;
    private int changingChannel;
    private float channelChangingAmount = 0.1f;
    private int isColorInc;

    [SerializeField] private float backgroundLerpSpeed = 0.5f; // Yumuþaklýk hýzý
    private Color currentBackgroundColor;

    private float streakIncreasment = 0.1f;

    [SerializeField] private AudioClip[] noteSounds;
    [SerializeField] private AudioClip[] noteSoundsDoubleBas;
    private int perfectAudioIndex;
    private int normalAudioIndex;
    private AudioSource audioSource;

    public bool isFirstColorSet;

    [SerializeField] private Canvas UICanvas;
    [SerializeField] private Canvas endGameCanvas;
    [SerializeField] private GameObject newHighScoreObject;
    [SerializeField] private GameObject endGameScoreObject;
    [SerializeField] private TextMeshProUGUI newHighScoreText;
    [SerializeField] private TextMeshProUGUI endGameScoreText;

    [SerializeField] private GameObject highestScoreObject;
    [SerializeField] private TextMeshProUGUI highestScoreText;

    private int blocksCreated;

    private void Awake()
    {
        blocksCreated = 0;
        instance = this;

        audioSource = GetComponent<AudioSource>();
        perfectAudioIndex = 0;
        normalAudioIndex = 0;

        currentBackgroundColor = Camera.main.backgroundColor;

        point = 0;
        streak = 0;
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        Time.timeScale = 1f;
        isGameFinished = false;
        isFirstColorSet = false;

        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Eðer zaten bir instance varsa kendini yok et
            return;
        }

        isInverted = false;
        inputDetected = false;
        spawnpoint = new Vector3(0f, 0.5f, 0f);
        currentBlock = firstBlock;
        blockY = block.transform.localScale.y;
        prevBlock = null;
        RandomizeStartingColor();
    }

    private void Update()
    {
        if (isGameFinished)
        {
            return;
        }

        HandleInputs();
        UpdateBackgroundColor();
        pointText.text = point.ToString();
        streakText.text = streak.ToString();
    }

    private void HandleInputs()
    {
        if (inputDetected)
        {
            inputDetected = false;
            SplitBlock();

            if(prevBlock == null)
            {
                point--;
            }

            // Bloðu durdur ve split iþlemini yap
            Vector3 currentPos = currentBlock.transform.position;
            // Bir sonraki blok için bilgileri güncelle
            prevBlockPos = currentPos;
            prevBlock = currentBlock;


            if (blockScript != null)
            {
                blockScript.isPlaced = true;
            }

            if (isGameFinished)
            {
                return;
            }
            
            cameraController.MoveCameraUp(blockY);
            blocksCreated++;
            SpawnBlock();
        }
    }

    //sonraki block'u küçüklt
    private void SpawnBlock()
    {
        if (!isInverted)
        {
            spawnpoint = new Vector3(prevBlockPos.x, spawnpoint.y, prevBlockPos.z + prevBlock.transform.localScale.z + 1f);
        }
        else
        {
            spawnpoint = new Vector3(prevBlockPos.x - prevBlock.transform.localScale.x - 1f, spawnpoint.y, prevBlockPos.z);
        }
        isInverted = !isInverted;
        currentBlock = Instantiate(block, spawnpoint, block.transform.rotation);
        ChangeColor(currentBlock);
        blockScript = currentBlock.GetComponent<Block>();
        spawnpoint += new Vector3(0f, blockY, 0f);
        blockScript.SetOrder(blocksCreated);
        point++;
    }
    
    private void SplitBlock()
    {
        if (prevBlock == null) return;
        float newSizeX;
        float newSizeZ;
        Vector3 fallingBlockScale;
        Vector3 fallingBlockSpawnPos;
        float distancePivot;

        if (isInverted)
        {
            newSizeZ = prevBlock.transform.localScale.z - Mathf.Abs(currentBlock.transform.position.z - prevBlockPos.z);
            distancePivot = currentBlock.transform.position.z - prevBlockPos.z;

            if(Mathf.Abs(distancePivot) < perfectPlacementLimit)
            {
                currentBlock.transform.position = new Vector3(prevBlockPos.x, currentBlock.transform.position.y, prevBlockPos.z);
                PlayPerfectAudio();
                streak++;
                normalAudioIndex = 0;
                HandleStreak();

                SpawnPulseEffect(currentBlock.transform.position, prevBlock.transform.localScale);

                return;
            }
            else
            {
                streak = 0;
                PlayNormalAudio();
            }

            fallingBlockScale = new Vector3(prevBlock.transform.localScale.x, prevBlock.transform.localScale.y, prevBlock.transform.localScale.z - newSizeZ);

            if (Mathf.Abs(distancePivot) >= prevBlock.transform.localScale.z) // Z ekseni için
            {
                EndGame();
                return;
            }

            // Blok boyutunu güncelle
            Vector3 newScale = currentBlock.transform.localScale;
            newScale.z = newSizeZ;
            currentBlock.transform.localScale = newScale;

            if(distancePivot < 0)
            {
                currentBlock.transform.position = new Vector3(prevBlockPos.x, currentBlock.transform.position.y, prevBlockPos.z);
                fallingBlockSpawnPos = currentBlock.transform.position - new Vector3(0f, 0f, fallingBlockScale.z);
            }
            else
            {
                fallingBlockSpawnPos = currentBlock.transform.position + new Vector3(0f, 0f, newSizeZ);
            }

        }
        else
        {
            newSizeX = prevBlock.transform.localScale.x - Mathf.Abs(currentBlock.transform.position.x - prevBlockPos.x);
            distancePivot = currentBlock.transform.position.x - prevBlockPos.x;

            if (Mathf.Abs(distancePivot) < perfectPlacementLimit)
            {
                currentBlock.transform.position = new Vector3(prevBlockPos.x, currentBlock.transform.position.y, prevBlockPos.z);
                PlayPerfectAudio();
                streak++;
                normalAudioIndex = 0;
                HandleStreak();

                SpawnPulseEffect(currentBlock.transform.position, prevBlock.transform.localScale);

                return;
            }
            else
            {
                streak = 0;
                PlayNormalAudio();
            }


            fallingBlockScale = new Vector3(prevBlock.transform.localScale.x - newSizeX, prevBlock.transform.localScale.y, prevBlock.transform.localScale.z);

            if (Mathf.Abs(distancePivot) >= prevBlock.transform.localScale.x) // X ekseni için
            {
                EndGame();
                return;
            }

            // Blok boyutunu güncelle
            Vector3 newScale = currentBlock.transform.localScale;
            newScale.x = newSizeX;
            currentBlock.transform.localScale = newScale;

            if(distancePivot > 0)
            {
                currentBlock.transform.position = new Vector3(prevBlockPos.x, currentBlock.transform.position.y, prevBlockPos.z);
                fallingBlockSpawnPos = currentBlock.transform.position + new Vector3(fallingBlockScale.x, 0f, 0f);
            }
            else 
            {
                fallingBlockSpawnPos = currentBlock.transform.position - new Vector3(newSizeX, 0f, 0f);
            }

        }
        
        GameObject fallingBlock = Instantiate(fallingBlockObj, fallingBlockSpawnPos, fallingBlockObj.transform.rotation);
        fallingBlock.transform.localScale = fallingBlockScale;
    }

    public Vector3 GetPrevBlockScale()
    {
        return prevBlock.transform.localScale;
    }

    public Vector3 GetCurBlockScale()
    {
        return currentBlock.transform.localScale;
    }

    public float GetSpawnPointY()
    {
        return spawnpoint.y;
    }

    public bool GetIsInverted()
    {
        return isInverted;
    }

    public Vector3 GetPrevBlockPos()
    {
        return prevBlockPos;
    }

    public Color GetCurColor()
    {
        return curBlockColor;
    }

    private void EndGame()
    {
        isGameFinished = true;

        StopGame();

        UICanvas.gameObject.SetActive(false);
        endGameCanvas.gameObject.SetActive(true);

        if (point > highScore)
        {
            highScore = point;

            PlayerPrefs.SetInt("HighScore", point);
            PlayerPrefs.Save();
            newHighScoreObject.SetActive(true); 
            newHighScoreText.text = point.ToString();
        }
        else
        {
            endGameScoreObject.SetActive(true);
            endGameScoreText.text = point.ToString();
            highestScoreObject.SetActive(true);
            highestScoreText.text = highScore.ToString();
        }
    }

    private void StopGame()
    {
        Time.timeScale = 0f;
    }

    private void ChangeColor(GameObject blockObj)
    {
        ChangeChannelValue();

        Renderer childRenderer = blockObj.GetComponentInChildren<Renderer>();

        if (childRenderer != null)
        {

            // Rengi uygula
            childRenderer.material.color = curBlockColor;
        }
    }

    private void ChangeChannelValue()
    {

        if (changingChannel == 0)
        {
            curBlockColor.r = Mathf.Clamp01(curBlockColor.r + channelChangingAmount * isColorInc);
            if (curBlockColor.r <= 0f || curBlockColor.r >= 1f)
            {
                changingChannel++;
                isColorInc *= -1;
            }

        }
        else if (changingChannel == 1)
        {
            curBlockColor.g = Mathf.Clamp01(curBlockColor.g + channelChangingAmount * isColorInc);
            if (curBlockColor.g <= 0f || curBlockColor.g >= 1f)
            {
                changingChannel++;
                isColorInc *= -1;
            }
        }
            
        else if(changingChannel == 2)
        {
            curBlockColor.b = Mathf.Clamp01(curBlockColor.b + channelChangingAmount * isColorInc);
            if (curBlockColor.b <= 0f || curBlockColor.b >= 1f)
            {
                changingChannel = 0;
                isColorInc *= -1;
            }
        }

    }


    private void RandomizeStartingColor()
    {
        curBlockColor.r = Random.value;
        curBlockColor.g = Random.value;
        curBlockColor.b = Random.value;

        firstBlock.GetComponentInChildren<Renderer>().material.color = curBlockColor;

        changingChannel = Random.Range(0, 3); // 0 = r, 1 = g, 2 = b
        isColorInc = Random.value < 0.5f ? -1 : 1;

        isFirstColorSet = true;
    }


    private void UpdateBackgroundColor()
    {
        Color targetColor = new Color(
            curBlockColor.r * 0.3f,
            curBlockColor.g * 0.3f,
            curBlockColor.b * 0.3f
        );
        Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, targetColor, Time.deltaTime * backgroundLerpSpeed);
    }

    private void HandleStreak()
    {
        if (streak >= 8)
        {
            Vector3 scale = currentBlock.transform.localScale;

            // Yeni boyutlarý 2f'i aþmayacak þekilde hesapla
            float newX = Mathf.Min(scale.x + streakIncreasment, 2f);
            float newZ = Mathf.Min(scale.z + streakIncreasment, 2f);

            // Artýþ miktarýný bul
            float increaseX = newX - scale.x;
            float increaseZ = newZ - scale.z;

            // Artýþ varsa pozisyonu yarýsý kadar kaydýr
            currentBlock.transform.position += new Vector3(increaseX / 2f, 0f, -increaseZ / 2f);

            // Yeni boyutu uygula
            currentBlock.transform.localScale = new Vector3(newX, scale.y, newZ);
        }
    }

    private void PlayPerfectAudio()
    {
        // streak'e göre nota seç
        perfectAudioIndex = streak;
        if (perfectAudioIndex >= noteSounds.Length)
            perfectAudioIndex = noteSounds.Length - 1; // Dizinin dýþýna çýkmayý önler

        // Ses çal
        if (noteSounds[perfectAudioIndex] != null && audioSource != null)
        {
            audioSource.PlayOneShot(noteSounds[perfectAudioIndex]);
        }
    }

    private void PlayNormalAudio()
    {
        if (normalAudioIndex >= noteSoundsDoubleBas.Length)
            normalAudioIndex = 0; // Dizinin dýþýna çýkmayý önler

        // Ses çal
        if (noteSoundsDoubleBas[normalAudioIndex] != null && audioSource != null)
        {
            audioSource.PlayOneShot(noteSoundsDoubleBas[normalAudioIndex]);
        }

        normalAudioIndex++;
    }

    public bool GetIsFirstColorSet()
    {
        return isFirstColorSet;
    }

    public void SetIsFirstColorSet(bool newVal)
    {
        isFirstColorSet = newVal;
    }

    public void SetInputDetected(bool newVal)
    {
        inputDetected = newVal;
    }

    public void ResetHighScore()
    {
        highScore = 0;
        PlayerPrefs.SetInt("HighScore", 0);
        PlayerPrefs.Save();
    }

    public int GetCreatedBlocks()
    {
        return blocksCreated;
    }

    private void SpawnPulseEffect(Vector3 position, Vector3 scale)
    {
        if (pulseEffect != null)
        {
            GameObject pulse = Instantiate(pulseEffect, PulseSpawnPoint(), Quaternion.identity);
            pulse.transform.localScale = scale;
        }
    }

    private Vector3 PulseSpawnPoint()
    {
        Vector3 pulseSpawnPoint = new Vector3(prevBlockPos.x - prevBlock.transform.localScale.x / 2f, spawnpoint.y - blockY, prevBlockPos.z + prevBlock.transform.localScale.z / 2f);

        return pulseSpawnPoint;

    }
}
