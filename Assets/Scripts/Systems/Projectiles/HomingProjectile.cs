using System.Collections;
using System.Collections.Generic;
using TDGame.Systems.Collision.Collider;
using TDGame.Systems.Enemy.DamageReceiver.Base;
using UnityEngine;

namespace TDGame
{
    public class HomingProjectile : MonoBehaviour
    {
        private Vector3 target;
        private float hitDamage;
        private float speed;
        private float turnRate;

        private void Start()
        {
            transform.LookAt(target);
            Destroy(gameObject, 1.5f);
        }

        public void Setup(Vector3 target, float hitDamage, float speed)
        {
            this.target = target;
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
            transform.LookAt(target);
            transform.Translate(Vector3.forward * (speed * Time.deltaTime));
        }
    }
}
