using Unity.Entities;
using UnityEditor;
using UnityEngine;
using Hash128 = Unity.Entities.Hash128;

namespace TDGame.PrefabManagement
{
   
    public struct PrefabManagerTag : IComponentData
    {
    }

    public struct PrefabElement : IBufferElementData
    {
        public Entity Value;
        public Hash128 GUID;

        public static implicit operator PrefabElement(Entity entity) => new() { Value = entity };
        public static implicit operator Entity(PrefabElement element) => element.Value;
    }
    
    public class PrefabManagerAuthoring : MonoBehaviour
    {
        public GameObject[] Prefabs;
        class Baker : Baker<PrefabManagerAuthoring>
        {
            public override void Bake(PrefabManagerAuthoring authoring)
            {
                var buf = AddBuffer<PrefabElement>(GetEntity(TransformUsageFlags.Dynamic));

                for (int i = 0; i < authoring.Prefabs.Length; i++)
                {
                    var path = AssetDatabase.GetAssetPath(authoring.Prefabs[i]);
                    var guid = AssetDatabase.GUIDFromAssetPath(path);
                    var entity = GetEntity(authoring.Prefabs[i], TransformUsageFlags.Dynamic);
                    
                    buf.Add(new PrefabElement{Value = entity, GUID = guid});
                }

                AddComponent(GetEntity(TransformUsageFlags.Dynamic), new PrefabManagerTag());
            }
        }
    }
}