using NUnit.Framework;
using TDGame.Systems.Grid;

namespace Tests.Systems.Grid
{
    [TestFixture]
    public class TestGridInitialization
    {
        [TestCase(10, 10)]
        [TestCase(2, 1)]
        public void TestSizeOnly(int x, int y)
        {
            Grid2D grid = new Grid2D(x, y);

            Assert.That(x * y, Is.EqualTo(grid.grid.Length));
        }

        [TestCase(10, 10)]
        [TestCase(2, 1)]
        public void TestDefaultCellSize(int x, int y)
        {
            Grid2D grid = new Grid2D(x, y);

            Assert.That(0.5f, Is.EqualTo(grid.CellSize));
        }

        [TestCase(10, 10, 1)]
        [TestCase(2, 1, 0.5f)]
        public void TestFullConstructor(int x, int y, float cellSize)
        {
            Grid2D grid = new Grid2D(x, y, cellSize);

            Assert.That(x * y, Is.EqualTo(grid.grid.Length));
            Assert.That(cellSize, Is.EqualTo(grid.CellSize));
        }
    }
}