﻿using System;
using TDGame.Data;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using Hash128 = Unity.Entities.Hash128;

namespace TDGame.PrefabManagement
{
    public class PrefabManager : MonoBehaviour
    {
        public static PrefabManager Instance;

        [SerializeField]
        private GameObjectList prefabList;

        [Sirenix.OdinInspector.ReadOnly]
        private Entity prefabManagerSingleton;


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

        public GameObject GetPrefab(Hash128 guid)
        {
            if (prefabManagerSingleton == Entity.Null)
                return null;

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var buffer = entityManager.GetBuffer<PrefabElement>(prefabManagerSingleton);

            for (int i = 0; i < buffer.Length; i++)
            {
                var entityPrefab = buffer[i];
                if (entityPrefab.GUID == guid)
                {
                    return prefabList.GetGameObject(i);
                }
            }

            return null;
        }
        
        public Entity GetEntityPrefab(Hash128 guid)
        {
            if (prefabManagerSingleton == Entity.Null)
                return Entity.Null;

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var buffer = entityManager.GetBuffer<PrefabElement>(prefabManagerSingleton);

            for (int i = 0; i < buffer.Length; i++)
            {
                var entityPrefab = buffer[i];
                if (entityPrefab.GUID == guid)
                {
                    return entityPrefab.Value;
                }
            }

            return Entity.Null;
        }

        public Hash128 GetPrefabHash(string prefabName)
        {
            int index = prefabList.GetIndexOfGameObjectName(prefabName);
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var buffer = entityManager.GetBuffer<PrefabElement>(prefabManagerSingleton);

            return buffer[index].GUID;
        }
    }
}