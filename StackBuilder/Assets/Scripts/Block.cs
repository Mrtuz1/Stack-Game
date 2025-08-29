using UnityEngine;

public class Block : MonoBehaviour
{
    public float speed = 3f;
    public bool isPlaced;
    public bool isInverted;
    private int order;
    private float zPos = 0.75f;
    private float xPos = 0.75f;
    private float yPos;
    private int isReturning;
    private Vector3 curDestination;
    private Rigidbody rb;
    private Vector3 prevBlockPos;
    private Vector3 prevBlockScale;

    private void Awake()
    {
        prevBlockPos = BlockSpawner.instance.GetPrevBlockPos();
        isInverted = BlockSpawner.instance.GetIsInverted();
        yPos = BlockSpawner.instance.GetSpawnPointY();
        prevBlockScale = BlockSpawner.instance.GetPrevBlockScale();

        transform.localScale = prevBlockScale;
        rb = GetComponent<Rigidbody>();
        isReturning = -1;
        isPlaced = false;
        rb.useGravity = false;
        UpdateDestination();
    }

    private void Update()
    {
        if (!isPlaced)
        {
            // Mevcut pozisyondan hedefe doðru hareket et
            transform.position = Vector3.MoveTowards(transform.position, curDestination, speed * Time.deltaTime);

            // Hedefe ulaþýldý mý?
            if (Vector3.Distance(transform.position, curDestination) < 0.01f)
            {
                // Gidiþ yönünü deðiþtir
                isReturning = -isReturning;
                UpdateDestination();
            }
        }

        if (isPlaced)
        {
            if(order + 20 < BlockSpawner.instance.GetCreatedBlocks())
            {
                Destroy(this.gameObject);
            }
        }
    }
    
    private void UpdateDestination()
    {
        if (!isInverted)
        {
            float prevBlockMidPoint = prevBlockPos.x;
            float destinationDistance = (xPos + prevBlockScale.x);
            // X ekseninde hareket (Z sabit)
            curDestination = new Vector3(prevBlockMidPoint - (destinationDistance * isReturning), yPos, prevBlockPos.z);
        }
        else
        {
            float prevBlockMidPoint = prevBlockPos.z;
            float destinationDistance = (zPos + prevBlockScale.z);
            // Z ekseninde hareket (X sabit)
            curDestination = new Vector3(prevBlockPos.x, yPos, prevBlockMidPoint + (destinationDistance * isReturning));
        }
    }

    public void SetOrder(int newOrder)
    {
        order = newOrder;
    }
}
