using JetBrains.Annotations;
using NUnit.Framework;
using TDGame.Systems.Grid;

namespace Tests.Systems.Grid
{
    [TestFixture]
    public class TestGridUtility
    {
        [TestCase(1, 1, 0, 1)]
        [TestCase(10, 3, 9, 93)]
        [TestCase(20, 8, 3, 68)]
        public void TestGetIndex(int sizeX, int x, int y, int expected)
        {
            var grid = new Grid2D(sizeX, sizeX);
            Assert.That(expected, Is.EqualTo(grid.getIndex(x, y)));
        }
        
        [TestCase(20, 13, 3, 4, true)]
        [TestCase(20, 13, 20, 13, false)]
        [TestCase(200, 13, 123, 12, true)]
        [TestCase(120, 120, 0, 0, true)]
        public void TestGridIsValidPoint(int sizeX, int sizeY, int x, int y, bool expected)
        {
            var grid = new Grid2D(sizeX, sizeY);
            Assert.That(expected, Is.EqualTo(grid.IsValidGridPosition(x, y)));
        }
    }
}