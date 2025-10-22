using UnityEngine;

namespace Selection
{
    public class GridController : MonoBehaviour
    {
        [SerializeField] private GameObject grid3x3;
        [SerializeField] private GameObject grid4x4;
        
        private int pointCount = 0;
        private bool hasShownGrid4x4 = false;

        private void Start()
        {
            // Ensure 3x3 grid is visible at start
            if (grid3x3 != null)
                grid3x3.SetActive(true);
            
            // Ensure 4x4 grid is hidden at start
            if (grid4x4 != null)
                grid4x4.SetActive(false);
        }

        /// <summary>
        /// Call this method when a point is selected/clicked on the grid
        /// </summary>
        public void OnPointSelected()
        {
            pointCount++;
            
            // Show 4x4 grid after the first point
            if (pointCount == 1 && !hasShownGrid4x4)
            {
                ShowGrid4x4();
            }
        }

        private void ShowGrid4x4()
        {
            hasShownGrid4x4 = true;
            
            if (grid3x3 != null)
                grid3x3.SetActive(false);
            
            if (grid4x4 != null)
                grid4x4.SetActive(true);
            
            Debug.Log("Grid switched from 3x3 to 4x4");
        }

        /// <summary>
        /// Reset the grids to initial state
        /// </summary>
        public void ResetGrids()
        {
            pointCount = 0;
            hasShownGrid4x4 = false;
            
            if (grid3x3 != null)
                grid3x3.SetActive(true);
            
            if (grid4x4 != null)
                grid4x4.SetActive(false);
            
            Debug.Log("Grids reset to initial state");
        }

        public int GetPointCount()
        {
            return pointCount;
        }
    }
}
