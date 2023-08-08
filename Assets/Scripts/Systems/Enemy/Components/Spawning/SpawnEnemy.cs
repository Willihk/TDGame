using System;
using System.Runtime.InteropServices;
using TDGame.Network.Components.Interfaces;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Serialization.Binary;

namespace TDGame.Systems.Enemy.Components.Spawning
{
    public struct SpawnEnemy : IComponentData, ITDSerializable
    {
        public Hash128 prefab;


        public const short NETWORK_MESSAGE_ID = 32145;

        public static void TDSerialize(SpawnEnemy instance, ref Span<byte> data)
        {
            unsafe
            {
                fixed (byte* dataPtr = data)
                {
                    var writer = new DataStreamWriter(dataPtr, TDLength());
                    writer.WriteUInt(instance.prefab.Value.x);
                    writer.WriteUInt(instance.prefab.Value.y);
                    writer.WriteUInt(instance.prefab.Value.z);
                    writer.WriteUInt(instance.prefab.Value.w);
                }
                // MemoryMarshal.Write(data.Slice(0, 4), ref instance.prefab.Value.x);
                // MemoryMarshal.Write(data.Slice(4, 4), ref instance.prefab.Value.y);
                // MemoryMarshal.Write(data.Slice(8,4 ), ref instance.prefab.Value.z);
                // MemoryMarshal.Write(data.Slice(12, 4), ref instance.prefab.Value.w);
            }
        }

        public static SpawnEnemy TDDeserialize(ref Span<byte> data)
        {
            unsafe
            {
                fixed (byte* dataPtr = data)
                {
                    var na = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(dataPtr, 16,
                        Allocator.Invalid);

                    var reader = new DataStreamReader(na);
                    uint x = reader.ReadUInt();
                    uint y = reader.ReadUInt();
                    uint z = reader.ReadUInt();
                    uint w = reader.ReadUInt();
                    return new SpawnEnemy() { prefab = new Hash128(x, y, z, w) };
                }
            }
        }

        public static int TDLength()
        {
            return 4 * 4;
        }
    }
}