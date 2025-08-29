using UnityEngine;

public class PulseScript : MonoBehaviour
{
    private Vector3 startScale;
    private Vector3 targetScale;
    [SerializeField] private float speed = 2f;      // büyüme hýzý
    [SerializeField] private float fadeSpeed = 2f;  // alpha düþme hýzý
    [SerializeField] private Renderer childRenderer; // child renderer (Inspector’dan ata)

    private float currentAlpha = 1f;

    private void Awake()
    {
        // Prev block'un scale'ýný al
        Vector3 prevScale = BlockSpawner.instance.GetPrevBlockScale();

        // sadece X ve Z al, Y sabit 1
        startScale = new Vector3(prevScale.x * 0.5f, 1f, prevScale.z * 0.5f);  // yarýsý
        targetScale = new Vector3(prevScale.x * 2f, 1f, prevScale.z * 2f);

        transform.localScale = startScale;

        if (childRenderer != null)
        {
            currentAlpha = childRenderer.material.color.a;
        }

        Debug.Log("start scale: " + startScale + " target scale: " + targetScale);
    }

    private void Update()
    {
        // X ve Z'yi büyüt
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * speed);

        // Alfa deðerini azalt
        if (childRenderer != null)
        {
            currentAlpha = Mathf.Lerp(currentAlpha, 0f, Time.deltaTime * fadeSpeed);

            Color c = childRenderer.material.color;
            c.a = currentAlpha;
            childRenderer.material.color = c;

            // Yok olma þartý -> alpha çok küçülünce
            if (currentAlpha <= 0.01f)
            {
                Destroy(gameObject);
            }
        }
    }
}
