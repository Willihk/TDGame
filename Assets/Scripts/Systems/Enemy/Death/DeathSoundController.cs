using TDGame.Systems.Sound.Pool;
using UnityEngine;

namespace TDGame.Systems.Enemy.Death
{
    public class DeathSoundController : MonoBehaviour
    {
        [SerializeField]
        private AudioClip deathClip;

        public void PlaySound()
        {
            var source = SoundPlayerPool.Instance.GetFromPool();
            source.clip = deathClip;
            source.spatialize = true;
            source.spatialBlend = 1;
            source.transform.position = transform.position;
            source.Play();
        }
    }
}