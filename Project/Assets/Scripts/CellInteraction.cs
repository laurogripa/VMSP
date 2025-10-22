using UnityEngine;
using UnityEngine.UI;

namespace Selection
{
    public class CellInteraction : MonoBehaviour
    {
        private GridController gridController;
        private Button cellButton;

        private void Start()
        {
            // Find the GridController on the Canvas
            gridController = FindFirstObjectByType<GridController>();
            
            // Get or add Button component
            cellButton = GetComponent<Button>();
            if (cellButton == null)
                cellButton = gameObject.AddComponent<Button>();
            
            // Register click handler
            cellButton.onClick.AddListener(OnCellClicked);
        }

        private void OnCellClicked()
        {
            if (gridController != null)
            {
                gridController.OnPointSelected();
                Debug.Log($"Cell clicked: {gameObject.name}");
            }
        }
    }
}
