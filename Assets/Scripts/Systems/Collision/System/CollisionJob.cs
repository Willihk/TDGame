using TDGame.Systems.Collision.Collider;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace TDGame.Systems.Collision.System
{
    public struct CollisionResult
    {
        public int colliderAIndex;
        public int colliderBIndex;
    }

    [BurstCompatible]
    public struct CollisionJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<DistanceColliderData> ColliderDatas;

        [WriteOnly]
        public NativeQueue<CollisionResult>.ParallelWriter Results;

        public void Execute(int index)
        {
            for (int otherIndex = 0; otherIndex < ColliderDatas.Length; otherIndex++)
            {
                if (otherIndex == index || ColliderDatas[index].CollidesWithLayer != ColliderDatas[otherIndex].Layer)
                    continue;

                float distance = math.distance(ColliderDatas[index].Center, ColliderDatas[otherIndex].Center);
                distance = math.abs(distance);

                if (distance <= ColliderDatas[index].Radius + ColliderDatas[otherIndex].Radius)
                    Results.Enqueue(new CollisionResult {colliderAIndex = index, colliderBIndex = otherIndex});
            }
        }
    }
}