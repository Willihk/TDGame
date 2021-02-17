using TDGame.Enemy.Base;
using UnityEngine;

namespace TDGame.Systems.Projectiles
{
    public class TurretProjectile : MonoBehaviour
    {
        [SerializeField]
        private float speed = 5;
        
        private Vector3 target;
        private float hitDamage;

        public void Setup(Vector3 target, float hitDamage)
        {
            this.target = target;
            this.hitDamage = hitDamage;
        }

        private void Start()
        {
            transform.LookAt(target);
            Destroy(gameObject, 10);
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