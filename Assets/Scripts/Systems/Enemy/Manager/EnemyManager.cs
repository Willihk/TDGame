using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TDGame.Systems.Enemy.Manager
{
    public class EnemyManager : MonoBehaviour
    {
        public static EnemyManager Instance;

        public List<GameObject> targets;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDrawGizmosSelected()
        {
            foreach (var target in targets.Select(x => x.GetComponent<Collider>().bounds))
            {
                Gizmos.DrawCube(target.center, target.size);
            }

        }

        public List<GameObject> GetTargets()
        {
            return targets;
        }

        public void RegisterTarget(GameObject targetObject)
        {
            targets.Add(targetObject);
        }

        public void UnregisterTarget(GameObject targetObject)
        {
            targets.RemoveAll(x => x == targetObject);
        }
    }
}