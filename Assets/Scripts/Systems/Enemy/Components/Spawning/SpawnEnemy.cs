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
            MemoryMarshal.Write(data.Slice(0, 4), ref instance.prefab.Value.x);
            MemoryMarshal.Write(data.Slice(4, 4), ref instance.prefab.Value.y);
            MemoryMarshal.Write(data.Slice(8,4 ), ref instance.prefab.Value.z);
            MemoryMarshal.Write(data.Slice(12, 4), ref instance.prefab.Value.w);
        }
        
        public static SpawnEnemy TDDeserialize(ref Span<byte> data)
        {
            uint x = MemoryMarshal.Read<uint>(data.Slice(0, 4));
            uint y = MemoryMarshal.Read<uint>(data.Slice(4, 4));
            uint z = MemoryMarshal.Read<uint>(data.Slice(8, 4));
            uint w = MemoryMarshal.Read<uint>(data.Slice(12, 4));
            
            
            return new SpawnEnemy() {prefab = new Hash128(x,y,z,w)};
        }

        public static int TDLength()
        {
            return 4 * sizeof(uint);
        }
    }
}