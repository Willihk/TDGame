using System.Collections.Generic;
using TDGame.Pathfinding.DotsAStar;
using TDGame.Systems.Grid;
using Unity.Mathematics;

namespace TDGame.Pathfinding
{
    public class Pathfinder
    {
        public List<int2> FindPath(int2 start, int2 end, int2 gridSize, Grid2D grid)
        {
            var path = new AStar().FindPath(start, end, gridSize, grid);

            path.Reverse();
            return path;
        }
    }
}