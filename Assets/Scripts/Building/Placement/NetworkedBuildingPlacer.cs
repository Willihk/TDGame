using System;
using Mirror;
using TDGame.Cursor;
using Unity.Mathematics;
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
        [SerializeField]
        private bool isValidPlacement;

        [SerializeField]
        private bool isColliding;

        [SerializeField]
        [SyncVar]
        private string prefabName;

        [SerializeField]
        private BuildingList buildingList;

        [SerializeField]
        private LocalCursorState cursorState;

        private Camera referenceCamera;

        private GameObject prefab;

        [SerializeField]
        private Material placementMaterial;

        private static readonly int IsValid = Shader.PropertyToID("IsValid");

        public override void OnStartClient()
        {
            base.OnStartClient();
            Setup();
        }

        private void Setup()
        {
            referenceCamera = Camera.main;

            var prefabModel = buildingList.GetBuilding(prefabName).transform.Find("Model").gameObject;

            var model = Instantiate(prefabModel, transform);
            ReplaceModelMaterialsRecursive(model.transform);
        }

        private void ReplaceModelMaterialsRecursive(Transform transform)
        {
            if (transform.TryGetComponent(out Renderer renderer))
            {
                renderer.materials = new Material[] {placementMaterial};
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            foreach (Transform child in transform)
            {
                ReplaceModelMaterialsRecursive(child);
            }
        }

        private void Update()
        {
            // TODO: Only set value when it's actually changed
            placementMaterial.SetInt(IsValid, (isValidPlacement && !isColliding) ? 1 : 0);

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
                var hitPoint = math.round(hit.point);
                transform.position = new Vector3(hitPoint.x, transform.position.y, hitPoint.z);
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
            if (!isValidPlacement || isColliding)
                return;

            var placedObject = Instantiate(buildingList.GetBuilding(prefabName));
            placedObject.transform.position = position;

            NetworkServer.Spawn(placedObject, connectionToClient);

            NetworkServer.Destroy(gameObject);
        }
       
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ground"))
                return;
            isColliding = true;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Ground"))
                return;
            isColliding = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Ground"))
                return;
            isColliding = false;
        }
    }
}