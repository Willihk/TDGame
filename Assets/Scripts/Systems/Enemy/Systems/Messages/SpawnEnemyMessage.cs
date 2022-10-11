using MessagePack;

namespace TDGame.Systems.Enemy.Systems.Messages
{
    [MessagePackObject]
    public struct SpawnEnemyMessage
    {
        [Key(0)]
        public string Id;
    }
}