using System;
using Mirror;
using UnityEngine;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

namespace TDGame.Building.Placement
{
    public class NetworkedBuildingPlacer : NetworkBehaviour
    {
        [SerializeField]
        private bool isValidPlacement;

        [SerializeField]
        [SyncVar]
        private string prefabName;

        [SerializeField]
        private BuildingList buildingList;

        private Camera referenceCamera;

        private GameObject prefab;

        public override void OnStartClient()
        {
            base.OnStartClient();
            referenceCamera = Camera.main;

            var prefabModel = buildingList.GetBuilding(prefabName).transform.Find("Model").gameObject;

            var model = Instantiate(prefabModel, transform);
        }

        private void Update()
        {
            if (!hasAuthority)
                return;

            Ray ray = referenceCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            }

            if (Input.GetMouseButtonDown(1))
            {
                Cmd_CancelPlacement();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                Cmd_ConfirmPlacement();
            }
            
        }

        [Server]
        public void Setup(string prefabName)
        {
            this.prefabName = prefabName;
            isValidPlacement = true;
        }

        [Command]
        void Cmd_CancelPlacement()
        {
            NetworkServer.Destroy(gameObject);
        }

        [Command]
        void Cmd_ConfirmPlacement()
        {
            if (!isValidPlacement)
                return;

            var placedObject = Instantiate(buildingList.GetBuilding(prefabName));
            placedObject.transform.position = transform.position;

            NetworkServer.Spawn(placedObject, connectionToClient);

            NetworkServer.Destroy(gameObject);
        }

        [ServerCallback]
        private void OnCollisionExit(Collision other)
        {
            isValidPlacement = true;
        }

        [ServerCallback]
        private void OnCollisionEnter(Collision other)
        {
            isValidPlacement = false;
        }
    }
}