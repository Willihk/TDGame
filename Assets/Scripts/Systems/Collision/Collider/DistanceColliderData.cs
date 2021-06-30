using System;
using TDGame.Systems.Collision.Layer;
using Unity.Mathematics;

namespace TDGame.Systems.Collision.Collider
{
    [Serializable]
    public struct DistanceColliderData
    {
        public ColliderLayer Layer;
        public ColliderLayer CollidesWithLayer;
        public float3 Center;
        public float Radius;
    }
}