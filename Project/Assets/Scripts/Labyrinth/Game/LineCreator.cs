using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineCreator : MonoBehaviour
{
    public GameObject linePrefab, currentLine;
    private Transform lineBox;
    private ManageControls controls;
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
    }

    void Update()
    {
        if(!BackAction.onPause)
        {
            if (!controls.showCanvas)
            {
                if (!waiting && controls.allowDraw || !waiting && !controls.allowDraw && GameObject.Find("Line") != null)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        CreateLine();
                    }
                    if (Input.GetMouseButton(0))
                    {
                        Vector2 tempFingerPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        if (Vector2.Distance(tempFingerPos, fingerPositions[fingerPositions.Count - 1]) > .1f)
                        {
                            UpdateLine(tempFingerPos);
                        }
                    }

                    if (destroyLine)
                    {
                        string newRegistry = PlayerPrefs.GetString("PlayerRegistry");
                        newRegistry += "ⓧ Colisão Detectada\n";
                        PlayerPrefs.SetString("PlayerRegistry", newRegistry);
                        currentLine.GetComponent<Renderer>().material = lineErrorMaterial;
                        GameObject.Find("Lifes").GetComponent<S2LifeManager>().ChangeLife(true);
                        destroyLine = false;
                        waiting = true;
                    }
                }
                else
                {
                    waitedTime += Time.deltaTime;
                    if (waitedTime >= 0.25f)
                    {
                        Destroy(currentLine);
                        waitedTime = 0;
                        waiting = false;
                    }
                }
            }
        }
        else if(currentLine != null)
        {
            Destroy(currentLine);
        }
    }
    void CreateLine()
    {
        if(!waiting)
        {
            currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
            currentLine.name = "Line";
            currentLine.transform.SetParent(gameObject.transform);
            lineRenderer = currentLine.GetComponent<LineRenderer>();
            edgeCollider = currentLine.GetComponent<EdgeCollider2D>();
            fingerPositions.Clear();
            fingerPositions.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            fingerPositions.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            lineRenderer.SetPosition(0, fingerPositions[0]);
            lineRenderer.SetPosition(1, fingerPositions[1]);
            edgeCollider.points = fingerPositions.ToArray();
        }
    }
    void UpdateLine(Vector2 newFingerPos)
    {
        if (!waiting && lineRenderer != null)
        {
            fingerPositions.Add(newFingerPos);
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, newFingerPos);
            edgeCollider.points = fingerPositions.ToArray();
        }
    }

    public void SuccessLine(int id)
    {
        GameObject newCorrectLine = Instantiate(currentLine);
        newCorrectLine.transform.SetParent(lineBox);
        newCorrectLine.GetComponent<Renderer>().material = lineSuccessMaterial;
        newCorrectLine.GetComponent<Renderer>().sortingOrder = -1;
        newCorrectLine.GetComponent<SetEffect>().enabled = true;
        newCorrectLine.GetComponent<SetEffect>().buttonID = id;
        successLines.Add(newCorrectLine);
        Destroy(currentLine);
    }
    
    
}