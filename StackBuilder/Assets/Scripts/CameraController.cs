using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float camSpeed = 2f;

    private Vector3 targetPosition;

    private void Awake()
    {
        
        targetPosition = transform.position;
    }

    private void Update()
    {
        // Kamera hedef pozisyona yumuþak geçiþ yapsýn
        transform.position = Vector3.Lerp(transform.position, targetPosition, camSpeed * Time.deltaTime);
    }

    // Bu fonksiyonu blok yerleþtirildiðinde çaðýr
    public void MoveCameraUp(float yAmount)
    {
        targetPosition += new Vector3(0f, yAmount, 0f);
    }

}
