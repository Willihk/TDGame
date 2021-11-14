using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TDGame.Systems.Old_Enemy.Manager;
using TDGame.Systems.Old_Enemy.Movement.Data;
using Object = UnityEngine.Object;


namespace TDGame.Command.Implementations.Wave
{
    [Obsolete]
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
            var spawnedObject = Object.Instantiate(prefab);

            spawnedObject.transform.position = startPosition;

            spawnedObject.GetComponent<EnemyMovement>().Setup(waypoints);

            NetworkServer.Spawn(spawnedObject);

            EnemyManager.Instance.RegisterTarget(spawnedObject);
        }
    }
}
