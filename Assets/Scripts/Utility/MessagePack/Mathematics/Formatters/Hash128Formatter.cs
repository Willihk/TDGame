using System;
using MessagePack;
using MessagePack.Formatters;
using Unity.Entities;

namespace TDGame.Utility.MessagePack.Mathematics.Formatters
{
    public sealed class Hash128Formatter : IMessagePackFormatter<Hash128>
    {
        public void Serialize(ref MessagePackWriter writer, Hash128 value, MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(4);
            writer.Write(value.Value.x);
            writer.Write(value.Value.y);
            writer.Write(value.Value.z);
            writer.Write(value.Value.w);
        }

        public Hash128 Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }

            int length = reader.ReadArrayHeader();
            uint y = default;
            uint x = default;
            uint z = default;
            uint w = default;
            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        x = reader.ReadUInt32();
                        break;
                    case 1:
                        y = reader.ReadUInt32();
                        break;
                    case 2:
                        z = reader.ReadUInt32();
                        break;
                    case 3:
                        w = reader.ReadUInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var result = new Hash128(x, y, z, w);
            return result;
        }
    }
}
