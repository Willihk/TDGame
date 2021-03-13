using System;
using System.Collections.Generic;
using System.Linq;
using TDGame.Systems.Collision.Collider;
using TDGame.Systems.Collision.Layer;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace TDGame.Systems.Collision.System
{
    public class CollisionSystem : MonoBehaviour
    {
        public static CollisionSystem Instance;
        
        public List<DistanceCollider> Colliders;

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
            CheckForCollisions();
        }

        void CheckForCollisions()
        {
            // convert data

            NativeArray<DistanceColliderData> colliderDatas =
                new NativeArray<DistanceColliderData>(Colliders.Select(x => x.colliderData).ToArray(), Allocator.Temp);


            for (int i = 0; i < colliderDatas.Length; i++)
            {
                var colliderA = colliderDatas[i];
                for (int j = 0; j < colliderDatas.Length; j++)
                {
                    if (j == i)
                        continue;
                    if (!CollidesWith(colliderA, colliderDatas[j])) 
                        continue;
                    
                    Debug.Log($"{Colliders[i]} collides with {Colliders[j]}");

                    Colliders[i].OnCollision(Colliders[j]);
                    Colliders[j].OnCollision(Colliders[i]);
                }
            }

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