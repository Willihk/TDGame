using System;
using UnityEngine;

namespace TDGame.Systems.Grid.Data
{
    [Serializable]
    public struct GridArea
    {
        public Vector2Int position;
        public int width;
        public int height;

        public Vector2Int[] GetPoints()
        {
            var points = new Vector2Int[width * height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    points[y * width + x] = position + new Vector2Int(x, y);
                }
            }

            return points;
        }

        public Vector3 GetWorldPosition()
        {
            return new Vector3(position.x, 0, position.y) * .5f;
        }

        public Vector2 ConvertToWorldSize()
        {
            return new Vector2(width, height) * .5f;
        }
    }
}