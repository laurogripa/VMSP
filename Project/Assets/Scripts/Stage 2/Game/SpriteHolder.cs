using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteHolder : MonoBehaviour
{
    private bool working;
    [SerializeField]
    private Slider slider;
    public Sprite[] to0, to1, to2, to3;
    private int idLoading, myID, index, proportion;
    private void Awake()
    {
        switch (gameObject.name.Substring(0, gameObject.name.Length - 6))
        {
            case "Blue":
                myID = 0;
                to0 = new Sprite[36];
                to1 = new Sprite[57];
                to2 = new Sprite[51];
                to3 = new Sprite[45];
                proportion = 189;
                break;
            case "Green":
                myID = 1;
                to1 = new Sprite[36];
                to0 = new Sprite[57];
                to2 = new Sprite[45];
                to3 = new Sprite[54];
                proportion = 192;
                break;
            case "Red":
                myID = 2;
                to2 = new Sprite[32];
                to1 = new Sprite[41];
                to0 = new Sprite[47];
                to3 = new Sprite[50];
                proportion = 170;
                break;
            case "Yellow":
                myID = 3;
                to3 = new Sprite[60];
                to1 = new Sprite[96];
                to2 = new Sprite[87];
                to0 = new Sprite[69];
                proportion = 312;
                break;
        }
    }

    void Start()
    {
        working = true;
        slider = GameObject.Find("Main Camera").GetComponent<StageScreen>().slider;
        slider.gameObject.SetActive(true);
    }

    void FixedUpdate()
    {
        if(working)
        {
            if (idLoading < 4)
            {
                if (idLoading == myID)
                {
                    switch (idLoading)
                    {
                        case 0:
                            if (index < 9)
                            {
                                to0[index] = Resources.Load("Stage 2/" + gameObject.name.Substring(0, gameObject.name.Length - 6) + "/toRed/0" + (index + 1), typeof(Sprite)) as Sprite;
                            }
                            else
                            {
                                to0[index] = Resources.Load("Stage 2/" + gameObject.name.Substring(0, gameObject.name.Length - 6) + "/toRed/" + (index + 1), typeof(Sprite)) as Sprite;
                            }
                            index += 1;
                            slider.value += 0.25f / proportion;
                            if (index > 35)
                            {
                                NextLoad();
                            }
                            break;
                        case 1:
                            if (index < 9)
                            {
                                to1[index] = Resources.Load("Stage 2/" + gameObject.name.Substring(0, gameObject.name.Length - 6) + "/toRed/0" + (index + 1), typeof(Sprite)) as Sprite;
                            }
                            else
                            {
                                to1[index] = Resources.Load("Stage 2/" + gameObject.name.Substring(0, gameObject.name.Length - 6) + "/toRed/" + (index + 1), typeof(Sprite)) as Sprite;
                            }
                            index += 1;
                            slider.value += 0.25f / proportion;
                            if (index > 35)
                            {
                                NextLoad();
                            }
                            break;
                        case 2:
                            if (index < 9)
                            {
                                to2[index] = Resources.Load("Stage 2/" + gameObject.name.Substring(0, gameObject.name.Length - 6) + "/toYellow/0" + (index + 1), typeof(Sprite)) as Sprite;
                            }
                            else
                            {
                                to2[index] = Resources.Load("Stage 2/" + gameObject.name.Substring(0, gameObject.name.Length - 6) + "/toYellow/" + (index + 1), typeof(Sprite)) as Sprite;
                            }
                            index += 1;
                            slider.value += 0.25f / proportion;
                            if (index > 31)
                            {
                                NextLoad();
                            }
                            break;
                        case 3:
                            if (index < 9)
                            {
                                to3[index] = Resources.Load("Stage 2/" + gameObject.name.Substring(0, gameObject.name.Length - 6) + "/toRed/0" + (index + 1), typeof(Sprite)) as Sprite;
                            }
                            else
                            {
                                to3[index] = Resources.Load("Stage 2/" + gameObject.name.Substring(0, gameObject.name.Length - 6) + "/toRed/" + (index + 1), typeof(Sprite)) as Sprite;
                            }
                            index += 1;
                            slider.value += 0.25f / proportion;
                            if (index > 59)
                            {
                                NextLoad();
                            }
                            break;
                    }
                }
                else
                {
                    switch (idLoading)
                    {
                        case 0:
                            if (index < 9)
                            {
                                to0[index] = Resources.Load("Stage 2/" + gameObject.name.Substring(0, gameObject.name.Length - 6) + "/toBlue/0" + (index + 1), typeof(Sprite)) as Sprite;
                            }
                            else
                            {
                                to0[index] = Resources.Load("Stage 2/" + gameObject.name.Substring(0, gameObject.name.Length - 6) + "/toBlue/" + (index + 1), typeof(Sprite)) as Sprite;
                            }
                            index += 1;
                            slider.value += 0.25f / proportion;
                            if (index > to0.Length - 1)
                            {
                                NextLoad();
                            }
                            break;
                        case 1:
                            if (index < 9)
                            {
                                to1[index] = Resources.Load("Stage 2/" + gameObject.name.Substring(0, gameObject.name.Length - 6) + "/toGreen/0" + (index + 1), typeof(Sprite)) as Sprite;
                            }
                            else
                            {
                                to1[index] = Resources.Load("Stage 2/" + gameObject.name.Substring(0, gameObject.name.Length - 6) + "/toGreen/" + (index + 1), typeof(Sprite)) as Sprite;
                            }
                            index += 1;
                            slider.value += 0.25f / proportion;
                            if (index > to1.Length - 1)
                            {
                                NextLoad();
                            }
                            break;
                        case 2:
                            if (index < 9)
                            {
                                to2[index] = Resources.Load("Stage 2/" + gameObject.name.Substring(0, gameObject.name.Length - 6) + "/toRed/0" + (index + 1), typeof(Sprite)) as Sprite;
                            }
                            else
                            {
                                to2[index] = Resources.Load("Stage 2/" + gameObject.name.Substring(0, gameObject.name.Length - 6) + "/toRed/" + (index + 1), typeof(Sprite)) as Sprite;
                            }
                            index += 1;
                            slider.value += 0.25f / proportion;
                            if (index > to2.Length - 1)
                            {
                                NextLoad();
                            }
                            break;
                        case 3:
                            if (index < 9)
                            {
                                to3[index] = Resources.Load("Stage 2/" + gameObject.name.Substring(0, gameObject.name.Length - 6) + "/toYellow/0" + (index + 1), typeof(Sprite)) as Sprite;
                            }
                            else
                            {
                                to3[index] = Resources.Load("Stage 2/" + gameObject.name.Substring(0, gameObject.name.Length - 6) + "/toYellow/" + (index + 1), typeof(Sprite)) as Sprite;
                            }
                            index += 1;
                            slider.value += 0.25f / proportion;
                            if (index > to3.Length - 1)
                            {
                                NextLoad();
                            }
                            break;
                    }
                }
            }
            else
            {
                transform.parent.gameObject.GetComponent<DontDestroy>().isLoaded[myID] = true;
                working = false;
            }
        }
    }

    private void NextLoad()
    {
        idLoading += 1;
        index = 0;
    }
}
