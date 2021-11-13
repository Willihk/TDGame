using Mirror;
using TDGame.Systems.Collision.Collider;
using TDGame.Systems.Old_Enemy.DamageReceiver.Base;
using TDGame.Systems.Targeting.Implementations;
using UnityEngine;

namespace TDGame.Systems.Projectiles
{
    public class HomingProjectile : NetworkBehaviour
    {
        [SerializeField]
        private SingleTargetSystem targetSystem;
        
        private Vector3 initialTarget;
        private float hitDamage;

        [SyncVar]
        private float speed;

        private void Start()
        {
            transform.LookAt(initialTarget);
            Destroy(gameObject, 1.5f);
        }
        

        public void Setup(Vector3 target, float hitDamage, float speed)
        {
            this.initialTarget = target;
            this.hitDamage = hitDamage;
            this.speed = speed;
        }

        public void OnCollision(DistanceCollider other)
        {
            if (other.gameObject.TryGetComponent(out BaseDamageReceiver damageReceiver))
            {
                damageReceiver.Damage(hitDamage);
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            transform.LookAt(targetSystem.clientTargetPosition);
            transform.Translate(Vector3.forward * (speed * Time.deltaTime));
        }
    }
}
