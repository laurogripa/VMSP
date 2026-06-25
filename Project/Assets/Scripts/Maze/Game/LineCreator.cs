using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineCreator : MonoBehaviour
{
    public GameObject linePrefab, currentLine;
    private Transform lineBox;
    private ManageControls controls;
    private S2LifeManager lifeManager;
    private Checkpoint[] checkpoints;
    private LineDestroyer[] lineDestroyers;
    public LineRenderer lineRenderer;
    public EdgeCollider2D edgeCollider;
    public List<Vector2> fingerPositions;
    public List<GameObject> successLines;
    public Material lineErrorMaterial, lineSuccessMaterial;
    public bool destroyLine, waiting;
    [SerializeField]
    private float waitedTime;

    private void Start()
    {
        controls = GameObject.Find("Neuron").GetComponent<ManageControls>();
        lineBox = GameObject.Find("Line Box").GetComponent<Transform>();
        lifeManager = FindObjectOfType<S2LifeManager>(true);
        checkpoints = FindObjectsOfType<Checkpoint>(true);
        lineDestroyers = FindObjectsOfType<LineDestroyer>(true);
    }

    void Update()
    {
        if(!BackAction.onPause)
        {
            if (!controls.showCanvas)
            {
                if (!waiting && controls.allowDraw)
                {
                    if (currentLine == null)
                    {
                        CreateLine();
                    }

                    Vector2 tempFingerPos = GetLocalPointerPosition();
                    if (Vector2.Distance(tempFingerPos, fingerPositions[fingerPositions.Count - 1]) > .1f)
                    {
                        UpdateLine(tempFingerPos);
                    }

                    if (destroyLine)
                    {
                        string newRegistry = PlayerPrefs.GetString("PlayerRegistry");
                        newRegistry += "ⓧ Colisão Detectada\n";
                        PlayerPrefs.SetString("PlayerRegistry", newRegistry);
                        currentLine.GetComponent<Renderer>().material = lineErrorMaterial;
                        lifeManager.ChangeLife(true);
                        destroyLine = false;
                        waiting = true;
                        controls.ResetCharge();
                    }
                }
                else if (currentLine != null || waiting)
                {
                    waitedTime += Time.deltaTime;
                    if (waitedTime >= 0.25f)
                    {
                        Destroy(currentLine);
                        currentLine = null;
                        waitedTime = 0;
                        waiting = false;
                        controls.ResetCharge();
                    }
                }
                else
                {
                    waitedTime = 0f;
                }
            }
        }
        else if(currentLine != null)
        {
            Destroy(currentLine);
            controls.ResetCharge();
        }
    }
    void CreateLine()
    {
        if(!waiting)
        {
            currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
            currentLine.name = "Line";
            currentLine.transform.SetParent(gameObject.transform, false);
            lineRenderer = currentLine.GetComponent<LineRenderer>();
            edgeCollider = currentLine.GetComponent<EdgeCollider2D>();
            lineRenderer.useWorldSpace = false;
            fingerPositions.Clear();
            Vector2 pointerPosition = GetLocalPointerPosition();
            fingerPositions.Add(pointerPosition);
            fingerPositions.Add(pointerPosition);
            lineRenderer.SetPosition(0, fingerPositions[0]);
            lineRenderer.SetPosition(1, fingerPositions[1]);
            UpdateEdgeCollider();
        }
    }
    void UpdateLine(Vector2 newFingerPos)
    {
        if (!waiting && lineRenderer != null)
        {
            fingerPositions.Add(newFingerPos);
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, newFingerPos);
            UpdateEdgeCollider();
        }
    }

    private void UpdateEdgeCollider()
    {
        edgeCollider.points = fingerPositions.ToArray();
        Physics2D.SyncTransforms();
        CheckCheckpointOverlap();
        CheckHazardOverlap();
    }

    private Vector2 GetLocalPointerPosition()
    {
        Vector3 pointerWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return transform.InverseTransformPoint(pointerWorldPosition);
    }

    public void SuccessLine(int id)
    {
        GameObject newCorrectLine = Instantiate(currentLine);
        newCorrectLine.transform.SetParent(lineBox, false);
        newCorrectLine.transform.localPosition = Vector3.zero;
        newCorrectLine.transform.localRotation = Quaternion.identity;
        newCorrectLine.transform.localScale = Vector3.one;
        newCorrectLine.GetComponent<Renderer>().material = lineSuccessMaterial;
        newCorrectLine.GetComponent<Renderer>().sortingOrder = -1;
        newCorrectLine.GetComponent<SetEffect>().enabled = true;
        newCorrectLine.GetComponent<SetEffect>().buttonID = id;
        successLines.Add(newCorrectLine);
        Destroy(currentLine);
        controls.ResetCharge();
    }

    public bool IsAutoDrawActive()
    {
        return controls.allowDraw;
    }

    private void CheckCheckpointOverlap()
    {
        if (currentLine == null || edgeCollider == null || !edgeCollider.enabled)
        {
            return;
        }

        for (int i = 0; i < checkpoints.Length; i++)
        {
            Collider2D checkpointCollider = checkpoints[i].GetComponent<Collider2D>();
            if (checkpointCollider != null && edgeCollider.Distance(checkpointCollider).isOverlapped)
            {
                currentLine.GetComponent<Rigidbody2D>().isKinematic = true;
                edgeCollider.enabled = false;
                SuccessLine(checkpoints[i].GetButtonId());
                return;
            }
        }
    }

    private void CheckHazardOverlap()
    {
        if (currentLine == null || edgeCollider == null || waiting || destroyLine)
        {
            return;
        }

        for (int i = 0; i < lineDestroyers.Length; i++)
        {
            Collider2D hazardCollider = lineDestroyers[i].GetComponent<Collider2D>();
            if (hazardCollider != null && edgeCollider.Distance(hazardCollider).isOverlapped)
            {
                destroyLine = true;
                return;
            }
        }
    }

}
