using Mirror;
using UnityEngine;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

namespace TDGame.Example
{
    public class ExamplePlayerMovement : NetworkBehaviour
    {
        [SerializeField]
        float speed = 5;

        private void Update()
        {
            if (!hasAuthority)
                return;

            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            transform.position = new Vector3(transform.position.x + (h * speed) * Time.deltaTime, transform.position.y,transform.position.z + (v * speed) * Time.deltaTime);
        }
    }
}
