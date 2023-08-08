using Unity.Entities;
using Unity.Mathematics;

namespace TDGame.Systems.Grid.SpatialTree
{
    public struct MapDetailsSingleton : IComponentData
    {
        public bool IsLoaded;
        public int2 Size;
    }
}