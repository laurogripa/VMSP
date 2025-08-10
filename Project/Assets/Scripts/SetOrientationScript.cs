using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetOrientationScript : MonoBehaviour
{
    void Start()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            switch(SceneManager.GetActiveScene().name)
            {
                case "Stage 0":
                    Screen.orientation = ScreenOrientation.Portrait;
                    break;
                case "Stage 1":
                    Screen.orientation = ScreenOrientation.Portrait;
                    break;
                case "Stage 2":
                    Screen.orientation = ScreenOrientation.LandscapeLeft;
                    break;
            }
        }
    }

}
