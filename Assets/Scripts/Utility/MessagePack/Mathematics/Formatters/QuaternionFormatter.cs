using System;
using MessagePack;
using MessagePack.Formatters;
using Unity.Mathematics;

namespace TDGame.Utility.MessagePack.Mathematics.Formatters
{
    public sealed class QuaternionFormatter : IMessagePackFormatter<quaternion>
    {
        public void Serialize(ref MessagePackWriter writer, quaternion value, MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(4);
            writer.Write(value.value.x);
            writer.Write(value.value.y);
            writer.Write(value.value.z);
            writer.Write(value.value.w);
        }

        public quaternion Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }

            var length = reader.ReadArrayHeader();
            var x = default(float);
            var y = default(float);
            var z = default(float);
            var w = default(float);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        x = reader.ReadSingle();
                        break;
                    case 1:
                        y = reader.ReadSingle();
                        break;
                    case 2:
                        z = reader.ReadSingle();
                        break;
                    case 3:
                        w = reader.ReadSingle();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var result = new quaternion(x, y, z, w);
            return result;
        }
    }
}
