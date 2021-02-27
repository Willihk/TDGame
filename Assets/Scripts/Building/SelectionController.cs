using Mirror;
using TDGame.Cursor;
using TDGame.Events.Base;
using UnityEngine;

namespace TDGame
{
    public class SelectionController : MonoBehaviour
    {
        private Camera referenceCamera;

        private GameObject selectedTower;

        [SerializeField]
        private Material rangeCircleMaterial;

        [SerializeField]
        private LocalCursorState cursorState;

        [SerializeField]
        private GameEvent<GameObject> gameEvent;

        private void Start()
        {
            referenceCamera = Camera.main;
        }

        void Update()
        {
            Ray ray = referenceCamera.ScreenPointToRay(Input.mousePosition);
            if (Input.GetMouseButtonDown(0) && cursorState.State == CursorState.None && Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Tower")))
            {
                GameObject hitPoint = hit.collider.gameObject;
                FindTowerCoreRecursive(hitPoint.transform);
            }
            else if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                CheckIfAlreadySelecting();
            }
        }

        private void CheckIfAlreadySelecting()
        {
            if (selectedTower != null)
            {
                selectedTower = null;
                gameEvent.Raise(selectedTower);
            }
        }

        private void FindTowerCoreRecursive(Transform transform)
        {
            if (transform.GetComponent(typeof(NetworkIdentity)))
            {
                if (selectedTower == transform.gameObject)
                    return;

                CheckIfAlreadySelecting();

                selectedTower = transform.gameObject;
                gameEvent.Raise(selectedTower);
            }
            else
                FindTowerCoreRecursive(transform.parent);
        }
    }
}
