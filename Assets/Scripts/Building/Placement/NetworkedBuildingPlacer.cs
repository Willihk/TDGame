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

		private void Update()
		{
			if (!hasAuthority)
				return;
			// TODO: Place this transform on the cursor
		}

		[ServerCallback]
		private void OnTriggerEnter(Collider other)
		{
			isValidPlacement = false;
		}
		
		[ServerCallback]
		private void OnTriggerExit(Collider other)
		{
			isValidPlacement = true;
		}
	}
}
