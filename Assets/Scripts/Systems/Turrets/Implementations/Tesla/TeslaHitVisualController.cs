using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Mirror;
using UnityEngine;
using UnityEngine.VFX;

namespace TDGame.Systems.Turrets.Implementations.Tesla
{
    public class TeslaHitVisualController : MonoBehaviour
    {
        [SerializeField]
        private TeslaTurret teslaTurret;

        public GameObject vfxPrefab;

        [SerializeField]
        private Transform effectPosition;

        private List<VisualEffect> effectPool;

        private int nameId = Shader.PropertyToID("Target");

        private void Start()
        {
            Setup();
        }

        void Setup()
        {
            CreateEffectPool(teslaTurret.targetSystem.maxTargets);
        }

        private void Update()
        {
            if (teslaTurret == null)
            {
                Destroy(this);
                return;
            }
            
            for (int i = 0; i < teslaTurret.syncedTargetPositions.Count; i++)
            {
                if (i >= effectPool.Count)
                    break;

                effectPool[i].transform.position = teslaTurret.syncedTargetPositions[i];
                effectPool[i].gameObject.SetActive(true);
            }
            
            for (int i = teslaTurret.syncedTargetPositions.Count; i < effectPool.Count; i++)
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