using UnityEngine;

public class RandomizeColor : MonoBehaviour
{
    private void Awake()
    {
        // Child içindeki Renderer'ý bul
        Renderer childRenderer = GetComponentInChildren<Renderer>();

        if (childRenderer != null)
        {
            // Rastgele renk oluþtur (Tam opak)
            Color randomColor = new Color(Random.value, Random.value, Random.value, 1f);

            // Rengi uygula
            childRenderer.material.color = randomColor;
        }
        else
        {
            Debug.LogWarning("Child Renderer bulunamadý!");
        }
    }
}
