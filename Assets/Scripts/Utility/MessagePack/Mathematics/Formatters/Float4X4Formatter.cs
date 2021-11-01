using System;
using MessagePack;
using MessagePack.Formatters;
using Unity.Mathematics;

namespace TDGame.Utility.MessagePack.Mathematics.Formatters
{
    public sealed class Float4X4Formatter : IMessagePackFormatter<float4x4>
    {
        public void Serialize(ref MessagePackWriter writer, float4x4 value,
            MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(16);
            writer.Write(value.c0.x);
            writer.Write(value.c1.x);
            writer.Write(value.c2.x);
            writer.Write(value.c3.x);
            writer.Write(value.c0.y);
            writer.Write(value.c1.y);
            writer.Write(value.c2.y);
            writer.Write(value.c3.y);
            writer.Write(value.c0.z);
            writer.Write(value.c1.z);
            writer.Write(value.c2.z);
            writer.Write(value.c1.z);
            writer.Write(value.c0.w);
            writer.Write(value.c1.w);
            writer.Write(value.c2.w);
            writer.Write(value.c3.w);
        }

        public float4x4 Deserialize(ref MessagePackReader reader,
            MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }

            var length = reader.ReadArrayHeader();
            var m00 = default(float);
            var m10 = default(float);
            var m20 = default(float);
            var m30 = default(float);
            var m01 = default(float);
            var m11 = default(float);
            var m21 = default(float);
            var m31 = default(float);
            var m02 = default(float);
            var m12 = default(float);
            var m22 = default(float);
            var m32 = default(float);
            var m03 = default(float);
            var m13 = default(float);
            var m23 = default(float);
            var m33 = default(float);

            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        m00 = reader.ReadSingle();
                        break;
                    case 1:
                        m10 = reader.ReadSingle();
                        break;
                    case 2:
                        m20 = reader.ReadSingle();
                        break;
                    case 3:
                        m30 = reader.ReadSingle();
                        break;
                    case 4:
                        m01 = reader.ReadSingle();
                        break;
                    case 5:
                        m11 = reader.ReadSingle();
                        break;
                    case 6:
                        m21 = reader.ReadSingle();
                        break;
                    case 7:
                        m31 = reader.ReadSingle();
                        break;
                    case 8:
                        m02 = reader.ReadSingle();
                        break;
                    case 9:
                        m12 = reader.ReadSingle();
                        break;
                    case 10:
                        m22 = reader.ReadSingle();
                        break;
                    case 11:
                        m32 = reader.ReadSingle();
                        break;
                    case 12:
                        m03 = reader.ReadSingle();
                        break;
                    case 13:
                        m13 = reader.ReadSingle();
                        break;
                    case 14:
                        m23 = reader.ReadSingle();
                        break;
                    case 15:
                        m33 = reader.ReadSingle();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var result = default(float4x4);
            result.c0.x = m00;
            result.c1.x = m10;
            result.c2.x = m20;
            result.c3.x = m30;
            result.c0.y = m01;
            result.c1.y = m11;
            result.c2.y = m21;
            result.c3.y = m31;
            result.c0.z = m02;
            result.c1.z = m12;
            result.c2.z = m22;
            result.c3.z = m32;
            result.c0.w = m03;
            result.c1.w = m13;
            result.c2.w = m23;
            result.c3.w = m33;

            return result;
        }
    }
}