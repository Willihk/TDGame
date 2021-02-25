using System;
using System.Collections;
using Mirror;
using TDGame.Cursor;
using TDGame.Systems.Economy;
using TDGame.Systems.Economy.Data;
using TDGame.Systems.Turrets.Base;
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

        [SyncVar]
        private int price;

        [SyncVar]
        private bool canAfford;

        [SerializeField]
        private BuildingList buildingList;

        [SerializeField]
        private LocalCursorState cursorState;

        [SerializeField]
        private Material placementMaterial;

        private Camera referenceCamera;

        private GameObject prefab;

        private Collider collider;

        private Material localMaterial;

        private NetworkedPlayerEconomy playerEconomy;

        private static readonly int IsValid = Shader.PropertyToID("IsValid");

        public override void OnStartClient()
        {
            base.OnStartClient();
            Setup();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            collider = GetComponent<Collider>();

            playerEconomy = PlayerEconomyManager.Instance.GetEconomy(connectionToClient);
        }

        private void Setup()
        {
            referenceCamera = Camera.main;

            var prefabModel = buildingList.GetBuilding(prefabName).transform.Find("Model").gameObject;

            var model = Instantiate(prefabModel, transform);
            localMaterial = new Material(placementMaterial);
            ReplaceModelMaterialsRecursive(model.transform);
        }

        private void ReplaceModelMaterialsRecursive(Transform transform)
        {
            if (transform.TryGetComponent(out Renderer renderer))
            {
                renderer.materials = new Material[] {localMaterial};
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            foreach (Transform child in transform)
            {
                ReplaceModelMaterialsRecursive(child);
            }
        }

        private void Update()
        {
            if (isServer)
            {
                canAfford = playerEconomy.CanAfford(price);
            }
            if (isClient)
            {
                // TODO: Only set value when it's actually changed
                isValidPlacement = !isColliding;

                isValidPlacement = canAfford && isValidPlacement;

                if (localMaterial)
                    localMaterial.SetInt(IsValid, isValidPlacement ? 1 : 0);
            }

            if (!hasAuthority)
                return;

            if (Input.GetMouseButtonDown(1))
            {
                cursorState.State = CursorState.None;
                Cmd_CancelPlacement();
            }

            Ray ray = referenceCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("TowerPlacementArea")))
            {
                var hitPoint = math.round(hit.point);
                transform.position = new Vector3(hitPoint.x, transform.position.y, hitPoint.z);
                isValidPlacement = true;
            }
            else
            {
                isValidPlacement = false;
                localMaterial.SetInt(IsValid, 0);
                return;
            }

            if (!isValidPlacement || isColliding)
                return;

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && canAfford)
            {
                cursorState.State = CursorState.None;
                Cmd_ConfirmPlacement(transform.position);
            }
        }

        [Server]
        public void Setup(string prefabName)
        {
            this.prefabName = prefabName;
            price = buildingList.GetBuilding(prefabName).TryGetComponent(out BaseNetworkedTurret turret) ? turret.price : 0;
            
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
            if (!isValidPlacement || isColliding || !playerEconomy.CanAfford(price))
                return;

            netIdentity.RemoveClientAuthority();
            // TODO: Check for collisions based on position given by client

            var placedObject = Instantiate(buildingList.GetBuilding(prefabName));
            placedObject.transform.position = position;

            playerEconomy.Purchase(price);

            NetworkServer.Spawn(placedObject, connectionToClient);

            NetworkServer.Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("TowerPlacementArea"))
                return;
            isColliding = true;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("TowerPlacementArea"))
                return;
            isColliding = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("TowerPlacementArea"))
                return;
            isColliding = false;
        }
    }
}