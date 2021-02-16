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

		private Camera referenceCamera;

		private GameObject prefab;

		public override void OnStartClient()
		{
			base.OnStartClient();
			referenceCamera = Camera.main;
		}

		[Server]
		public void Setup(string prefabName)
		{
			this.prefabName = prefabName;
		}

		private void Update()
		{
			if (!hasAuthority)
				return;
			
			// TODO: Place this transform on the cursor
			
			Ray ray = referenceCamera.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
			{
				transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
			}
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
