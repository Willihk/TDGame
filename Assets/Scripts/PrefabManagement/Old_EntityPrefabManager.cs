using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Scenes;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TDGame.PrefabManagement
{
    public class Old_EntityPrefabManager : MonoBehaviour
    {
        public static Old_EntityPrefabManager Instance;

        public readonly Dictionary<string, Entity> Prefabs = new Dictionary<string, Entity>();

        EntityManager entityManager;

        BlobAssetStore assetStore;

        private void Awake()
        {
            if (Instance is null)
                Instance = this;
        }

        private void Start()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            assetStore = new BlobAssetStore();
        }

        public Entity ConvertGameObjectToEntity(GameObject gameObject)
        {
            var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, assetStore);

            var entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(gameObject, settings);

            entityManager.AddComponent<Disabled>(entity);
            entityManager.AddComponent<Prefab>(entity);

            Prefabs.Add(gameObject.name, entity);

            return entity;
        }

        public Entity SpawnEntityPrefab(string name)
        {
            if (Prefabs.TryGetValue(name, out Entity prefab))
            {
                Entity entity = entityManager.Instantiate(prefab);
                entityManager.RemoveComponent<Disabled>(entity);
                entityManager.RemoveComponent<Prefab>(entity);
                return entity;
            }

            return Entity.Null;
        }

        public Entity SpawnEntityPrefab(EntityCommandBuffer commandBuffer, string name)
        {
            var prefab = GetEntityPrefab(name);
            if (prefab != Entity.Null)
            {
                Entity entity = commandBuffer.Instantiate(prefab);
                commandBuffer.RemoveComponent<Disabled>(entity);
                return entity;
            }

            return Entity.Null;
        }

        public Entity GetEntityPrefab(string name)
        {
            if (Prefabs.TryGetValue(name, out Entity prefab))
            {
                return prefab;
            }

            return Entity.Null;
        }

        // public Entity SpawnVisualOnlyPrefab(string name)
        // {
        //     var prefab = GetEntityPrefab(name);
        //     if (prefab != Entity.Null)
        //     {
        //         var entity = entityManager.CreateEntity(
        //             ComponentType.ReadOnly<Translation>(),
        //             ComponentType.ReadOnly<Rotation>(),
        //             ComponentType.ReadOnly<Scale>(),
        //             ComponentType.ReadOnly<RenderMesh>(),
        //             ComponentType.ReadOnly<RenderBounds>(),
        //             ComponentType.ReadOnly<WorldRenderBounds>(),
        //             ComponentType.ReadOnly<LocalToWorld>());
        //
        //         if (entityManager.HasComponent<NonUniformScale>(prefab))
        //             entityManager.AddComponentData(entity, entityManager.GetComponentData<NonUniformScale>(prefab));
        //         else if (entityManager.HasComponent<Scale>(prefab))
        //             entityManager.AddComponentData(entity, entityManager.GetComponentData<Scale>(prefab));
        //         else
        //             entityManager.AddComponentData(entity, new Scale { Value = 1 });
        //
        //         entityManager.AddComponentData(entity, entityManager.GetComponentData<Translation>(prefab));
        //         entityManager.AddComponentData(entity, entityManager.GetComponentData<Rotation>(prefab));
        //         entityManager.AddComponentData(entity, entityManager.GetComponentData<RenderBounds>(prefab));
        //         entityManager.AddSharedComponentData(entity, entityManager.GetSharedComponentData<RenderMesh>(prefab));
        //         return entity;
        //     }
            // return Entity.Null;
        // }
    }
}