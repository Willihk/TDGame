using System.Collections;
using NUnit.Framework;
using TDGame.Systems.Collision.Collider;
using TDGame.Systems.Collision.Layer;
using TDGame.Systems.Collision.Processes;
using TDGame.Systems.Collision.System;
using Unity.Mathematics;

namespace Tests.Systems.Collision
{
    [TestFixture]
    public class TestCollisionProcess
    {
        [TestCase(true, ColliderLayer.None, ColliderLayer.None)]
        [TestCase(false, ColliderLayer.Enemy, ColliderLayer.None)]
        public void TestCanColideWith(bool expected, ColliderLayer canCollideWith, ColliderLayer otherLayer)
        {
            var colliderA = new DistanceColliderData() {CollidesWithLayer = canCollideWith};
            var colliderB = new DistanceColliderData() {Layer = otherLayer};

            Assert.That(expected, Is.EqualTo(CollisionProcess.CanCollideWith(colliderA, colliderB)));
        }

        [TestCaseSource(nameof(cases))]
        public void TestCollidesWith(bool expected, DistanceColliderData colliderA, DistanceColliderData colliderB)
        {
            Assert.That(expected, Is.EqualTo(CollisionProcess.CollidesWith(colliderA, colliderB)));
        }

        static object[] cases =
        {
            new object[]
            {
                true,
                new DistanceColliderData
                    {Center = new float3(1, 0, 1), Radius = 1, CollidesWithLayer = ColliderLayer.Enemy},
                new DistanceColliderData {Center = new float3(2, 0, 1), Radius = 0, Layer = ColliderLayer.Enemy}
            },new object[]
            {
                true,
                new DistanceColliderData
                    {Center = new float3(-1.5f, 0, -2.5f), Radius = 1, CollidesWithLayer = ColliderLayer.None},
                new DistanceColliderData {Center = new float3(-1.5f, 0, -3.25f), Radius = 0, Layer = ColliderLayer.None}
            },
        };
    }
}