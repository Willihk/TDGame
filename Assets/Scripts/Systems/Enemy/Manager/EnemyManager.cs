using System.Collections.Generic;
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