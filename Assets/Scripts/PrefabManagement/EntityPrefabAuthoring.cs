using Unity.Entities;
using UnityEditor;
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

        class Baker : Baker<EntityPrefabAuthoring>
        {
            public override void Bake(EntityPrefabAuthoring authoring)
            {
                string path = AssetDatabase.GetAssetPath(authoring.Prefab);
                var guid = AssetDatabase.GUIDFromAssetPath(path);

                AddComponent(new EntityPrefabData() {Value = guid});
            }
        }
    }
}