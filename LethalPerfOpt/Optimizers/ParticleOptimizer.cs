using UnityEngine;
using TAIGU_LC_OptimizePerformance.Config;

namespace TAIGU_LC_OptimizePerformance.Optimizers
{
    /// <summary>
    /// Particle system optimization module.
    /// Consolidates optimizations from NoRainParticles and general particle optimization.
    /// Features: Distance culling, particle count limits, rain/fog particle control, update rate throttling.
    /// </summary>
    public class ParticleOptimizer
    {
        private bool _isApplied;
        private float _lastUpdateTime;
        private int _originalMaxParticles;

        public void Apply()
        {
            if (_isApplied) return;

            _isApplied = true;
            _lastUpdateTime = 0f;

            // Disable rain particles if configured
            if (ModConfig.DisableRainParticles.Value)
            {
                DisableRainParticles();
            }

            // Apply particle limits to all existing particle systems
            ApplyParticleLimits();

            Plugin.LogSource.LogInfo("[LethalPerfOpt:Particle] Particle optimizations applied");
        }

        public void Revert()
        {
            if (!_isApplied) return;

            // Re-enable all particle systems
            EnableAllParticles();

            _isApplied = false;
            Plugin.LogSource.LogInfo("[LethalPerfOpt:Particle] Particle optimizations reverted");
        }

        public void Update()
        {
            if (!_isApplied) return;

            float currentTime = Time.realtimeSinceStartup;

            // Throttle particle updates for performance
            if (currentTime - _lastUpdateTime < ModConfig.ParticleUpdateRate.Value)
                return;

            _lastUpdateTime = currentTime;

            // Distance-based particle culling
            PerformParticleCulling();
        }

        private void DisableRainParticles()
        {
            // Find and disable rain-related particle systems
            var particleSystems = Object.FindObjectsOfType<ParticleSystem>();

            foreach (var ps in particleSystems)
            {
                if (ps == null || ps.gameObject == null) continue;

                string name = ps.gameObject.name.ToLower();
                string parentName = ps.transform.parent != null ?
                    ps.transform.parent.name.ToLower() : "";

                // Identify rain/weather particle systems
                if (name.Contains("rain") || name.Contains("weather") ||
                    name.Contains("storm") || name.Contains("precipitation") ||
                    parentName.Contains("rain") || parentName.Contains("weather"))
                {
                    var emission = ps.emission;
                    emission.enabled = false;

                    Plugin.LogSource.LogDebug($"[LethalPerfOpt:Particle] Disabled rain particle system: {ps.gameObject.name}");
                }
            }
        }

        private void ApplyParticleLimits()
        {
            var particleSystems = Object.FindObjectsOfType<ParticleSystem>();
            int maxParticles = ModConfig.MaxParticles.Value;

            foreach (var ps in particleSystems)
            {
                if (ps == null) continue;

                var main = ps.main;
                if (main.maxParticles > maxParticles)
                {
                    main.maxParticles = maxParticles;
                }
            }
        }

        private void PerformParticleCulling()
        {
            Camera mainCam = Camera.main;
            if (mainCam == null) return;

            Vector3 camPos = mainCam.transform.position;
            float cullDist = ModConfig.ParticleCullingDistance.Value;

            var particleSystems = Object.FindObjectsOfType<ParticleSystem>();

            foreach (var ps in particleSystems)
            {
                if (ps == null || ps.gameObject == null) continue;

                float distance = Vector3.Distance(camPos, ps.transform.position);

                if (distance > cullDist)
                {
                    // Disable emission for distant particles
                    if (ps.isPlaying)
                    {
                        var emission = ps.emission;
                        emission.enabled = false;
                    }
                }
                else
                {
                    // Re-enable emission for nearby particles
                    if (!ps.emission.enabled && ps.gameObject.activeSelf)
                    {
                        var emission = ps.emission;
                        emission.enabled = true;
                    }
                }
            }
        }

        private void EnableAllParticles()
        {
            var particleSystems = Object.FindObjectsOfType<ParticleSystem>();

            foreach (var ps in particleSystems)
            {
                if (ps == null) continue;

                var emission = ps.emission;
                emission.enabled = true;
            }
        }

        public bool IsApplied => _isApplied;
    }
}
