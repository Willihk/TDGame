using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TDGame.Systems.Targeting.Data;
using TDGame.Enemy.Base;
using Mirror;


namespace TDGame.Command.Implementations.Wave
{
    public class SpawnPrefab : WaveCommand
    {
        GameObject prefab;
        Transform holder;

        Vector3 startPosition;
        List<Vector3> waypoints;

        public SpawnPrefab(GameObject prefab, Transform holder, Vector3 startPosition, List<Vector3> waypoints)
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
