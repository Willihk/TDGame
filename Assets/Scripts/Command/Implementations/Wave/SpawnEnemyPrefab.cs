using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TDGame.Systems.Enemy.Manager;
using TDGame.Systems.Enemy.Movement.Base;


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

            spawnedObject.GetComponent<BaseMovementController>().Setup(waypoints);

            NetworkServer.Spawn(spawnedObject);

            EnemyManager.Instance.RegisterTarget(spawnedObject);
        }
    }
}