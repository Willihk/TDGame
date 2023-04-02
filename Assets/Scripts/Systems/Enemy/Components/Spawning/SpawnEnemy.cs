using System;
using TDGame.Network.Components.Interfaces;
using Unity.Entities;

namespace TDGame.Systems.Enemy.Components.Spawning
{
    public struct SpawnEnemy : IComponentData, ITDSerializable
    {
        public Hash128 prefab;


        public const short NETWORK_MESSAGE_ID = 32145;

        public static void TDSerialize(SpawnEnemy instance, ref Span<byte> data)
        {
        }
        
        public static SpawnEnemy TDDeserialize(ref Span<byte> data)
        {
            return new SpawnEnemy() { };
        }

        public static int TDLength()
        {
            return 4 * 4;
        }
    }
}