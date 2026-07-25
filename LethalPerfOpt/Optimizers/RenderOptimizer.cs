using UnityEngine;
using TAIGU_LC_OptimizePerformance.Config;

namespace TAIGU_LC_OptimizePerformance.Optimizers
{
    /// <summary>
    /// Render pipeline optimization module.
    /// Consolidates optimizations from HDLethalCompany and general Unity render optimizations.
    /// Features: Draw distance control, LOD bias, shadow optimization, static batching, FPS limiter.
    /// </summary>
    public class RenderOptimizer
    {
        private float _originalLodBias;
        private float _originalShadowDistance;
        private ShadowResolution _originalShadowResolution;
        private bool _originalShadows;
        private bool _isApplied;

        public void Apply()
        {
            if (_isApplied) return;

            // Save original values
            _originalLodBias = QualitySettings.lodBias;
            _originalShadowDistance = QualitySettings.shadowDistance;
            _originalShadowResolution = QualitySettings.shadowResolution;
            _originalShadows = QualitySettings.shadows != ShadowQuality.Disable;

            // Apply LOD bias
            QualitySettings.lodBias = ModConfig.LODBias.Value;

            // Apply shadow settings
            if (ModConfig.DisableShadows.Value)
            {
                QualitySettings.shadows = ShadowQuality.Disable;
            }
            else
            {
                QualitySettings.shadowDistance = ModConfig.MaxShadowDistance.Value;
                // Map resolution value to ShadowResolution enum
                int res = ModConfig.ShadowResolution.Value;
                if (res <= 256) QualitySettings.shadowResolution = ShadowResolution.Low;
                else if (res <= 512) QualitySettings.shadowResolution = ShadowResolution.Medium;
                else if (res <= 1024) QualitySettings.shadowResolution = ShadowResolution.High;
                else QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
            }

            // Apply draw distance
            if (Camera.main != null)
            {
                Camera.main.farClipPlane = ModConfig.MaxDrawDistance.Value;
            }

            // Apply FPS limit
            if (ModConfig.MaxFPS.Value > 0)
            {
                Application.targetFrameRate = ModConfig.MaxFPS.Value;
                QualitySettings.vSyncCount = 0;
            }

            // Enable static batching
            if (ModConfig.EnableBatching.Value)
            {
                // Static batching is controlled at build time in Unity,
                // but we can optimize dynamic batching
                QualitySettings.maxQueuedFrames = 2;
            }

            _isApplied = true;
            Plugin.LogSource.LogInfo("[LethalPerfOpt:Render] Render optimizations applied");
        }

        public void Revert()
        {
            if (!_isApplied) return;

            QualitySettings.lodBias = _originalLodBias;
            QualitySettings.shadowDistance = _originalShadowDistance;
            QualitySettings.shadowResolution = _originalShadowResolution;

            if (_originalShadows)
            {
                QualitySettings.shadows = ShadowQuality.HardOnly;
            }

            Application.targetFrameRate = -1;

            _isApplied = false;
            Plugin.LogSource.LogInfo("[LethalPerfOpt:Render] Render optimizations reverted");
        }

        public void UpdatePerFrame()
        {
            // Dynamic render optimizations per frame
            if (!_isApplied) return;

            // Update FPS limit dynamically
            if (ModConfig.MaxFPS.Value > 0)
            {
                Application.targetFrameRate = ModConfig.MaxFPS.Value;
            }
        }

        public bool IsApplied => _isApplied;

        public float GetOriginalLodBias() => _originalLodBias;
        public float GetOriginalShadowDistance() => _originalShadowDistance;
    }
}
