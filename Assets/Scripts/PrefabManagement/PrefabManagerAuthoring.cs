using TDGame.Data;
using Unity.Entities;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
        public GameObjectList Prefabs;
        
#if UNITY_EDITOR
        class Baker : Baker<PrefabManagerAuthoring>
        {
            public override void Bake(PrefabManagerAuthoring authoring)
            {
                var buf = AddBuffer<PrefabElement>(GetEntity(TransformUsageFlags.None));
                
                var gameObjectList = authoring.Prefabs.GetGameObjects();
                
                for (int i = 0; i < gameObjectList.Count; i++)
                {
                    string path = AssetDatabase.GetAssetPath(gameObjectList[i]);
                    var guid = AssetDatabase.GUIDFromAssetPath(path);
                    var entity = GetEntity(gameObjectList[i], TransformUsageFlags.Dynamic);
                    
                    buf.Add(new PrefabElement{Value = entity, GUID = guid});
                }

                AddComponent(GetEntity(TransformUsageFlags.None), new PrefabManagerTag());
            }
        }
#endif

    }
}