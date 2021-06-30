using TDGame.Systems.Collision.Collider;
using TDGame.Systems.Collision.Processes;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace TDGame.Systems.Collision.System
{
    public struct CollisionResult
    {
        public int ColliderAIndex;
        public int ColliderBIndex;
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
                if (otherIndex == index)
                    continue;

                if (CollisionProcess.CollidesWith(ColliderDatas[index], ColliderDatas[otherIndex]))
                {
                    Results.Enqueue(new CollisionResult {ColliderAIndex = index, ColliderBIndex = otherIndex});
                }
            }
        }
    }
}