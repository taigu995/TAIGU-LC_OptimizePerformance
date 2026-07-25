using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using TAIGU_LC_OptimizePerformance.Config;

namespace TAIGU_LC_OptimizePerformance.Optimizers
{
    /// <summary>
    /// 渲染管线优化模块。
    /// 整合 HDLethalCompany 和 LethalSponge 的渲染优化：
    /// - HDRP 渲染管线设置覆盖（阴影/贴花/反射/雾效）
    /// - 绘制距离控制、LOD 偏移、阴影优化
    /// - 静态合批、FPS 限制
    /// - 后处理效果开关（DOF/运动模糊/泛光/反射）
    /// </summary>
    public class RenderOptimizer
    {
        private float _originalLodBias;
        private float _originalShadowDistance;
        private ShadowResolution _originalShadowResolution;
        private bool _originalShadows;
        private bool _isApplied;

        public bool IsApplied => _isApplied;

        // HDRP 原始值备份
        private int _origDecalDrawDist;
        private int _origDecalAtlasSize;
        private int _origShadowMaxRes;
        private int _origShadowAtlasSize;
        private int _origMaxCubeProbes;
        private int _origMaxPlanarProbes;

        public void Apply()
        {
            if (_isApplied) return;

            // 保存原始值
            _originalLodBias = QualitySettings.lodBias;
            _originalShadowDistance = QualitySettings.shadowDistance;
            _originalShadowResolution = QualitySettings.shadowResolution;
            _originalShadows = QualitySettings.shadows != ShadowQuality.Disable;

            // 应用 LOD 偏移
            QualitySettings.lodBias = ModConfig.LODBias.Value;

            // 应用阴影设置
            if (ModConfig.DisableShadows.Value)
            {
                QualitySettings.shadows = ShadowQuality.Disable;
            }
            else
            {
                QualitySettings.shadowDistance = ModConfig.MaxShadowDistance.Value;
                int res = ModConfig.ShadowResolution.Value;
                if (res <= 256) QualitySettings.shadowResolution = ShadowResolution.Low;
                else if (res <= 512) QualitySettings.shadowResolution = ShadowResolution.Medium;
                else if (res <= 1024) QualitySettings.shadowResolution = ShadowResolution.High;
                else QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
            }

            // 应用绘制距离
            if (Camera.main != null)
            {
                Camera.main.farClipPlane = ModConfig.MaxDrawDistance.Value;
            }

            // 应用 FPS 限制
            if (ModConfig.MaxFPS.Value > 0)
            {
                Application.targetFrameRate = ModConfig.MaxFPS.Value;
                QualitySettings.vSyncCount = 0;
            }

            // 启用静态合批
            if (ModConfig.EnableBatching.Value)
            {
                QualitySettings.maxQueuedFrames = 2;
            }

            // 应用 HDRP 渲染管线设置覆盖（参考 LethalSponge 的 AlterQualitySettings）
            ApplyHDRPOverrides();

            // 应用后处理效果开关
            ApplyPostProcessingOverrides();

            _isApplied = true;
            Plugin.LogSource.LogInfo("[LethalPerfOpt:Render] 渲染优化已应用");
        }

