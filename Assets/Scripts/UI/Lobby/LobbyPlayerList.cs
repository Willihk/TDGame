using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TDGame.UI.Lobby
{
    public class LobbyPlayerList : MonoBehaviour
    {
        [SerializeField]
        private Network.Player.PlayerList playerList;


        [SerializeField]
        private AssetReferenceGameObject entryPrefab;

        [SerializeField]
        private Transform content;


        // Handles needs to be cached, they will be used to destroy the entries.
        private List<AsyncOperationHandle<GameObject>> handles = new List<AsyncOperationHandle<GameObject>>();

        private void Start()
        {
            RefreshPlayerList();
        }

        [Button]
        public void RefreshPlayerList()
        {
            ClearList();

            GenerateEntries().Forget();
        }


        private async UniTaskVoid GenerateEntries()
        {
            for (var i = 0; i < playerList.players.Count; i++)
            {
                var id = playerList.players[i];
                var handle = Addressables.InstantiateAsync(entryPrefab);
                handles.Add(handle);

                var entry = await handle;
                entry.GetComponent<LobbyPlayerListEntry>().Initialize("Player" + id, true);
                entry.SetActive(true);
                entry.transform.SetParent(content);

                entry.transform.localScale = Vector3.one;
            }
        }

        private void ClearList()
        {
            foreach (var handle in handles)
            {
                Addressables.ReleaseInstance(handle);
            }
            
            handles.Clear();
        }
    }
}