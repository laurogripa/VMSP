using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public bool[] isLoaded;

    void Awake()
    {
        isLoaded = new bool[4];
        if (GameObject.FindGameObjectsWithTag("Sprite_Holder").Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
    private void Update()
    {
        if(AllLoaded())
        {
            GameObject.Find("Main Camera").GetComponent<StageScreen>().GenLoad();
            this.enabled = false;
        }
    }

    private bool AllLoaded()
    {
        bool result = true;
        for(int i = 0; i < isLoaded.Length; i++)
        {
            if(!isLoaded[i])
            {
                result = false;
            }
        }
        return result;
    }
}
