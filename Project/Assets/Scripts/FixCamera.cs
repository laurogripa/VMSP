using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FixCamera : MonoBehaviour
{
    [SerializeField] private GameObject referenceIn, referenceOut;
    private Camera gameCam;

    void Start()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            switch (SceneManager.GetActiveScene().name)
            {
                case "Main Menu":
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
        gameCam = gameObject.GetComponent<Camera>();
        if (!IsTargetVisible(gameCam, referenceIn))
        {
            IncreaseSize();
        }
        else if (IsTargetVisible(gameCam, referenceOut))
        {
            DecreaseSize();
        }
    }

    void DecreaseSize()
    {
        gameCam.orthographicSize -= 0.01f;
        if (IsTargetVisible(gameCam, referenceOut))
        {
            DecreaseSize();
        }
    }

    void IncreaseSize()
    {
        gameCam.orthographicSize += 0.01f;
        if (!IsTargetVisible(gameCam, referenceIn))
        {
            IncreaseSize();
        }
    }

    bool IsTargetVisible(Camera c, GameObject go)
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(c);
        var point = go.transform.position;
        foreach (var plane in planes)
        {
            if (plane.GetDistanceToPoint(point) < 0)
                return false;
        }
        return true;
    }
}
