using TDGame.Systems.Grid.Data;
using UnityEngine;

namespace TDGame.Systems.Grid.InGame
{
    public class GridObstacle : MonoBehaviour
    {
        public Transform originPoint;

        public GridArea area;

        private void Start()
        {
            var worldPosition = originPoint.position;

            if (GridController.Instance)
                area.position = GridController.Instance.towerGrid.WorldToGridPosition(worldPosition);
        }
        
        void OnDrawGizmosSelected()
        {
            var worldPosition = originPoint.position;
            var size = area.ConvertToWorldSize();
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(worldPosition, worldPosition + new Vector3(size.x, 0, 0));
            Gizmos.DrawLine(worldPosition, worldPosition + new Vector3(0, 0, size.y));
        }
    }
}