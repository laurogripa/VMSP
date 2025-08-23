using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceManager : MonoBehaviour
{
    [SerializeField] private List<int> localSequence;
    [SerializeField] private SequenceAnimation[] sequenceAnimation;
    public List<int> globalSequence;
    public bool isClickable;

    void Start()
    {   
        isClickable = true;
        localSequence = new List<int>();
        globalSequence = new List<int>();
        StartCoroutine(NextLevel());
    }

    private void FixedUpdate()
    {
        if(IsTwinSequence())
        {
            StartCoroutine(NextLevel());
        }
        else
        {
            ResetGame();
        }
    }

    private void AddSequence()
    {
        localSequence.Add(Random.Range(0, 4));
    }

    private void ResetGame()
    {
        globalSequence.Clear();
        localSequence.Clear();
        StartCoroutine(NextLevel());
    }

    private IEnumerator NextLevel()
    {
        if(globalSequence.Count == localSequence.Count)
        {
            globalSequence.Clear();
            AddSequence();
            gameObject.GetComponent<RecordScore>().SetScore(localSequence.Count - 1);
            isClickable = false;
            for(int i = 0; i < localSequence.Count; i++)
            {
                if (i == 0)
                {
                    yield return StartCoroutine(sequenceAnimation[localSequence[i]].RunAnimation(false, localSequence[i]));
                }
                if (localSequence.Count > 1)
                {
                    if(i != localSequence.Count - 1)
                    {
                        if (sequenceAnimation[localSequence[i]] != sequenceAnimation[localSequence[i + 1]])
                        {
                            yield return StartCoroutine(sequenceAnimation[localSequence[i]].RunAnimation(false, localSequence[i + 1]));
                            yield return StartCoroutine(sequenceAnimation[localSequence[i + 1]].RunAnimation(true, localSequence[i]));
                        }
                        else
                        {
                            yield return StartCoroutine(sequenceAnimation[localSequence[i]].RunAnimation(true, localSequence[i]));
                            yield return StartCoroutine(sequenceAnimation[localSequence[i]].RunAnimation(false, localSequence[i]));
                        }
                    }
                }
                if (i == localSequence.Count - 1)
                {
                    yield return StartCoroutine(sequenceAnimation[localSequence[i]].RunAnimation(true, localSequence[i]));
                }

            }
            isClickable = true;
        }
    }

    private bool IsTwinSequence()
    {
        bool isEqualUntilNow = true;
        for(int i = 0; i < globalSequence.Count; i++)
        {
            if(globalSequence[i] != localSequence[i])
            {
                isEqualUntilNow = false;
            }
        }
        return isEqualUntilNow;
    }
}
