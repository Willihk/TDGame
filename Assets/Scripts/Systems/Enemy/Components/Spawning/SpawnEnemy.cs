using System;
using System.Runtime.InteropServices;
using TDGame.Network.Components.Interfaces;
using Unity.Entities;
using Unity.Mathematics;

namespace TDGame.Systems.Enemy.Components.Spawning
{
    public struct SpawnEnemy : IComponentData, ITDSerializable
    {
        public Hash128 prefab;


        public const short NETWORK_MESSAGE_ID = 32145;

        public static void TDSerialize(SpawnEnemy instance, ref Span<byte> data)
        {
            MemoryMarshal.Write(data, ref instance.prefab.Value.x);
            MemoryMarshal.Write(data.Slice(4), ref instance.prefab.Value.y);
            MemoryMarshal.Write(data.Slice(8), ref instance.prefab.Value.z);
            MemoryMarshal.Write(data.Slice(12), ref instance.prefab.Value.w);
        }
        
        public static SpawnEnemy TDDeserialize(ref Span<byte> data)
        {
            return new SpawnEnemy() {prefab = new Hash128(
                MemoryMarshal.Read<uint>(data),
                MemoryMarshal.Read<uint>(data.Slice(4)),
                MemoryMarshal.Read<uint>(data.Slice(8)),
                MemoryMarshal.Read<uint>(data.Slice(12))
                )};
        }

        public static int TDLength()
        {
            return 4 * sizeof(uint);
        }
    }
}