using MessagePack;
using TDGame.Systems.Grid.Cell;
using TDGame.Systems.Grid.Data;

namespace TDGame.Systems.Grid.Messages.Server
{
    [MessagePackObject]
    public struct SetGridAreaMessage
    {
        [Key(0)]
        public GridType Type;

        [Key(1)]
        public GridArea Area;

        [Key(2)]
        public GridCell Cell;
    }
}