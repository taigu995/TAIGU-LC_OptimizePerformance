using UnityEngine;
using TAIGU_LC_OptimizePerformance.Config;

namespace TAIGU_LC_OptimizePerformance.Optimizers
{
    /// <summary>
    /// Lighting optimization module.
    /// Consolidates optimizations from LightsOut and general Unity lighting optimization.
    /// Features: Dynamic light distance culling, light count limiting, shadow cascade tuning,
    /// ship lamp optimization.
    /// </summary>
    public class LightingOptimizer
    {
        private bool _isApplied;
        private float _lastUpdateTime;
        private const float UPDATE_INTERVAL = 0.2f;

        public void Apply()
        {
            if (_isApplied) return;

            _isApplied = true;
            _lastUpdateTime = 0f;

            // Apply shadow cascade settings
            ApplyShadowCascades();

            Plugin.LogSource.LogInfo("[LethalPerfOpt:Lighting] Lighting optimizations applied");
        }

        public void Revert()
        {
            if (!_isApplied) return;

            // Re-enable all lights
            EnableAllLights();

            _isApplied = false;
            Plugin.LogSource.LogInfo("[LethalPerfOpt:Lighting] Lighting optimizations reverted");
        }

        public void Update()
        {
            if (!_isApplied) return;

            float currentTime = Time.realtimeSinceStartup;
            if (currentTime - _lastUpdateTime < UPDATE_INTERVAL) return;

            _lastUpdateTime = currentTime;

            // Perform light culling
            PerformLightCulling();

            // Disable dynamic shadows if configured
            if (ModConfig.DisableDynamicShadows.Value)
            {
                DisableDynamicShadows();
            }
        }

        private void ApplyShadowCascades()
        {
            int cascades = ModConfig.ShadowCascades.Value;

            // Set shadow cascades through QualitySettings
            switch (cascades)
            {
                case 1:
                    QualitySettings.shadowCascades = 1;
                    break;
                case 2:
                    QualitySettings.shadowCascades = 2;
                    break;
                case 3:
                    QualitySettings.shadowCascades = 4; // Unity only supports 1, 2, or 4
                    break;
                case 4:
                    QualitySettings.shadowCascades = 4;
                    break;
                default:
                    QualitySettings.shadowCascades = 2;
                    break;
            }
        }

        private void PerformLightCulling()
        {
            Camera mainCam = Camera.main;
            if (mainCam == null) return;

            Vector3 camPos = mainCam.transform.position;
            float cullDist = ModConfig.LightCullingDistance.Value;
            int maxLights = ModConfig.MaxDynamicLights.Value;

            var lights = Object.FindObjectsOfType<Light>();
            int activeCount = 0;

            // Sort lights by distance to camera (closest first)
            System.Array.Sort(lights, (a, b) =>
            {
                if (a == null || b == null) return 0;
                float distA = Vector3.Distance(camPos, a.transform.position);
                float distB = Vector3.Distance(camPos, b.transform.position);
                return distA.CompareTo(distB);
            });

            foreach (var light in lights)
            {
                if (light == null || light.gameObject == null) continue;

                // Skip directional lights (sun) - always keep active
                if (light.type == LightType.Directional)
                {
                    light.enabled = true;
                    continue;
                }

                float distance = Vector3.Distance(camPos, light.transform.position);

                if (distance > cullDist || activeCount >= maxLights)
                {
                    light.enabled = false;
                }
                else
                {
                    light.enabled = true;
                    activeCount++;

                    // Reduce light range for performance at distance
                    if (distance > cullDist * 0.6f)
                    {
                        light.shadows = LightShadows.None;
                    }
                }
            }
        }

        private void DisableDynamicShadows()
        {
            var lights = Object.FindObjectsOfType<Light>();

            foreach (var light in lights)
            {
                if (light == null) continue;

                // Only disable shadows for non-directional lights
                if (light.type != LightType.Directional)
                {
                    light.shadows = LightShadows.None;
                }
            }
        }

        private void EnableAllLights()
        {
            var lights = Object.FindObjectsOfType<Light>();

            foreach (var light in lights)
            {
                if (light != null)
                {
                    light.enabled = true;
                }
            }
        }

        public bool IsApplied => _isApplied;
    }
}
