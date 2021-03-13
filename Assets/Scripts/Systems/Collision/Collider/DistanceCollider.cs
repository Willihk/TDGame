using System;
using TDGame.Systems.Collision.System;
using UnityEngine;
using UnityEngine.Events;

namespace TDGame.Systems.Collision.Collider
{
    public class DistanceCollider : MonoBehaviour
    {
        public bool autoRegisterForCollision = true;

        [SerializeField]
        private Transform centerPoint;

        public DistanceColliderData colliderData;

        public UnityEvent<DistanceCollider> collisionEvent;

        private void Awake()
        {
            centerPoint ??= transform;
            collisionEvent ??= new UnityEvent<DistanceCollider>();
        }

        private void Start()
        {
            if (autoRegisterForCollision)
            {
                CollisionSystem.Instance.RegisterCollider(this);
            }
        }

        private void OnDestroy()
        {
            if (autoRegisterForCollision)
            {
                CollisionSystem.Instance.UnregisterCollider(this);
            }
        }

        public void OnCollision(DistanceCollider other)
        {
            collisionEvent.Invoke(other);
        }

        private void Update()
        {
            colliderData.Center = centerPoint.position;
        }

        private void OnDrawGizmosSelected()
        {
            Update();
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(colliderData.Center, colliderData.Radius);
        }
    }
}