using Unity.Entities;

namespace TDGame.Managers
{
    public enum GameState
    {
        Undefined,
        Playing
    }
    
    public struct GameData : IComponentData
    {
        public GameState State;
    }
}