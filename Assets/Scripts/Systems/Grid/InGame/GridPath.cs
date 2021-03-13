﻿using TDGame.Systems.Grid.Data;
using UnityEngine;

namespace TDGame.Systems.Grid.InGame
{
    public class GridPath : MonoBehaviour
    {
        public Transform originPoint;

        public GridArea area;

        private void Start()
        {
            var worldPosition = originPoint.position;

            if (GridController.Instance)
                area.position = GridController.Instance.towerGrid.WorldToGridPosition(worldPosition);
        }
        
        void OnDrawGizmos()
        {
            var worldPosition = originPoint.position;
            var size = area.ConvertToWorldSize();
            Gizmos.color = Color.green;
            Gizmos.DrawCube(worldPosition + (new Vector3(size.x, 0, size.y) / 2), new Vector3(size.x, .1f, size.y));
        }
    }
}