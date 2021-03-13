using System;
using System.Collections.Generic;
using System.Linq;
using TDGame.Systems.Collision.Collider;
using TDGame.Systems.Collision.Layer;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace TDGame.Systems.Collision.System
{
    public class CollisionSystem : MonoBehaviour
    {
        public static CollisionSystem Instance;

        public List<DistanceCollider> Colliders;

        private NativeArray<DistanceColliderData> colliderDatas;
        private NativeQueue<CollisionResult> collisions;

        private JobHandle handle;

        private void Awake()
        {
            Instance = this;
            Colliders ??= new List<DistanceCollider>();
        }

        public void RegisterCollider(DistanceCollider collider)
        {
            Colliders.Add(collider);
        }

        public void UnregisterCollider(DistanceCollider collider)
        {
            Colliders.Remove(collider);
        }

        private void Update()
        {
            colliderDatas =
                new NativeArray<DistanceColliderData>(Colliders.Select(x => x.colliderData).ToArray(),
                    Allocator.TempJob);
            collisions = new NativeQueue<CollisionResult>(Allocator.TempJob);

            CollisionJob job = new CollisionJob()
            {
                ColliderDatas = colliderDatas,
                Results = collisions.AsParallelWriter()
            };
            handle = job.Schedule(colliderDatas.Length, 10);

        }

        private void LateUpdate()
        {
            handle.Complete();

            while (collisions.TryDequeue(out CollisionResult collision))
            {
                Colliders[collision.colliderAIndex].OnCollision(Colliders[collision.colliderBIndex]);
                Colliders[collision.colliderBIndex].OnCollision(Colliders[collision.colliderBIndex]);
            }

            collisions.Dispose();
            colliderDatas.Dispose();
        }

        public static bool CollidesWith(DistanceColliderData colliderA, DistanceColliderData colliderB)
        {
            if (!CanCollideWith(colliderA, colliderB))
            {
                return false;
            }

            float distance = math.distance(colliderA.Center, colliderB.Center);
            distance = math.abs(distance);

            return distance <= colliderA.Radius + colliderB.Radius;
        }

        public static bool CanCollideWith(DistanceColliderData colliderA, DistanceColliderData colliderB)
        {
            return colliderA.CollidesWithLayer == colliderB.Layer;
        }
    }
}