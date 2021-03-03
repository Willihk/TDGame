using TDGame.Enemy.Base;
using UnityEngine;

namespace TDGame.Systems.Projectiles
{
    public class TurretProjectile : MonoBehaviour
    {

        private Vector3 target;
        private float hitDamage;
        private float speed;

        public void Setup(Vector3 target, float hitDamage, float speed)
        {
            this.target = target;
            this.hitDamage = hitDamage;
            this.speed = speed;
        }

        private void Start()
        {
            transform.LookAt(target);
            Destroy(gameObject, 3);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out NetworkedEnemy enemy))
            {
                enemy.Damage(hitDamage);
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            transform.Translate(Vector3.forward * (speed * Time.deltaTime));
        }
    }
}