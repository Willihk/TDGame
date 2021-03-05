using NUnit.Framework;
using TDGame.Systems.Grid;
using UnityEngine;

namespace Tests.Systems.Grid
{
    [TestFixture]
    public class TestGridConversion
    {
        [TestCase(0.5F, 1, 3, .5F, 0F, 1.5F)]
        [TestCase(1.5F, 1, 5, 1.5F, 0F, 7.5F)]
        [TestCase(10F, 3, 17, 30F, 0F, 170F)]
        public void TestGridToWorldPosition(float cellSize, int gridX, int gridY, float expectedX, float expectedY,
            float expectedZ)
        {
            var grid = new Grid2D(10, 20, cellSize);
            var converted = grid.GridToWorldPosition(gridX, gridY);
            Assert.That(new Vector3(expectedX, expectedY, expectedZ), Is.EqualTo(converted));
        }

        [TestCase(0.5F, .5F, 0F, 1.5F, 1, 3)]
        [TestCase(1.5F, 1.5F, 5, 7.5F, 1, 5)]
        [TestCase(10F, 30, 17, 170, 3, 17)]
        public void TestWorldToGridPosition(float cellSize, float worldX, float worldY, float worldZ, int expectedX,
            int expectedY
        )
        {
            var grid = new Grid2D(10, 20, cellSize);
            var converted = grid.WorldToGridPosition(new Vector3(worldX, worldY, worldZ));
            Assert.That(new Vector2Int(expectedX, expectedY), Is.EqualTo(converted));
        }
    }
}