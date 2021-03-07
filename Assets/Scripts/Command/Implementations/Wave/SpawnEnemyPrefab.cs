using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TDGame.Systems.Targeting.Data;
using TDGame.Enemy.Base;
using Mirror;


namespace TDGame.Command.Implementations.Wave
{
    public class SpawnEnemyPrefab : WaveCommand
    {
        private GameObject prefab;
        private Transform holder;

        private Vector3 startPosition;
        private List<Vector3> waypoints;

        public SpawnEnemyPrefab(GameObject prefab, Transform holder, Vector3 startPosition, List<Vector3> waypoints)
        {
            this.prefab = prefab;
            this.holder = holder;
            this.startPosition = startPosition;
            this.waypoints = waypoints;
        }

        public override void Execute()
        {
            var spawnedObject = Object.Instantiate(prefab, holder);

            spawnedObject.transform.position = startPosition;

            spawnedObject.GetComponent<NetworkedEnemy>().Setup(waypoints);

            NetworkServer.Spawn(spawnedObject);

            EnemyTargetsController.Instance.targets.Add(spawnedObject);
        }
    }
}
