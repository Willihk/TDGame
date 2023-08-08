using System;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;
using TDGame.Utility.MessagePack.Mathematics.Formatters;
using Unity.Entities;
using Unity.Mathematics;

namespace TDGame.Utility.MessagePack.Mathematics
{
    public class MathematicsResolver : IFormatterResolver
    {
        public static readonly MathematicsResolver Instance = new();

        private MathematicsResolver()
        {
        }

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        private static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> Formatter;

            static FormatterCache()
            {
                Formatter = (IMessagePackFormatter<T>)MathematicsResolverGetFormatterHelper.GetFormatter(typeof(T));
            }
        }
    }

    public static class MathematicsResolverGetFormatterHelper
    {
        private static readonly Dictionary<Type, object> FormatterMap = new()
        {
            // standard
            { typeof(Hash128), new Hash128Formatter() },
            { typeof(int2), new Int2Formatter() },
            { typeof(float3), new Float3Formatter() },
            { typeof(float4), new Float4Formatter() },
            { typeof(quaternion), new QuaternionFormatter() },
            { typeof(float4x4), new Float4X4Formatter() },
   

            // standard + array
            { typeof(float3[]), new ArrayFormatter<float3>() },
            { typeof(Hash128[]), new ArrayFormatter<Hash128>() },
            { typeof(float4[]), new ArrayFormatter<float4>() },
            { typeof(quaternion[]), new ArrayFormatter<quaternion>() },
            { typeof(float4x4[]), new ArrayFormatter<float4x4>() },
       

            // standard + list
            { typeof(List<Hash128>), new ListFormatter<Hash128>() },
            { typeof(List<float3>), new ListFormatter<float3>() },
            { typeof(List<float4>), new ListFormatter<float4>() },
            { typeof(List<quaternion>), new ListFormatter<quaternion>() },
            { typeof(List<float4x4>), new ListFormatter<float4x4>() },
        };

        internal static object GetFormatter(Type t)
        {
            return FormatterMap.TryGetValue(t, out var formatter) ? formatter : null;
        }
    }
}