using UnityEngine;

public class Food : MonoBehaviour
{
    [SerializeField] BoxCollider2D gridArea;
    [SerializeField] LayerMask collisionLayer; // Layer to check for collisions
    [SerializeField] bool isStatic = false;

    private void Start()
    {
        if (!isStatic)
            RandomizePosition();
    }

    private void RandomizePosition()
    {
        bool positionFound = false;
        Vector3 newPosition = Vector3.zero;

        // Try multiple times to find a non-colliding position
        int maxAttempts = 100;
        for (int i = 0; i < maxAttempts; i++)
        {
            newPosition = GetRandomPosition();
            if (!IsPositionColliding(newPosition))
            {
                positionFound = true;
                break;
            }
        }

        if (positionFound)
        {
            this.transform.position = newPosition;
        }
        else
        {
            Debug.LogWarning("No non-colliding position found for food after " + maxAttempts + " attempts.");
        }
    }

    private Vector3 GetRandomPosition()
    {
        Bounds bounds = gridArea.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector3(Mathf.Round(x), Mathf.Round(y), 0.0f);
    }

    private bool IsPositionColliding(Vector3 position)
    {
        // Adjust the size according to your game's requirements
        float checkRadius = 1.0f;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(position, checkRadius, collisionLayer);
        return hitColliders.Length > 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            isStatic = false;
            RandomizePosition();
        }
    }

    //private void Start()
    //{
    //    RandomizePosition();
    //}

    //private void RandomizePosition()
    //{
    //    Bounds bounds = this.gridArea.bounds;

    //    float x = Random.Range(bounds.min.x, bounds.max.x);
    //    float y = Random.Range(bounds.min.y, bounds.max.y);

    //    this.transform.position = new Vector3(Mathf.Round(x), Mathf.Round(y), 0.0f);
    //}

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.tag == "Player")
    //    {
    //        RandomizePosition();
    //    }
    //}

    //[SerializeField] BoxCollider2D gridArea;

}


