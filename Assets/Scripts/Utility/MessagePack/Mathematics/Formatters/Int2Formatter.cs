using System;
using MessagePack;
using MessagePack.Formatters;
using Unity.Mathematics;

namespace TDGame.Utility.MessagePack.Mathematics.Formatters
{
    public class Int2Formatter : IMessagePackFormatter<int2>
    {
        public void Serialize(ref MessagePackWriter writer, int2 value, MessagePackSerializerOptions options)
        {
            writer.WriteInt32(value.x);
            writer.WriteInt32(value.y);
        }

        public int2 Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }

            var x = reader.ReadInt32();
            var y = reader.ReadInt32();
            
            var result = new int2(x, y);
            return result;
        }
    }
}