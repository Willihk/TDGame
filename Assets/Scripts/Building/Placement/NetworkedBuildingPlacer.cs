using System;
using Mirror;
using TDGame.Cursor;
using UnityEngine;
using UnityEngine.EventSystems;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

namespace TDGame.Building.Placement
{
    public class NetworkedBuildingPlacer : NetworkBehaviour
    {
        private bool isValidPlacement;

        [SerializeField]
        [SyncVar]
        private string prefabName;

        [SerializeField]
        private BuildingList buildingList;

        [SerializeField]
        private LocalCursorState cursorState;

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

            if (Input.GetMouseButtonDown(1))
            {
                cursorState.State = CursorState.None;
                Cmd_CancelPlacement();
            }

            Ray ray = referenceCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                isValidPlacement = true;
            }
            else
            {
                isValidPlacement = false;
                return;
            }

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                cursorState.State = CursorState.None;
                Cmd_ConfirmPlacement(transform.position);
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
        void Cmd_ConfirmPlacement(Vector3 position)
        {
            if (!isValidPlacement)
                return;

            var placedObject = Instantiate(buildingList.GetBuilding(prefabName));
            placedObject.transform.position = position;

            NetworkServer.Spawn(placedObject, connectionToClient);

            NetworkServer.Destroy(gameObject);
        }

        private void OnCollisionExit(Collision other)
        {
            isValidPlacement = true;
        }

        private void OnCollisionEnter(Collision other)
        {
            isValidPlacement = false;
        }
    }
}