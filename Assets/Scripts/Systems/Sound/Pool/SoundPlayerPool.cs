using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TDGame.Systems.Sound.Pool
{
    public class SoundPlayerPool : MonoBehaviour
    {
        public static SoundPlayerPool Instance;
        
        private int poolSize = 10;
        private bool autoExpand = true;
        
        [SerializeField]
        private AudioSource prefabSource;

        private List<AudioSource> pool;
        private List<AudioSource> activePool;
        
        private void Awake()
        {
            Instance = this;
            
            pool = new List<AudioSource>(poolSize);
            activePool = new List<AudioSource>();
            for (int i = 0; i < poolSize; i++)
            {
               ExpandPool();
            }
        }

        private void ReturnToPool(AudioSource source)
        {
            source.Stop();
            source.clip = null;
            source.enabled = false;
            pool.Add(source);

            if (activePool.Contains(source))
            {
                activePool.Remove(source);
            }
        }

        private void ExpandPool()
        {
            var spawnedObj = Instantiate(prefabSource.gameObject, transform);
            var source = spawnedObj.GetComponent<AudioSource>();
            source.enabled = false;
            pool.Add(source);
        }

        public AudioSource GetFromPool(bool autoReturn = true)
        {
            foreach (var source in pool)
            {
                if (source.enabled) 
                    continue;
                
                source.enabled = true;
                pool.Remove(source);
                if (autoReturn)
                {
                    activePool.Add(source);
                }
                return source;
            }

            if (autoExpand)
            {
                ExpandPool();
                return GetFromPool(autoReturn);
            }

            return null;
        }

        public void PlaySoundAtPosition(Vector3 position)
        {
            var source = GetFromPool();
            source.Play();
            Invoke(nameof(ReturnToPool), source.clip.length);
        }

        private void Update()
        {
            for (int index = 0; index < activePool.Count; index++)
            {
                var source = activePool[index];
                if (!source.isPlaying)
                {
                    ReturnToPool(source);
                    break;
                }
            }
        }
    }
}