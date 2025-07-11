using UnityEngine;

public class ParallaxLayers : MonoBehaviour
{
    public float scrollSpeed = 2f;
    private float tileWidth;
    private Vector3 startPosition;
    
    void Start()
    {
        startPosition = transform.position;
        tileWidth = GetVisualWidth(gameObject);

    }

    void Update()
    {
        transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

        // If tile moved completely offscreen to the left, move it to the right
        if (transform.position.x <= startPosition.x - tileWidth)
        {
            transform.position += Vector3.right * tileWidth;
        }
    }

    float GetVisualWidth(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return 0f;

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        return bounds.size.x;
    }
}