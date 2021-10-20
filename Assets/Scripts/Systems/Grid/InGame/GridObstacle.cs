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

            if (Old_GridController.Instance)
                area.position = Old_GridController.Instance.towerGrid.WorldToGridPosition(worldPosition);
        }
        
        void OnDrawGizmosSelected()
        {
            var worldPosition = originPoint.position;
            var size = area.ConvertToWorldSize();
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(worldPosition + (new Vector3(size.x, 0, size.y) / 2), new Vector3(size.x, .1f, size.y));
        }
    }
}