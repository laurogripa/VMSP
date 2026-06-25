using UnityEngine;

public class NeuronElectricEffect : MonoBehaviour
{
    private const int ArcCount = 3;
    private const int OutlinePointCount = 40;
    private const float OutlinePadding = 1.08f;

    private PolygonCollider2D sourceCollider;
    private LineRenderer[] arcs;
    private Vector2[] baseOutline;
    private Vector2[] animatedOutline;
    private Material lineMaterial;
    private int sortingLayerId;
    private int sortingOrder;
    private float animationSeed;
    private bool isActive;

    public void Initialize(PolygonCollider2D collider, int layerId, int renderOrder)
    {
        sourceCollider = collider;
        sortingLayerId = layerId;
        sortingOrder = renderOrder;
        animationSeed = Random.Range(0f, 1000f);
        BuildOutline();
        EnsureArcs();
        SetActive(false);
    }

    public void Play()
    {
        if (sourceCollider == null || baseOutline == null || baseOutline.Length < 3)
        {
            return;
        }

        EnsureArcs();
        isActive = true;
        gameObject.SetActive(true);
        UpdateArcVisuals();
    }

    public void Stop()
    {
        isActive = false;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        UpdateArcVisuals();
    }

    private void BuildOutline()
    {
        if (sourceCollider == null || sourceCollider.pathCount == 0)
        {
            return;
        }

        Vector2[] path = sourceCollider.GetPath(0);
        if (path.Length < 3)
        {
            return;
        }

        baseOutline = new Vector2[OutlinePointCount];
        for (int i = 0; i < OutlinePointCount; i++)
        {
            float t = i / (float)OutlinePointCount;
            int index = Mathf.FloorToInt(t * path.Length) % path.Length;
            baseOutline[i] = path[index] * OutlinePadding;
        }

        animatedOutline = new Vector2[OutlinePointCount];
    }

    private void EnsureArcs()
    {
        if (arcs != null && arcs.Length == ArcCount)
        {
            return;
        }

        if (lineMaterial == null)
        {
            lineMaterial = new Material(Shader.Find("Sprites/Default"));
        }

        arcs = new LineRenderer[ArcCount];
        for (int i = 0; i < ArcCount; i++)
        {
            GameObject arcObject = new GameObject("ElectricArc_" + i);
            arcObject.transform.SetParent(transform, false);

            LineRenderer lineRenderer = arcObject.AddComponent<LineRenderer>();
            lineRenderer.useWorldSpace = false;
            lineRenderer.loop = true;
            lineRenderer.material = lineMaterial;
            lineRenderer.sortingLayerID = sortingLayerId;
            lineRenderer.sortingOrder = sortingOrder + i;
            lineRenderer.numCapVertices = 4;
            lineRenderer.numCornerVertices = 2;
            lineRenderer.textureMode = LineTextureMode.Stretch;
            lineRenderer.alignment = LineAlignment.TransformZ;
            lineRenderer.positionCount = OutlinePointCount;
            arcs[i] = lineRenderer;
        }
    }

    private void UpdateArcVisuals()
    {
        if (baseOutline == null || arcs == null)
        {
            return;
        }

        float time = Time.time;
        for (int arcIndex = 0; arcIndex < arcs.Length; arcIndex++)
        {
            LineRenderer arc = arcs[arcIndex];
            float arcSeed = animationSeed + arcIndex * 17.13f;
            float arcSpeed = 10f + arcIndex * 2.5f;
            float arcJitter = 2.4f + arcIndex * 0.8f;
            float arcWidth = 2.8f - arcIndex * 0.55f;
            float pulse = 0.65f + 0.35f * Mathf.PingPong(time * (4.5f + arcIndex) + arcSeed, 1f);

            for (int pointIndex = 0; pointIndex < baseOutline.Length; pointIndex++)
            {
                Vector2 basePoint = baseOutline[pointIndex];
                Vector2 previous = baseOutline[(pointIndex - 1 + baseOutline.Length) % baseOutline.Length];
                Vector2 next = baseOutline[(pointIndex + 1) % baseOutline.Length];
                Vector2 tangent = (next - previous).normalized;
                Vector2 normal = new Vector2(-tangent.y, tangent.x);

                float normalNoise = Mathf.PerlinNoise(arcSeed + pointIndex * 0.21f, time * arcSpeed);
                float tangentNoise = Mathf.PerlinNoise(arcSeed + 50f + pointIndex * 0.17f, time * (arcSpeed * 0.85f));
                float normalOffset = (normalNoise - 0.5f) * 2f * arcJitter;
                float tangentOffset = (tangentNoise - 0.5f) * arcJitter * 0.35f;

                animatedOutline[pointIndex] = basePoint + normal * normalOffset + tangent * tangentOffset;
            }

            Color coreColor = arcIndex == 0
                ? new Color(0.75f, 0.95f, 1f, 0.95f * pulse)
                : new Color(0.45f, 0.78f, 1f, (0.55f - arcIndex * 0.12f) * pulse);

            arc.startColor = coreColor;
            arc.endColor = coreColor;
            arc.startWidth = arcWidth * pulse;
            arc.endWidth = arcWidth * pulse * 0.85f;
            arc.SetPositions(System.Array.ConvertAll(animatedOutline, v => (Vector3)v));
        }
    }

    private void SetActive(bool active)
    {
        isActive = active;
        gameObject.SetActive(active);
    }

    private void OnDestroy()
    {
        if (lineMaterial != null)
        {
            Destroy(lineMaterial);
        }
    }
}
