using System;
using Mirror;
using TDGame.Cursor;
using TDGame.Data;
using TDGame.Systems.Economy.Old;
using TDGame.Systems.Grid.Data;
using TDGame.Systems.Grid.InGame;
using TDGame.Systems.Tower.Base;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

namespace TDGame.Building.Placement
{
    [Obsolete]
    public class NetworkedBuildingPlacer : NetworkBehaviour
    {
        [SerializeField]
        private bool isValidGridPosition;

        private GridAreaController areaController;

        [SerializeField]
        [SyncVar]
        private string prefabName;

        [SyncVar]
        private int price;

        [SyncVar]
        private bool canAfford;

        [SerializeField]
        private GameObjectList gameObjectList;

        [SerializeField]
        private LocalCursorState cursorState;

        [SerializeField]
        private Material placementMaterial;

        private Camera referenceCamera;

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

            playerEconomy = PlayerEconomyManager.Instance.GetEconomy(connectionToClient);
        }

        private void Setup()
        {
            referenceCamera = Camera.main;

            var prefabModel = gameObjectList.GetGameObject(prefabName).transform.Find("Model").gameObject;

            var model = Instantiate(prefabModel, transform);
            localMaterial = new Material(placementMaterial);
            ReplaceModelMaterialsRecursive(model.transform);
            areaController = model.GetComponent<GridAreaController>();
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
                if (!Old_GridController.Instance || !gameObject || !areaController)
                    return;

                isValidGridPosition = Old_GridController.Instance.CanPlaceTower(gameObject, areaController.area);

                if (localMaterial)
                    localMaterial.SetInt(IsValid, isValidGridPosition ? 1 : 0);
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
                float gridOffset = 1f / Old_GridController.Instance.cellSize;
                var hitPoint = math.round(((float3)hit.point) * gridOffset) / gridOffset;

                transform.position = new Vector3(hitPoint.x, transform.position.y, hitPoint.z);
            }
            else
            {
                localMaterial.SetInt(IsValid, 0);
                return;
            }

            if (!isValidGridPosition)
                return;

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && canAfford)
            {
                cursorState.State = CursorState.None;
                Cmd_ConfirmPlacement(areaController.area);
            }
        }

        [Server]
        public void Setup(string prefabName)
        {
            this.prefabName = prefabName;
            price = gameObjectList.GetGameObject(prefabName).TryGetComponent(out BaseNetworkedTower turret) ? turret.price : 0;
        }

        [Command]
        void Cmd_CancelPlacement()
        {
            NetworkServer.Destroy(gameObject);
        }

        [Command]
        void Cmd_ConfirmPlacement(GridArea area)
        {
            if (!Old_GridController.Instance.CanPlaceTower(gameObject, area) || !playerEconomy.CanAfford(price))
                return;

            // TODO: Check for collisions based on position given by client

            var placedObject = Instantiate(gameObjectList.GetGameObject(prefabName));

            var worldSize = area.ConvertToWorldSize();
            placedObject.transform.position = area.GetWorldPosition() + (new Vector3(worldSize.x, 0, worldSize.y) * 0.5f);

            playerEconomy.Purchase(price);

            NetworkServer.Spawn(placedObject, netIdentity.connectionToClient);
            Old_GridController.Instance.PlaceTowerOnGrid(placedObject, area);

            NetworkServer.Destroy(gameObject);
        }
    }
}