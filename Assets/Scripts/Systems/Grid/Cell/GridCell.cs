using MessagePack;

namespace TDGame.Systems.Grid.Cell
{
    [MessagePackObject]
    public struct GridCell
    {
        [Key(0)]
        public GridCellState State;

        [Key(1)]
        public int OccupierId;
    }
}