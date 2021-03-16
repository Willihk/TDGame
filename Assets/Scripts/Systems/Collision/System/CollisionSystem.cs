using System.Collections.Generic;
using System.Linq;
using TDGame.Systems.Collision.Collider;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace TDGame.Systems.Collision.System
{
    public class CollisionSystem : MonoBehaviour
    {
        public static CollisionSystem Instance;

        public List<DistanceCollider> Colliders;

        private NativeList<DistanceColliderData> colliderDatas;
        private NativeQueue<CollisionResult> collisions;

        private JobHandle handle;

        private void Awake()
        {
            Instance = this;
            Colliders ??= new List<DistanceCollider>();
            colliderDatas = new NativeList<DistanceColliderData>(Allocator.Persistent);
            collisions= new NativeQueue<CollisionResult>(Allocator.Persistent);
        }

        private void OnDestroy()
        {
            collisions.Dispose();
            colliderDatas.Dispose();
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
            if (colliderDatas.Length != Colliders.Count)
            {
                colliderDatas.Resize(Colliders.Count, NativeArrayOptions.UninitializedMemory);
            }
            
            for (int i = 0; i < Colliders.Count; i++)
            {
                colliderDatas[i] = Colliders[i].colliderData;
            }

            CollisionJob job = new CollisionJob
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
                if (Colliders.Count > collision.ColliderAIndex && Colliders.Count > collision.ColliderBIndex)
                {
                    Colliders[collision.ColliderAIndex].OnCollision(Colliders[collision.ColliderBIndex]);
                    Colliders[collision.ColliderBIndex].OnCollision(Colliders[collision.ColliderBIndex]);
                }
            }

        }
    }
}