using UnityEngine;

namespace Larje.Core.Effects
{
    [RequireComponent(typeof(ParticleSystem))]
    public sealed class ParticleSystemDestroyer : MonoBehaviour, IEffectDestroyer
    {
        [SerializeField] private bool autoDestroy = true;
        [SerializeField] private bool stopOnDestroy;
        [SerializeField] private float delay;
        [SerializeField] private GameObject targetOverride;

        private bool _destroying;

        public void DestroyEffect()
        {
            if (_destroying)
            {
                return;
            }

            _destroying = true;
            ParticleSystem particles = GetComponent<ParticleSystem>();
            if (stopOnDestroy)
            {
                particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }

            float lifetime = particles.main.duration + particles.main.startLifetime.constantMax;
            Destroy(targetOverride != null ? targetOverride : gameObject, lifetime + delay);
        }

        private void Start()
        {
            if (autoDestroy)
            {
                DestroyEffect();
            }
        }
    }
}
