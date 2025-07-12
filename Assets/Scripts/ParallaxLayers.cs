using UnityEngine;

public class ParallaxLayers : MonoBehaviour
{
    [Header("Scroll Settings")]
    public float scrollSpeed = 2f;

    private float tileWidth;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        tileWidth = GetVisualWidth(gameObject); // Calculate width of entire visual
    }

    void Update()
    {
        // Scroll background to the left
        transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

        // When completely offscreen, move the tile to the right to loop
        if (transform.position.x <= startPosition.x - tileWidth)
        {
            transform.position += Vector3.right * tileWidth;
        }
    }

    // Calculates total width of this object's rendered visuals
    float GetVisualWidth(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return 0f;

        Bounds bounds = renderers[0].bounds;

        // Expand bounds to include all child renderers
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        return bounds.size.x;
    }
}
