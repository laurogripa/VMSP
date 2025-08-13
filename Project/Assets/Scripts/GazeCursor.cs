using UnityEngine;

public class GazeCursor : MonoBehaviour
{
    [Header("Cursor Settings")]
    public float fadeSpeed = 5f;
    public float scaleSpeed = 2f;
    public float minScale = 0.5f;
    public float maxScale = 1.5f;
    
    private SpriteRenderer spriteRenderer;
    private Vector3 baseScale;
    private float alpha = 1f;
    private bool isVisible = true;
    
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseScale = transform.localScale;
        
        // Create a simple circle cursor if no sprite is assigned
        if (spriteRenderer != null && spriteRenderer.sprite == null)
        {
            CreateDefaultCursor();
        }
    }
    
    private void Update()
    {
        // Animate the cursor with a pulsing effect
        float scale = minScale + (maxScale - minScale) * (0.5f + 0.5f * Mathf.Sin(Time.time * scaleSpeed));
        transform.localScale = baseScale * scale;
        
        // Handle visibility
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
    }
    
    public void SetVisibility(bool visible)
    {
        isVisible = visible;
        alpha = visible ? 1f : 0f;
    }
    
    public void FadeIn()
    {
        isVisible = true;
        alpha = Mathf.Lerp(alpha, 1f, fadeSpeed * Time.deltaTime);
    }
    
    public void FadeOut()
    {
        isVisible = false;
        alpha = Mathf.Lerp(alpha, 0f, fadeSpeed * Time.deltaTime);
    }
    
    private void CreateDefaultCursor()
    {
        // Create a simple white circle texture
        Texture2D texture = new Texture2D(32, 32);
        Vector2 center = new Vector2(16, 16);
        
        for (int x = 0; x < 32; x++)
        {
            for (int y = 0; y < 32; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                if (distance <= 15f && distance >= 12f)
                {
                    texture.SetPixel(x, y, Color.white);
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }
        
        texture.Apply();
        Sprite cursorSprite = Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
        spriteRenderer.sprite = cursorSprite;
    }
}