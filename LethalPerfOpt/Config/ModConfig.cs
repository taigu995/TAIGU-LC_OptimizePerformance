using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TAIGU_LC_OptimizePerformance.Config
{
    public static class ModConfig
    {
        public static ConfigFile ConfigFile { get; private set; }

        // ===== 快捷键 =====
        public static ConfigEntry<KeyCode> ToggleUIKey;
        public static ConfigEntry<KeyCode> ToggleFPSKey;

        // ===== 通用设置 =====
        public static ConfigEntry<bool> EnableOnStart;
        public static ConfigEntry<string> QualityPreset;

        // ===== 优化开关 =====
        public static ConfigEntry<bool> EnableRenderOpt;
        public static ConfigEntry<bool> EnableMemoryOpt;
        public static ConfigEntry<bool> EnablePhysicsOpt;
        public static ConfigEntry<bool> EnableCullingOpt;
        public static ConfigEntry<bool> EnableParticleOpt;
        public static ConfigEntry<bool> EnableLightingOpt;
        public static ConfigEntry<bool> EnableAudioOpt;
        public static ConfigEntry<bool> EnableCameraOpt;

        // ===== 渲染优化 =====
        public static ConfigEntry<float> LODBias;
        public static ConfigEntry<bool> DisableShadows;
        public static ConfigEntry<float> MaxShadowDistance;
        public static ConfigEntry<int> ShadowResolution;
        public static ConfigEntry<float> MaxDrawDistance;
        public static ConfigEntry<int> MaxFPS;
        public static ConfigEntry<bool> EnableBatching;

        // ===== HDRP 渲染管线覆盖 (来自 LethalSponge) =====
        public static ConfigEntry<int> DecalDrawDistance;
        public static ConfigEntry<int> DecalAtlasSize;
        public static ConfigEntry<int> ShadowMaxResolution;
        public static ConfigEntry<int> ShadowAtlasSize;
        public static ConfigEntry<int> MaxCubeReflectionProbes;
        public static ConfigEntry<int> MaxPlanarReflectionProbes;
        public static ConfigEntry<float> FogBudget;
        public static ConfigEntry<bool> DeferredOnly;

        // ===== 后处理效果开关 (来自 LethalSponge) =====
        public static ConfigEntry<bool> DisableDOF;
        public static ConfigEntry<bool> DisableMotionBlur;
        public static ConfigEntry<bool> DisableBloom;
        public static ConfigEntry<bool> DisableReflections;

        // ===== 内存优化 =====
        public static ConfigEntry<float> GCInterval;
        public static ConfigEntry<bool> AggressiveGC;
        public static ConfigEntry<bool> EnableDailyCleanup;

        // ===== 资源去重 (来自 LethalSponge) =====
        public static ConfigEntry<bool> EnableMeshDedup;
        public static ConfigEntry<bool> EnableTextureDedup;
        public static ConfigEntry<bool> EnableAudioDedup;
        public static ConfigEntry<bool> EnableTextureResize;
        public static ConfigEntry<int> MaxTextureSize;

        // ===== 遮挡剔除 =====
        public static ConfigEntry<bool> EnableFrustumCulling;
        public static ConfigEntry<bool> EnableRoomCulling;
        public static ConfigEntry<float> CullingDistance;
        public static ConfigEntry<float> FrustumCullingMargin;

        // ===== 粒子优化 =====
        public static ConfigEntry<bool> DisableRainParticles;
        public static ConfigEntry<float> ParticleUpdateRate;
        public static ConfigEntry<int> MaxParticlesPerSystem;
        public static ConfigEntry<float> ParticleCullingDistance;

        // ===== 物理优化 =====
        public static ConfigEntry<float> PhysicsUpdateRate;
        public static ConfigEntry<bool> DisableRagdoll;
        public static ConfigEntry<float> PhysicsTimeStep;
        public static ConfigEntry<int> MaxPhysicsIterations;
        public static ConfigEntry<float> SleepThreshold;
        public static ConfigEntry<bool> DisableSleepingBodies;

        // ===== 灯光优化 =====
        public static ConfigEntry<float> LightIntensityMultiplier;
        public static ConfigEntry<float> LightFadeDistanceMultiplier;
        public static ConfigEntry<bool> DisableLightShadows;
        public static ConfigEntry<float> VolumetricFogDistanceMultiplier;
        public static ConfigEntry<float> VolumetricFogDistanceCap;
        public static ConfigEntry<bool> DisableDynamicShadows;
        public static ConfigEntry<float> LightCullingDistance;
        public static ConfigEntry<int> MaxDynamicLights;
        public static ConfigEntry<int> ShadowCascades;

        // ===== 摄像头优化 =====
        public static ConfigEntry<int> MapCameraFramerate;
        public static ConfigEntry<int> SecurityCameraFramerate;
        public static ConfigEntry<int> ShipCameraFramerate;
        public static ConfigEntry<bool> FixCameraSettings;
        public static ConfigEntry<bool> PatchCameraScript;
        public static ConfigEntry<bool> DisableTransparentShipCamera;
        public static ConfigEntry<bool> DisableTransparentMapCamera;

        // ===== 摄像头分辨率自定义 =====
        public static ConfigEntry<bool> EnableCameraResCustom;
        public static ConfigEntry<bool> CameraResAutoSize;
        public static ConfigEntry<int> CameraResWidth;
        public static ConfigEntry<int> CameraResHeight;
        public static ConfigEntry<bool> CheckResEveryFrame;

        // ===== 音频优化 =====
        public static ConfigEntry<bool> DisableDistantAudio;
        public static ConfigEntry<float> DistantAudioThreshold;
        public static ConfigEntry<bool> ReduceAudioUpdateRate;
        public static ConfigEntry<bool> DisableReverb;
        public static ConfigEntry<float> AudioCullingDistance;
        public static ConfigEntry<int> MaxAudioSources;
        public static ConfigEntry<int> MaxParticles;

        // ===== 内存优化增强 (来自 LethalSponge) =====
        public static ConfigEntry<bool> EnableComponentCaching;
        public static ConfigEntry<int> MaxCachedComponents;

        // ===== 网格优化 (来自 LethalSponge) =====
        public static ConfigEntry<bool> GenerateLODs;
        public static ConfigEntry<bool> FixComplexMeshes;
        public static ConfigEntry<bool> FixFoliageLOD;

        // ===== 面罩移除 =====
        public static ConfigEntry<bool> EnableVisorRemoval;

        // ===== HUD 宽高比解锁 =====
        public static ConfigEntry<bool> EnableHUDAspectRatioUnlock;
        public static ConfigEntry<float> HUDAspectRatio;

        // ===== 雾效模式 =====
        public static ConfigEntry<string> FogMode;

        public static void Init(ConfigFile file)
        {
            ConfigFile = file;

            // ===== 快捷键 =====
            ToggleUIKey = file.Bind("快捷键", "ToggleUIKey", KeyCode.F5, "切换 UI 面板的快捷键");
            ToggleFPSKey = file.Bind("快捷键", "ToggleFPSKey", KeyCode.F6, "切换 FPS 显示的快捷键");

            // ===== 通用设置 =====
            EnableOnStart = file.Bind("通用", "EnableOnStart", true, "是否在启动时自动应用优化");
            QualityPreset = file.Bind("通用", "QualityPreset", "Performance", "质量预设 (Vanilla/Balanced/Performance/Extreme)");

            // ===== 优化开关 =====
            EnableRenderOpt = file.Bind("优化开关", "EnableRenderOpt", true, "启用渲染优化");
            EnableMemoryOpt = file.Bind("优化开关", "EnableMemoryOpt", true, "启用内存优化");
            EnablePhysicsOpt = file.Bind("优化开关", "EnablePhysicsOpt", true, "启用物理优化");
            EnableCullingOpt = file.Bind("优化开关", "EnableCullingOpt", true, "启用遮挡剔除优化");
            EnableParticleOpt = file.Bind("优化开关", "EnableParticleOpt", true, "启用粒子优化");
            EnableLightingOpt = file.Bind("优化开关", "EnableLightingOpt", true, "启用灯光优化");
            EnableAudioOpt = file.Bind("优化开关", "EnableAudioOpt", true, "启用音频优化");
            EnableCameraOpt = file.Bind("优化开关", "EnableCameraOpt", true, "启用摄像头优化");

            // ===== 渲染优化 =====
            LODBias = file.Bind("渲染", "LODBias", 1f, new ConfigDescription("LOD 偏移量 (0.5=激进, 1=原版, 2=高质量)", new AcceptableValueRange<float>(0.1f, 5f)));
            DisableShadows = file.Bind("渲染", "DisableShadows", false, "禁用所有阴影");
            MaxShadowDistance = file.Bind("渲染", "MaxShadowDistance", 300f, new ConfigDescription("最大阴影距离", new AcceptableValueRange<float>(0f, 1000f)));
            ShadowResolution = file.Bind("渲染", "ShadowResolution", 2, new ConfigDescription("阴影分辨率 (0=低, 1=中, 2=高, 3=非常高)", new AcceptableValueRange<int>(0, 3)));
            MaxDrawDistance = file.Bind("渲染", "MaxDrawDistance", 5000f, new ConfigDescription("最大绘制距离", new AcceptableValueRange<float>(100f, 10000f)));
            MaxFPS = file.Bind("渲染", "MaxFPS", 0, new ConfigDescription("最大帧率限制 (0=无限制)", new AcceptableValueRange<int>(0, 300)));
            EnableBatching = file.Bind("渲染", "EnableBatching", true, "启用静态批处理");

            // ===== HDRP 渲染管线覆盖 (来自 LethalSponge) =====
            DecalDrawDistance = file.Bind("HDRP", "DecalDrawDistance", 100, new ConfigDescription("贴花绘制距离", new AcceptableValueRange<int>(10, 1000)));
            DecalAtlasSize = file.Bind("HDRP", "DecalAtlasSize", 512, new ConfigDescription("贴花图集大小", new AcceptableValueRange<int>(128, 2048)));
            ShadowMaxResolution = file.Bind("HDRP", "ShadowMaxResolution", 1024, new ConfigDescription("阴影最大分辨率", new AcceptableValueRange<int>(256, 4096)));
            ShadowAtlasSize = file.Bind("HDRP", "ShadowAtlasSize", 1024, new ConfigDescription("阴影图集大小", new AcceptableValueRange<int>(256, 4096)));
            MaxCubeReflectionProbes = file.Bind("HDRP", "MaxCubeReflectionProbes", 32, new ConfigDescription("最大立方体反射探针数", new AcceptableValueRange<int>(0, 128)));
            MaxPlanarReflectionProbes = file.Bind("HDRP", "MaxPlanarReflectionProbes", 8, new ConfigDescription("最大平面反射探针数", new AcceptableValueRange<int>(0, 32)));
            FogBudget = file.Bind("HDRP", "FogBudget", 0.33f, new ConfigDescription("雾效预算 (0=禁用, 1=完全)", new AcceptableValueRange<float>(0f, 1f)));
            DeferredOnly = file.Bind("HDRP", "DeferredOnly", false, "仅使用延迟渲染（可能导致画面异常，建议关闭）");

            // ===== 后处理效果开关 (来自 LethalSponge) =====
            DisableDOF = file.Bind("后处理", "DisableDOF", true, "禁用景深");
            DisableMotionBlur = file.Bind("后处理", "DisableMotionBlur", true, "禁用运动模糊");
            DisableBloom = file.Bind("后处理", "DisableBloom", true, "禁用光晕");
            DisableReflections = file.Bind("后处理", "DisableReflections", true, "禁用反射");

            // ===== 内存优化 =====
            GCInterval = file.Bind("内存", "GCInterval", 30f, new ConfigDescription("GC 间隔（秒）", new AcceptableValueRange<float>(5f, 300f)));
            AggressiveGC = file.Bind("内存", "AggressiveGC", false, "激进 GC 模式");
            EnableDailyCleanup = file.Bind("内存", "EnableDailyCleanup", true, "启用每日清理");

            // ===== 资源去重 (来自 LethalSponge) =====
            EnableMeshDedup = file.Bind("去重", "EnableMeshDedup", true, "启用网格去重");
            EnableTextureDedup = file.Bind("去重", "EnableTextureDedup", true, "启用纹理去重");
            EnableAudioDedup = file.Bind("去重", "EnableAudioDedup", true, "启用音频去重");
            EnableTextureResize = file.Bind("去重", "EnableTextureResize", false, "启用纹理缩放");
            MaxTextureSize = file.Bind("去重", "MaxTextureSize", 1024, new ConfigDescription("最大纹理大小", new AcceptableValueRange<int>(256, 4096)));

            // ===== 遮挡剔除 =====
            EnableFrustumCulling = file.Bind("剔除", "EnableFrustumCulling", true, "启用视锥剔除");
            EnableRoomCulling = file.Bind("剔除", "EnableRoomCulling", true, "启用房间剔除");
            CullingDistance = file.Bind("剔除", "CullingDistance", 50f, new ConfigDescription("剔除距离", new AcceptableValueRange<float>(10f, 200f)));
            FrustumCullingMargin = file.Bind("剔除", "FrustumCullingMargin", 0.1f, new ConfigDescription("视锥剔除边距", new AcceptableValueRange<float>(0f, 1f)));

            // ===== 粒子优化 =====
            DisableRainParticles = file.Bind("粒子", "DisableRainParticles", true, "禁用雨粒子");
            ParticleUpdateRate = file.Bind("粒子", "ParticleUpdateRate", 0.5f, new ConfigDescription("粒子更新率", new AcceptableValueRange<float>(0.1f, 1f)));
            MaxParticlesPerSystem = file.Bind("粒子", "MaxParticlesPerSystem", 500, new ConfigDescription("每个粒子系统最大粒子数", new AcceptableValueRange<int>(50, 5000)));
            ParticleCullingDistance = file.Bind("粒子", "ParticleCullingDistance", 30f, new ConfigDescription("粒子剔除距离", new AcceptableValueRange<float>(5f, 100f)));

            // ===== 物理优化 =====
            PhysicsUpdateRate = file.Bind("物理", "PhysicsUpdateRate", 0.5f, new ConfigDescription("物理更新率", new AcceptableValueRange<float>(0.1f, 1f)));
            DisableRagdoll = file.Bind("物理", "DisableRagdoll", false, "禁用布娃娃物理");
            PhysicsTimeStep = file.Bind("物理", "PhysicsTimeStep", 0.02f, new ConfigDescription("物理时间步长", new AcceptableValueRange<float>(0.005f, 0.1f)));
            MaxPhysicsIterations = file.Bind("物理", "MaxPhysicsIterations", 6, new ConfigDescription("最大物理迭代次数", new AcceptableValueRange<int>(1, 20)));
            SleepThreshold = file.Bind("物理", "SleepThreshold", 0.005f, new ConfigDescription("睡眠阈值", new AcceptableValueRange<float>(0.001f, 0.1f)));
            DisableSleepingBodies = file.Bind("物理", "DisableSleepingBodies", false, "禁用刚体睡眠");

            // ===== 灯光优化 =====
            LightIntensityMultiplier = file.Bind("灯光", "LightIntensityMultiplier", 0.8f, new ConfigDescription("灯光强度乘数", new AcceptableValueRange<float>(0.1f, 2f)));
            LightFadeDistanceMultiplier = file.Bind("灯光", "LightFadeDistanceMultiplier", 0.6f, new ConfigDescription("灯光淡入距离乘数", new AcceptableValueRange<float>(0.1f, 2f)));
            DisableLightShadows = file.Bind("灯光", "DisableLightShadows", true, "禁用灯光阴影");
            VolumetricFogDistanceMultiplier = file.Bind("灯光", "VolumetricFogDistanceMultiplier", 0.5f, new ConfigDescription("体积雾距离乘数", new AcceptableValueRange<float>(0.1f, 2f)));
            VolumetricFogDistanceCap = file.Bind("灯光", "VolumetricFogDistanceCap", 50f, new ConfigDescription("体积雾距离上限", new AcceptableValueRange<float>(10f, 200f)));
            DisableDynamicShadows = file.Bind("灯光", "DisableDynamicShadows", true, "禁用动态阴影");
            LightCullingDistance = file.Bind("灯光", "LightCullingDistance", 30f, new ConfigDescription("灯光剔除距离", new AcceptableValueRange<float>(5f, 100f)));
            MaxDynamicLights = file.Bind("灯光", "MaxDynamicLights", 8, new ConfigDescription("最大动态灯光数", new AcceptableValueRange<int>(0, 32)));
            ShadowCascades = file.Bind("灯光", "ShadowCascades", 2, new ConfigDescription("阴影级联数", new AcceptableValueRange<int>(0, 4)));

            // ===== 摄像头优化 =====
            MapCameraFramerate = file.Bind("摄像头", "MapCameraFramerate", 15, new ConfigDescription("地图摄像头帧率", new AcceptableValueRange<int>(5, 60)));
            SecurityCameraFramerate = file.Bind("摄像头", "SecurityCameraFramerate", 15, new ConfigDescription("监控摄像头帧率", new AcceptableValueRange<int>(5, 60)));
            ShipCameraFramerate = file.Bind("摄像头", "ShipCameraFramerate", 30, new ConfigDescription("飞船摄像头帧率", new AcceptableValueRange<int>(5, 60)));
            FixCameraSettings = file.Bind("摄像头", "FixCameraSettings", true, "修复摄像头设置");
            PatchCameraScript = file.Bind("摄像头", "PatchCameraScript", true, "修补摄像头脚本");
            DisableTransparentShipCamera = file.Bind("摄像头", "DisableTransparentShipCamera", true, "禁用透明飞船摄像头");
            DisableTransparentMapCamera = file.Bind("摄像头", "DisableTransparentMapCamera", true, "禁用透明地图摄像头");

            // ===== 摄像头分辨率自定义 =====
            EnableCameraResCustom = file.Bind("摄像头分辨率", "EnableCameraResCustom", false, "启用自定义摄像头分辨率");
            CameraResAutoSize = file.Bind("摄像头分辨率", "CameraResAutoSize", true, "自动适配屏幕分辨率");
            CameraResWidth = file.Bind("摄像头分辨率", "CameraResWidth", 1920, new ConfigDescription("自定义宽度", new AcceptableValueRange<int>(320, 3840)));
            CameraResHeight = file.Bind("摄像头分辨率", "CameraResHeight", 1080, new ConfigDescription("自定义高度", new AcceptableValueRange<int>(240, 2160)));
            CheckResEveryFrame = file.Bind("摄像头分辨率", "CheckResEveryFrame", false, "每帧检测分辨率变化");

            // ===== 音频优化 =====
            DisableDistantAudio = file.Bind("音频", "DisableDistantAudio", true, "禁用远距离音频");
            DistantAudioThreshold = file.Bind("音频", "DistantAudioThreshold", 50f, new ConfigDescription("远距离音频阈值", new AcceptableValueRange<float>(10f, 200f)));
            ReduceAudioUpdateRate = file.Bind("音频", "ReduceAudioUpdateRate", true, "降低音频更新率");
            DisableReverb = file.Bind("音频", "DisableReverb", true, "禁用混响");
            AudioCullingDistance = file.Bind("音频", "AudioCullingDistance", 100f, new ConfigDescription("音频剔除距离", new AcceptableValueRange<float>(10f, 300f)));
            MaxAudioSources = file.Bind("音频", "MaxAudioSources", 32, new ConfigDescription("最大音频源数", new AcceptableValueRange<int>(8, 128)));
            MaxParticles = file.Bind("音频", "MaxParticles", 5000, new ConfigDescription("最大粒子数", new AcceptableValueRange<int>(500, 20000)));

            // ===== 内存优化增强 (来自 LethalSponge) =====
            EnableComponentCaching = file.Bind("内存增强", "EnableComponentCaching", true, "启用组件缓存");
            MaxCachedComponents = file.Bind("内存增强", "MaxCachedComponents", 1000, new ConfigDescription("最大缓存组件数", new AcceptableValueRange<int>(100, 10000)));

            // ===== 网格优化 (来自 LethalSponge) =====
            GenerateLODs = file.Bind("网格", "GenerateLODs", true, "生成 LOD");
            FixComplexMeshes = file.Bind("网格", "FixComplexMeshes", true, "修复复杂网格");
            FixFoliageLOD = file.Bind("网格", "FixFoliageLOD", true, "修复植被 LOD 材质泄漏");

            // ===== 面罩移除 =====
            EnableVisorRemoval = file.Bind("面罩", "EnableVisorRemoval", false, "启用面罩移除");

            // ===== HUD 宽高比解锁 =====
            EnableHUDAspectRatioUnlock = file.Bind("HUD", "EnableHUDAspectRatioUnlock", false, "启用 HUD 宽高比解锁");
            HUDAspectRatio = file.Bind("HUD", "HUDAspectRatio", 1.777f, new ConfigDescription("HUD 宽高比 (1.777=16:9)", new AcceptableValueRange<float>(1f, 3f)));

            // ===== 雾效模式 =====
            FogMode = file.Bind("雾效", "FogMode", "Vanilla", new ConfigDescription("雾效模式 (Vanilla/Hide/Disable/ForceDisable)", new AcceptableValueList<string>("Vanilla", "Hide", "Disable", "ForceDisable")));
        }

        /// <summary>
        /// 重置所有配置项为默认值
        /// </summary>
        public static void ResetToDefaults()
        {
            // 通用设置
            QualityPreset.Value = "Performance";
            EnableRenderOpt.Value = true;
            EnableMemoryOpt.Value = true;
            EnablePhysicsOpt.Value = true;
            EnableCullingOpt.Value = true;
            EnableParticleOpt.Value = true;
            EnableLightingOpt.Value = true;
            EnableAudioOpt.Value = true;
            EnableCameraOpt.Value = true;

            // 渲染优化
            LODBias.Value = 1f;
            DisableShadows.Value = false;
            MaxShadowDistance.Value = 300f;
            ShadowResolution.Value = 2;
            MaxDrawDistance.Value = 5000f;
            MaxFPS.Value = 0;
            EnableBatching.Value = true;

            // HDRP
            DecalDrawDistance.Value = 100;
            DecalAtlasSize.Value = 512;
            ShadowMaxResolution.Value = 1024;
            ShadowAtlasSize.Value = 1024;
            MaxCubeReflectionProbes.Value = 32;
            MaxPlanarReflectionProbes.Value = 8;
            FogBudget.Value = 0.33f;
            DeferredOnly.Value = false;

            // 后处理
            DisableDOF.Value = true;
            DisableMotionBlur.Value = true;
            DisableBloom.Value = true;
            DisableReflections.Value = true;

            // 内存
            GCInterval.Value = 30f;
            AggressiveGC.Value = false;
            EnableDailyCleanup.Value = true;

            // 去重
            EnableMeshDedup.Value = true;
            EnableTextureDedup.Value = true;
            EnableAudioDedup.Value = true;
            EnableTextureResize.Value = false;
            MaxTextureSize.Value = 1024;

            // 剔除
            EnableFrustumCulling.Value = true;
            EnableRoomCulling.Value = true;
            CullingDistance.Value = 50f;
            FrustumCullingMargin.Value = 0.1f;

            // 粒子
            DisableRainParticles.Value = true;
            ParticleUpdateRate.Value = 0.5f;
            MaxParticlesPerSystem.Value = 500;
            ParticleCullingDistance.Value = 30f;

            // 物理
            PhysicsUpdateRate.Value = 0.5f;
            DisableRagdoll.Value = false;
            PhysicsTimeStep.Value = 0.02f;
            MaxPhysicsIterations.Value = 6;
            SleepThreshold.Value = 0.005f;
            DisableSleepingBodies.Value = false;

            // 灯光
            LightIntensityMultiplier.Value = 0.8f;
            LightFadeDistanceMultiplier.Value = 0.6f;
            DisableLightShadows.Value = true;
            VolumetricFogDistanceMultiplier.Value = 0.5f;
            VolumetricFogDistanceCap.Value = 50f;
            DisableDynamicShadows.Value = true;
            LightCullingDistance.Value = 30f;
            MaxDynamicLights.Value = 8;
            ShadowCascades.Value = 2;

            // 摄像头
            MapCameraFramerate.Value = 15;
            SecurityCameraFramerate.Value = 15;
            ShipCameraFramerate.Value = 30;
            FixCameraSettings.Value = true;
            PatchCameraScript.Value = true;
            DisableTransparentShipCamera.Value = true;
            DisableTransparentMapCamera.Value = true;

            // 摄像头分辨率
            EnableCameraResCustom.Value = false;
            CameraResAutoSize.Value = true;
            CameraResWidth.Value = 1920;
            CameraResHeight.Value = 1080;
            CheckResEveryFrame.Value = false;

            // 音频
            DisableDistantAudio.Value = true;
            DistantAudioThreshold.Value = 50f;
            ReduceAudioUpdateRate.Value = true;
            DisableReverb.Value = true;
            AudioCullingDistance.Value = 100f;
            MaxAudioSources.Value = 32;
            MaxParticles.Value = 5000;

            // 内存增强
            EnableComponentCaching.Value = true;
            MaxCachedComponents.Value = 1000;

            // 网格
            GenerateLODs.Value = true;
            FixComplexMeshes.Value = true;
            FixFoliageLOD.Value = true;

            // 面罩
            EnableVisorRemoval.Value = false;

            // HUD
            EnableHUDAspectRatioUnlock.Value = false;
            HUDAspectRatio.Value = 1.777f;

            // 雾效
            FogMode.Value = "Vanilla";

            // 快捷键
            ToggleUIKey.Value = KeyCode.F5;
            ToggleFPSKey.Value = KeyCode.F6;
        }
    }
}
