using System;
using MessagePack;
using Unity.Mathematics;
using UnityEngine;

namespace TDGame.Systems.Grid.Data
{
    [Serializable]
    [MessagePackObject]
    public struct GridArea
    {
        [Key(0)]
        public int2 position;

        [Key(1)]
        public int width;

        [Key(2)]
        public int height;

        public int2[] GetPoints()
        {
            var points = new int2[width * height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    points[y * width + x] = position + new int2(x, y);
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