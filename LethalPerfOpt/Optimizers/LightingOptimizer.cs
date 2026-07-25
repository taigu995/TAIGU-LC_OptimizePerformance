using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using TAIGU_LC_OptimizePerformance.Config;

namespace TAIGU_LC_OptimizePerformance.Optimizers
{
    /// <summary>
    /// 灯光优化模块。
    /// 整合 LethalSponge 的 LightService 优化：
    /// - 灯光淡入距离控制
    /// - 体积光距离乘数/上限
    /// - 灯光强度调节
    /// - 灯光雾效距离控制
    /// </summary>
    public class LightingOptimizer
    {
        private bool _isApplied;
        public bool IsApplied => _isApplied;
        private readonly List<(Light light, float originalIntensity)> _modifiedLights
            = new List<(Light, float)>();
        private readonly List<(LocalVolumetricFog fog, float originalDistance)> _modifiedFogs
            = new List<(LocalVolumetricFog, float)>();

        public int ModifiedLightsCount => _modifiedLights.Count;
        public int ModifiedFogsCount => _modifiedFogs.Count;

        public void Apply()
        {
            if (_isApplied) return;

            UpdateAllLights();
            UpdateAllFogs();

            _isApplied = true;
            Plugin.LogSource.LogInfo($"[LethalPerfOpt:Lighting] 灯光优化已应用 - 修改灯光:{_modifiedLights.Count} 修改雾效:{_modifiedFogs.Count}");
        }

        /// <summary>
        /// 重新应用灯光优化（场景加载后调用，重新扫描场景中的灯光和雾效）
        /// </summary>
        public void Reapply()
        {
            _isApplied = false;
            _modifiedLights.Clear();
            _modifiedFogs.Clear();
            Apply();
        }

        public void Revert()
        {
            if (!_isApplied) return;

            // 恢复灯光强度
            foreach (var (light, originalIntensity) in _modifiedLights)
            {
                if (light != null)
                {
                    light.intensity = originalIntensity;
                }
            }
            _modifiedLights.Clear();

            // 恢复雾效距离
            foreach (var (fog, originalDistance) in _modifiedFogs)
            {
                if (fog != null)
                {
                    fog.parameters.meanFreePath = originalDistance;
                }
            }
            _modifiedFogs.Clear();

            _isApplied = false;
            Plugin.LogSource.LogInfo("[LethalPerfOpt:Lighting] 灯光优化已恢复");
        }

        /// <summary>
        /// 更新所有灯光（参考 LethalSponge 的 LightService.UpdateAllLights）
        /// </summary>
        public void UpdateAllLights()
        {
            _modifiedLights.Clear();

            float intensityMult = ModConfig.LightIntensityMultiplier.Value;
            float fadeDistanceMult = ModConfig.LightFadeDistanceMultiplier.Value;

            var allLights = Resources.FindObjectsOfTypeAll<Light>();
            foreach (var light in allLights)
            {
                if (light == null) continue;

                // 记录原始强度
                _modifiedLights.Add((light, light.intensity));

                // 调整灯光强度
                if (intensityMult != 1.0f)
                {
                    light.intensity *= intensityMult;
                }

                // 调整灯光范围（淡入距离）
                if (fadeDistanceMult != 1.0f)
                {
                    light.range *= fadeDistanceMult;
                }

                // 调整 HD 额外灯光设置
                var hdAdditionalLight = light.GetComponent<HDAdditionalLightData>();
                if (hdAdditionalLight != null)
                {
                    if (ModConfig.DisableLightShadows.Value)
                    {
                        hdAdditionalLight.SetShadowDimmer(0f);
                    }
                }
            }
        }

        /// <summary>
        /// 更新所有体积雾（参考 LethalSponge 的 FogModifier）
        /// </summary>
        private void UpdateAllFogs()
        {
            _modifiedFogs.Clear();

            float fogDistMult = ModConfig.VolumetricFogDistanceMultiplier.Value;
            float fogDistCap = ModConfig.VolumetricFogDistanceCap.Value;

            var allFogs = Resources.FindObjectsOfTypeAll<LocalVolumetricFog>();
            foreach (var fog in allFogs)
            {
                if (fog == null) continue;

                float origDistance = fog.parameters.meanFreePath;
                _modifiedFogs.Add((fog, origDistance));

                if (fogDistMult != 1.0f)
                {
                    float newDistance = origDistance * fogDistMult;
                    if (fogDistCap > 0)
                    {
                        newDistance = Mathf.Min(newDistance, fogDistCap);
                    }
                    fog.parameters.meanFreePath = newDistance;
                }
            }
        }
    }
}
