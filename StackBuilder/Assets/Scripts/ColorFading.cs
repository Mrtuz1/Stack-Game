using UnityEngine;

public class ColorFading : MonoBehaviour
{
    [SerializeField] Renderer blockRenderer;
    private float fadeSpeed = 2f; // saniyede ne kadar azalacak

    private Color currentColor;

    private void Awake()
    {
        currentColor = BlockSpawner.instance.GetCurColor();
        currentColor.a = 1f;
        blockRenderer.material.color = currentColor;
    }

    void Update()
    {
        FadeColor();
    }

    private void FadeColor()
    {
        // Alfa deðerini azalt
        currentColor.a -= fadeSpeed * Time.deltaTime;

        // Yeni rengi uygula
        blockRenderer.material.color = currentColor;

        // Alfa sýfýra veya altýna düþünce yok et
        if (currentColor.a <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
