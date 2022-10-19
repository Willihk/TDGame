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

        public static implicit operator PrefabElement(Entity entity) => new PrefabElement() { Value = entity };
        public static implicit operator Entity(PrefabElement element) => element.Value;
    }
    
    public class PrefabManagerAuthoring : MonoBehaviour
    {
        public GameObject[] Prefabs;
        class Baker : Baker<PrefabManagerAuthoring>
        {
            public override void Bake(PrefabManagerAuthoring authoring)
            {
                var buf = AddBuffer<PrefabElement>();

                for (int i = 0; i < authoring.Prefabs.Length; i++)
                {
                    var path = AssetDatabase.GetAssetPath(authoring.Prefabs[i]);
                    var guid = AssetDatabase.GUIDFromAssetPath(path);
                    var entity = GetEntity(authoring.Prefabs[i]);
                    
                    buf.Add(new PrefabElement{Value = entity, GUID = guid});
                }

                AddComponent( new PrefabManagerTag());
            }
        }
    }
}