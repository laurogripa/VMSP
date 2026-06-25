using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageControls : MonoBehaviour
{
    private const float HoverThreshold = 1f;

    public bool allowDraw;
    public bool showCanvas;
    private float time;
    private float hoverTime;
    private bool isCharged;
    private Vector3 baseScale;
    private Quaternion baseRotation;
    private SpriteRenderer spriteRenderer;
    private Color baseColor;
    private Collider2D hitCollider;
    private Transform animatedVisual;

    void Start()
    {
        animatedVisual = transform;
        baseScale = animatedVisual.localScale;
        baseRotation = animatedVisual.localRotation;
        hitCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            baseColor = spriteRenderer.color;
            GameObject visual = new GameObject("NeuronVisual");
            animatedVisual = visual.transform;
            animatedVisual.SetParent(transform, false);
            animatedVisual.localPosition = Vector3.zero;
            animatedVisual.localRotation = Quaternion.identity;
            animatedVisual.localScale = Vector3.one;

            SpriteRenderer visualRenderer = visual.AddComponent<SpriteRenderer>();
            visualRenderer.sprite = spriteRenderer.sprite;
            visualRenderer.color = spriteRenderer.color;
            visualRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
            visualRenderer.sortingOrder = spriteRenderer.sortingOrder;
            visualRenderer.material = spriteRenderer.material;
            spriteRenderer.enabled = false;
            spriteRenderer = visualRenderer;
            baseScale = animatedVisual.localScale;
            baseRotation = animatedVisual.localRotation;
        }

        allowDraw = false;
        showCanvas = true;
    }

    private void Update()
    {
        if (BackAction.onPause)
        {
            return;
        }

        EnsureVisualState();

        if (showCanvas)
        {
            time += Time.deltaTime;
            if (time >= 0.1f)
            {
                showCanvas = false;
                time = 0;
            }
            return;
        }

        bool isHovering = IsPointerOverNeuron();

        if (allowDraw)
        {
            ApplyChargedVisual();
            return;
        }

        Transform currentVisual = GetAnimatedVisual();

        if (!isCharged)
        {
            hoverTime = isHovering ? Mathf.Min(hoverTime + Time.deltaTime, HoverThreshold) : 0f;
            float hoverT = Mathf.Clamp01(hoverTime / HoverThreshold);
            float jitter = Mathf.Sin(Time.time * 40f) * 6f * hoverT;
            currentVisual.localRotation = baseRotation * Quaternion.Euler(0f, 0f, jitter);
            currentVisual.localScale = baseScale * (1f + 0.12f * hoverT);

            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.Lerp(baseColor, Color.white, 0.65f * hoverT);
            }

            if (hoverTime >= HoverThreshold)
            {
                isCharged = true;
                ApplyChargedVisual();
            }
            return;
        }

        ApplyChargedVisual();
        if (!isHovering)
        {
            allowDraw = true;
        }
    }

    public void ResetCharge()
    {
        EnsureVisualState();
        allowDraw = false;
        isCharged = false;
        hoverTime = 0f;
        Transform currentVisual = GetAnimatedVisual();
        currentVisual.localScale = baseScale;
        currentVisual.localRotation = baseRotation;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = baseColor;
        }
    }

    public Vector2 GetStartLocalPosition(Transform relativeTo)
    {
        return relativeTo.InverseTransformPoint(transform.position);
    }

    private bool IsPointerOverNeuron()
    {
        if (hitCollider == null || Camera.main == null)
        {
            return false;
        }

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return hitCollider.OverlapPoint(new Vector2(mouseWorld.x, mouseWorld.y));
    }

    private void EnsureVisualState()
    {
        if (hitCollider == null)
        {
            hitCollider = GetComponent<Collider2D>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                baseColor = spriteRenderer.color;
            }
        }
    }

    private void ApplyChargedVisual()
    {
        Transform currentVisual = GetAnimatedVisual();
        currentVisual.localRotation = baseRotation;
        currentVisual.localScale = baseScale * 1.12f;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.Lerp(baseColor, Color.white, 0.65f);
        }
    }

    private Transform GetAnimatedVisual()
    {
        if (animatedVisual == null)
        {
            animatedVisual = transform.childCount > 0 ? transform.GetChild(0) : transform;
            baseScale = animatedVisual.localScale;
            baseRotation = animatedVisual.localRotation;
        }

        return animatedVisual;
    }
}
