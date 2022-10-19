using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace TDGame.PrefabManagement
{
    public class PrefabManager : MonoBehaviour
    {
        public static PrefabManager Instance;

        [Sirenix.OdinInspector.ReadOnly]
        private Entity prefabManagerSingleton;

        public PrefabManagerTag manager;


        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            // prefabManagerSingleton = SystemAPI.GetSingletonEntity<PrefabManagerTag>();
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entities = entityManager.GetAllEntities();

            foreach (var entity in entities)
            {
                if (entityManager.HasComponent<PrefabManagerTag>(entity))
                {
                    prefabManagerSingleton = entity;
                    break;
                }
            }


            Debug.Log("Found PrefabManagerTag singleton entity");
        }
    }
}