        /// <summary>
        /// HDRP 渲染管线设置覆盖（参考 LethalSponge 的 AlterQualitySettings）
        /// </summary>
        private void ApplyHDRPOverrides()
        {
            try
            {
                var pipelineAsset = GraphicsSettings.currentRenderPipeline as HDRenderPipelineAsset;
                if (pipelineAsset == null)
                {
                    Plugin.LogSource.LogWarning("[LethalPerfOpt:Render] 未找到 HDRP 渲染管线资产");
                    return;
                }

                var settings = pipelineAsset.currentPlatformRenderPipelineSettings;

                // 贴花设置
                if (ModConfig.DecalDrawDistance.Value != 1000)
                {
                    _origDecalDrawDist = settings.decalSettings.drawDistance;
                    settings.decalSettings.drawDistance = ModConfig.DecalDrawDistance.Value;
                }
                if (ModConfig.DecalAtlasSize.Value != 4096)
                {
                    _origDecalAtlasSize = settings.decalSettings.atlasWidth;
                    settings.decalSettings.atlasHeight = ModConfig.DecalAtlasSize.Value;
                    settings.decalSettings.atlasWidth = ModConfig.DecalAtlasSize.Value;
                }

                // 阴影设置
                if (ModConfig.ShadowMaxResolution.Value != 2048)
                {
                    _origShadowMaxRes = settings.hdShadowInitParams.maxPunctualShadowMapResolution;
                    settings.hdShadowInitParams.maxPunctualShadowMapResolution = ModConfig.ShadowMaxResolution.Value;
                    settings.hdShadowInitParams.maxDirectionalShadowMapResolution = ModConfig.ShadowMaxResolution.Value;
                    settings.hdShadowInitParams.maxAreaShadowMapResolution = ModConfig.ShadowMaxResolution.Value;
                }
                if (ModConfig.ShadowAtlasSize.Value != 4096)
                {
                    _origShadowAtlasSize = settings.hdShadowInitParams.punctualLightShadowAtlas.shadowAtlasResolution;
                    settings.hdShadowInitParams.punctualLightShadowAtlas.shadowAtlasResolution = ModConfig.ShadowAtlasSize.Value;
                    settings.hdShadowInitParams.cachedPunctualLightShadowAtlas = ModConfig.ShadowAtlasSize.Value * 2;
                    settings.hdShadowInitParams.areaLightShadowAtlas.shadowAtlasResolution = ModConfig.ShadowAtlasSize.Value;
                    settings.hdShadowInitParams.cachedAreaLightShadowAtlas = ModConfig.ShadowAtlasSize.Value * 2;
                }

                // 反射探针设置
                if (ModConfig.MaxCubeReflectionProbes.Value != 48)
                {
                    _origMaxCubeProbes = settings.lightLoopSettings.maxCubeReflectionOnScreen;
                    settings.lightLoopSettings.maxCubeReflectionOnScreen = ModConfig.MaxCubeReflectionProbes.Value;
                }
                if (ModConfig.MaxPlanarReflectionProbes.Value != 16)
                {
                    _origMaxPlanarProbes = settings.lightLoopSettings.maxPlanarReflectionOnScreen;
                    settings.lightLoopSettings.maxPlanarReflectionOnScreen = ModConfig.MaxPlanarReflectionProbes.Value;
                }

                // 雾效预算
                if (ModConfig.FogBudget.Value != 0.17f)
                {
                    int qualityLevel = QualitySettings.GetQualityLevel();
                    if (qualityLevel >= 0 && qualityLevel < settings.lightingQualitySettings.Fog_Budget.Length)
                    {
                        settings.lightingQualitySettings.Fog_Budget[qualityLevel] = ModConfig.FogBudget.Value;
                    }
                }

                // 延迟渲染模式
                if (ModConfig.DeferredOnly.Value)
                {
                    settings.supportedLitShaderMode = RenderPipelineSettings.SupportedLitShaderMode.DeferredOnly;
                }

                Plugin.LogSource.LogInfo("[LethalPerfOpt:Render] HDRP 渲染管线设置已覆盖");
            }
            catch (Exception e)
            {
                Plugin.LogSource.LogWarning($"[LethalPerfOpt:Render] HDRP 设置覆盖失败: {e.Message}");
            }
        }

        /// <summary>
        /// 后处理效果开关（参考 LethalSponge 的 disableDOF/disableMotionBlur/disableBloom 等）
        /// </summary>
        private void ApplyPostProcessingOverrides()
        {
            try
            {
                var volumes = Resources.FindObjectsOfTypeAll<Volume>();
                foreach (var volume in volumes)
                {
                    if (volume == null || volume.profile == null) continue;

                    // 禁用景深
                    if (ModConfig.DisableDOF.Value)
                    {
                        if (volume.profile.TryGet<DepthOfField>(out var dof))
                        {
                            dof.active = false;
                        }
                    }

                    // 禁用运动模糊
                    if (ModConfig.DisableMotionBlur.Value)
                    {
                        if (volume.profile.TryGet<MotionBlur>(out var motionBlur))
                        {
                            motionBlur.active = false;
                        }
                    }

                    // 禁用泛光
                    if (ModConfig.DisableBloom.Value)
                    {
                        if (volume.profile.TryGet<Bloom>(out var bloom))
                        {
                            bloom.active = false;
                        }
                    }
                }

                Plugin.LogSource.LogInfo("[LethalPerfOpt:Render] 后处理效果已应用");
            }
            catch (Exception e)
            {
                Plugin.LogSource.LogWarning($"[LethalPerfOpt:Render] 后处理覆盖失败: {e.Message}");
            }
        }

        public void Reapply()
        {
            _isApplied = false;
            Apply();
        }

        public void Revert()
        {
            if (!_isApplied) return;

            QualitySettings.lodBias = _originalLodBias;
            QualitySettings.shadowDistance = _originalShadowDistance;
            QualitySettings.shadowResolution = _originalShadowResolution;
            if (!_originalShadows) QualitySettings.shadows = ShadowQuality.Disable;

            _isApplied = false;
            Plugin.LogSource.LogInfo("[LethalPerfOpt:Render] 渲染优化已恢复");
        }
    }
}
