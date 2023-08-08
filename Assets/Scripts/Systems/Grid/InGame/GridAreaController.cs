﻿using System;
using TDGame.Systems.Grid.Data;
using UnityEngine;

namespace TDGame.Systems.Grid.InGame
{
    public class GridAreaController : MonoBehaviour
    {
        [SerializeField]
        private Transform originPoint;
        
        public GridArea area;

        private void Update()
        {
            var worldPosition = originPoint.position;

            area.position = GridManager.Instance.towerGrid.WorldToGridPosition(worldPosition);
        }

        public GridArea CalculateArea()
        {
            var worldPosition = originPoint.position;

            area.position = GridManager.Instance.towerGrid.WorldToGridPosition(worldPosition);

            return area;
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