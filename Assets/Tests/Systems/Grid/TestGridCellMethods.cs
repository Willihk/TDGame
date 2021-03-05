using NUnit.Framework;
using TDGame.Systems.Grid;
using TDGame.Systems.Grid.Cell.Implementations;

namespace Tests.Systems.Grid
{
    [TestFixture]
    public class TestGridCellMethods
    {
        [TestCase(2, 4, true)]
        [TestCase(5, 15, true)]
        [TestCase(1, 40, false)]
        public void TestGridSetCell(int x, int y, bool expected)
        {
            var grid = new Grid2D(21, 32);
            Assert.That(grid.SetCell(x, y, new EmptyCell()), Is.EqualTo(expected));
        }
    }
}