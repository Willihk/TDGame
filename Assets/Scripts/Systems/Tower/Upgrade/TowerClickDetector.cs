using TDGame.Events;
using TDGame.Systems.Grid.Cell;
using TDGame.Systems.Grid.InGame;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TDGame.Systems.Tower.Upgrade
{
    public class TowerClickDetector : MonoBehaviour
    {
        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("TowerPlacementArea")))
                    return;
                
                var point = GridManager.Instance.towerGrid.WorldToGridPosition(hit.point);
                var cell = GridManager.Instance.towerGrid.GetCell(point.x, point.y);
                if (cell.State == GridCellState.Occupied)
                {
                    int id = cell.OccupierId;
                    EventManager.Instance.onClickTower.Raise(id);
                }
                else
                {
                    EventManager.Instance.onClickTower.Raise(-1);

                }
            }
        }
    }
}