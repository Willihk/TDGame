using Unity.Entities;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using Hash128 = Unity.Entities.Hash128;

namespace TDGame.PrefabManagement
{
    public struct EntityPrefabData : IComponentData
    {
        public Hash128 Value;
    }

    public class EntityPrefabAuthoring : MonoBehaviour
    {
        public GameObject Prefab;

#if UNITY_EDITOR
        class Baker : Baker<EntityPrefabAuthoring>
        {
            public override void Bake(EntityPrefabAuthoring authoring)
            {
                string path = AssetDatabase.GetAssetPath(authoring.Prefab);
                var guid = AssetDatabase.GUIDFromAssetPath(path);

                AddComponent(GetEntity(TransformUsageFlags.None), new EntityPrefabData() {Value = guid});
            }
        }
#endif

    }
}