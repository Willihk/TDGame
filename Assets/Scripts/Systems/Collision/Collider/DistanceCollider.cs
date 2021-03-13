using System;
using TDGame.Systems.Collision.System;
using UnityEngine;
using UnityEngine.Events;

namespace TDGame.Systems.Collision.Collider
{
    public class DistanceCollider : MonoBehaviour
    {
        public bool autoRegisterForCollision = true;

        public DistanceColliderData colliderData;

        public UnityEvent<DistanceCollider> collisionEvent;

        private void Awake()
        {
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
            colliderData.Center = transform.position;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(colliderData.Center, colliderData.Radius);
        }
    }
}