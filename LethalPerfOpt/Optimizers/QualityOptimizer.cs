using UnityEngine;
using UnityEngine.Rendering;
using TAIGU_LC_OptimizePerformance.Config;

namespace TAIGU_LC_OptimizePerformance.Optimizers
{
    /// <summary>
    /// Quality settings optimizer.
    /// Manages quality presets that combine multiple optimization settings.
    /// </summary>
    public class QualityOptimizer
    {
        private string _originalQuality;
        private bool _isApplied;

        public void Apply()
        {
            if (_isApplied) return;

            _originalQuality = ModConfig.QualityPreset.Value;
            ApplyPreset(_originalQuality);
            _isApplied = true;

            Plugin.LogSource.LogInfo($"[LethalPerfOpt:Quality] Quality preset '{_originalQuality}' applied");
        }

        public void Reapply()
        {
            _isApplied = false;
            Apply();
        }

        public void Revert()
        {
            _isApplied = false;
            Plugin.LogSource.LogInfo("[LethalPerfOpt:Quality] Quality preset reverted");
        }

        public void ApplyPreset(string preset)
        {
            switch (preset)
            {
                case "Ultra":
                    ApplyUltraSettings();
                    break;
                case "High":
                    ApplyHighSettings();
                    break;
                case "Balanced":
                    ApplyBalancedSettings();
                    break;
                case "Performance":
                    ApplyPerformanceSettings();
                    break;
                case "Extreme":
                    ApplyExtremeSettings();
                    break;
            }

            // Also update ModConfig values so UI sliders reflect the changes
            UpdateModConfigFromPreset(preset);
        }

        private void UpdateModConfigFromPreset(string preset)
        {
            // Update ModConfig values so UI sliders reflect the preset changes
            switch (preset)
            {
                case "Ultra":
                    ModConfig.LODBias.Value = 2.0f;
                    ModConfig.MaxShadowDistance.Value = 300f;
                    ModConfig.ShadowResolution.Value = 2048;
                    ModConfig.DisableShadows.Value = false;
                    break;
                case "High":
                    ModConfig.LODBias.Value = 1.5f;
                    ModConfig.MaxShadowDistance.Value = 200f;
                    ModConfig.ShadowResolution.Value = 1024;
                    ModConfig.DisableShadows.Value = false;
                    break;
                case "Balanced":
                    ModConfig.LODBias.Value = 1.0f;
                    ModConfig.MaxShadowDistance.Value = 150f;
                    ModConfig.ShadowResolution.Value = 512;
                    ModConfig.DisableShadows.Value = false;
                    break;
                case "Performance":
                    ModConfig.LODBias.Value = 0.5f;
                    ModConfig.MaxShadowDistance.Value = 80f;
                    ModConfig.ShadowResolution.Value = 256;
                    ModConfig.DisableShadows.Value = false;
                    break;
                case "Extreme":
                    ModConfig.LODBias.Value = 0.3f;
                    ModConfig.MaxShadowDistance.Value = 50f;
                    ModConfig.ShadowResolution.Value = 256;
                    ModConfig.DisableShadows.Value = true;
                    break;
            }

            Plugin.LogSource.LogInfo($"[LethalPerfOpt:Quality] ModConfig updated for preset '{preset}'");
        }

        private void ApplyUltraSettings()
        {
            // Maximum visual quality with light optimizations
            QualitySettings.lodBias = 2.0f;
            QualitySettings.shadowDistance = 300f;
            QualitySettings.shadowResolution = ShadowResolution.High;
            QualitySettings.shadows = ShadowQuality.All;
            QualitySettings.antiAliasing = 4;
            QualitySettings.vSyncCount = 1;
            QualitySettings.softParticles = true;
            QualitySettings.particleRaycastBudget = 256;

            Plugin.LogSource.LogInfo("[LethalPerfOpt:Quality] Ultra preset applied - Maximum visual quality");
        }

        private void ApplyHighSettings()
        {
            QualitySettings.lodBias = 1.5f;
            QualitySettings.shadowDistance = 200f;
            QualitySettings.shadowResolution = ShadowResolution.High;
            QualitySettings.shadows = ShadowQuality.All;
            QualitySettings.antiAliasing = 2;
            QualitySettings.vSyncCount = 1;
            QualitySettings.softParticles = true;
            QualitySettings.particleRaycastBudget = 128;

            Plugin.LogSource.LogInfo("[LethalPerfOpt:Quality] High preset applied");
        }

        private void ApplyBalancedSettings()
        {
            QualitySettings.lodBias = 1.0f;
            QualitySettings.shadowDistance = 150f;
            QualitySettings.shadowResolution = ShadowResolution.Medium;
            QualitySettings.shadows = ShadowQuality.HardOnly;
            QualitySettings.antiAliasing = 2;
            QualitySettings.vSyncCount = 0;
            QualitySettings.softParticles = false;
            QualitySettings.particleRaycastBudget = 64;

            Plugin.LogSource.LogInfo("[LethalPerfOpt:Quality] Balanced preset applied");
        }

        private void ApplyPerformanceSettings()
        {
            QualitySettings.lodBias = 0.5f;
            QualitySettings.shadowDistance = 80f;
            QualitySettings.shadowResolution = ShadowResolution.Low;
            QualitySettings.shadows = ShadowQuality.HardOnly;
            QualitySettings.antiAliasing = 0;
            QualitySettings.vSyncCount = 0;
            QualitySettings.softParticles = false;
            QualitySettings.particleRaycastBudget = 32;
            QualitySettings.pixelLightCount = 2;
            QualitySettings.skinWeights = SkinWeights.TwoBones;

            Plugin.LogSource.LogInfo("[LethalPerfOpt:Quality] Performance preset applied - Optimized for FPS");
        }

        private void ApplyExtremeSettings()
        {
            // Maximum performance, minimum visual quality
            QualitySettings.lodBias = 0.3f;
            QualitySettings.shadowDistance = 50f;
            QualitySettings.shadows = ShadowQuality.Disable;
            QualitySettings.antiAliasing = 0;
            QualitySettings.vSyncCount = 0;
            QualitySettings.softParticles = false;
            QualitySettings.particleRaycastBudget = 16;
            QualitySettings.pixelLightCount = 1;
            QualitySettings.skinWeights = SkinWeights.OneBone;
            QualitySettings.maximumLODLevel = 2;

            // Reduce render scale for extreme performance
            if (Camera.main != null)
            {
                Camera.main.allowHDR = false;
                Camera.main.allowMSAA = false;
            }

            Plugin.LogSource.LogInfo("[LethalPerfOpt:Quality] Extreme preset applied - Maximum FPS, minimum visuals");
        }

        public bool IsApplied => _isApplied;
    }
}
