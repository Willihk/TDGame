using UnityEngine;
using Mirror;
using System.Collections.Generic;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class ExamplePlayerMovement : NetworkBehaviour
{
    [SerializeField]
    public float speed;

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        gameObject.transform.position = new Vector2(transform.position.x + (h * speed) * Time.deltaTime, transform.position.y + (v * speed) * Time.deltaTime);
    }
}