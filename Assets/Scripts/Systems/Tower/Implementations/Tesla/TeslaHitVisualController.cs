using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace TDGame.Systems.Tower.Implementations.Tesla
{
    public class TeslaHitVisualController : MonoBehaviour
    {
        [SerializeField]
        private TeslaTower teslaTower;

        public GameObject vfxPrefab;

        [SerializeField]
        private Transform effectPosition;

        private List<VisualEffect> effectPool;

        private int nameId = Shader.PropertyToID("BasePosition");

        private void Start()
        {
            Setup();
        }

        void Setup()
        {
            CreateEffectPool(teslaTower.targetSystem.maxTargets);
        }

        private void Update()
        {
            if (teslaTower == null)
            {
                Destroy(this);
                return;
            }
            
            for (int i = 0; i < teslaTower.syncedTargetPositions.Count; i++)
            {
                if (i >= effectPool.Count)
                    break;

                effectPool[i].transform.position = teslaTower.syncedTargetPositions[i];
                effectPool[i].gameObject.SetActive(true);
            }
            
            for (int i = teslaTower.syncedTargetPositions.Count; i < effectPool.Count; i++)
            {
                effectPool[i].gameObject.SetActive(false);
            }
        }

        void CreateEffectPool(int poolCapacity)
        {
            effectPool = new List<VisualEffect>();

            for (int i = 0; i < poolCapacity; i++)
            {
                var spawned = Instantiate(vfxPrefab, effectPosition);

                spawned.SetActive(false);
                effectPool.Add(spawned.GetComponent<VisualEffect>());
                
                effectPool[i].SetVector3(nameId, effectPosition.position);
            }
        }
    }
}