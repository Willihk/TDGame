using TDGame.Systems.Collision.Collider;
using Unity.Mathematics;

namespace TDGame.Systems.Collision.Processes
{
    public static class CollisionProcess
    {
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