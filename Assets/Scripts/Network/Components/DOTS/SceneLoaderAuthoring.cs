﻿using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngine;

namespace TDGame.Network.Components.DOTS
{
    // Runtime component, SceneSystem uses EntitySceneReference to identify scenes.
    public struct SceneLoader : IComponentData
    {
        public EntitySceneReference SceneReference;
    }

#if UNITY_EDITOR
// Authoring component, a SceneAsset can only be used in the Editor
    public class SceneLoaderAuthoring : MonoBehaviour
    {
        public UnityEditor.SceneAsset Scene;

        class Baker : Baker<SceneLoaderAuthoring>
        {
            public override void Bake(SceneLoaderAuthoring authoring)
            {
                var reference = new EntitySceneReference(authoring.Scene);
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new SceneLoader
                {
                    SceneReference = reference
                });
            }
        }
    }
#endif
}