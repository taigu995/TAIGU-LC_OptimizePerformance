using BepInEx.Configuration;
using UnityEngine;

namespace TAIGU_LC_OptimizePerformance.Config
{
    /// <summary>
    /// 模组配置 - 整合 LethalSponge 等优化模组的所有配置项
    /// </summary>
    public static class ModConfig
    {
        // ===== 快捷键 =====
        public static ConfigEntry<KeyCode> ToggleUIKey;
        public static ConfigEntry<KeyCode> ToggleFPSKey;

        // ===== 通用设置 =====
        public static ConfigEntry<bool> EnableOnStart;
        public static ConfigEntry<string> QualityPreset;

        // ===== 模块启用开关 =====
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

        // ===== 灯光优化 (来自 LethalSponge) =====
        public static ConfigEntry<float> LightIntensityMultiplier;
        public static ConfigEntry<float> LightFadeDistanceMultiplier;
        public static ConfigEntry<bool> DisableLightShadows;
        public static ConfigEntry<float> VolumetricFogDistanceMultiplier;
        public static ConfigEntry<float> VolumetricFogDistanceCap;
        public static ConfigEntry<bool> DisableDynamicShadows;
        public static ConfigEntry<float> LightCullingDistance;
        public static ConfigEntry<int> MaxDynamicLights;
        public static ConfigEntry<int> ShadowCascades;

        // ===== 摄像头优化 (来自 LethalSponge) =====
        public static ConfigEntry<int> MapCameraFramerate;
        public static ConfigEntry<int> SecurityCameraFramerate;
        public static ConfigEntry<int> ShipCameraFramerate;
        public static ConfigEntry<bool> FixCameraSettings;
        public static ConfigEntry<bool> PatchCameraScript;
        public static ConfigEntry<bool> DisableTransparentShipCamera;
        public static ConfigEntry<bool> DisableTransparentMapCamera;

        // ===== 音频优化 =====
        public static ConfigEntry<bool> DisableDistantAudio;
        public static ConfigEntry<float> DistantAudioThreshold;
        public static ConfigEntry<bool> ReduceAudioUpdateRate;
        public static ConfigEntry<bool> DisableReverb;
        public static ConfigEntry<float> AudioCullingDistance;
        public static ConfigEntry<int> MaxAudioSources;
        public static ConfigEntry<int> MaxParticles;

        // ===== 组件缓存 =====
        public static ConfigEntry<bool> EnableComponentCaching;
        public static ConfigEntry<int> MaxCachedComponents;

        // ===== 网格 LOD (来自 LethalSponge) =====
        public static ConfigEntry<bool> GenerateLODs;
        public static ConfigEntry<bool> FixComplexMeshes;
        public static ConfigEntry<float> ComplexMeshVertCutoff;

        // ===== 植被 LOD 修复 (来自 LethalSponge) =====
        public static ConfigEntry<bool> FixFoliageLOD;

        // ===== 输入延迟修复 (来自 LethalSponge) =====
        public static ConfigEntry<bool> FixInputActions;

        // ===== VSync (来自 LethalSponge) =====
        public static ConfigEntry<int> VSyncCount;

        // ===== 摄像头分辨率自定义 (来自 Fix-Camera-Resolution) =====
        public static ConfigEntry<bool> EnableCameraResCustom;
        public static ConfigEntry<bool> CameraResAutoSize;
        public static ConfigEntry<int> CameraResWidth;
        public static ConfigEntry<int> CameraResHeight;
        public static ConfigEntry<bool> CheckResEveryFrame;

        // ===== HDRP 后处理模式 (来自 Fix-Camera-Resolution) =====
        public static ConfigEntry<string> AntialiasingMode;
        public static ConfigEntry<string> HDRPBloomMode;
        public static ConfigEntry<string> FogMode;
        public static ConfigEntry<string> HDRPShadowMode;
        public static ConfigEntry<string> HDRPPostProcessingMode;
        public static ConfigEntry<string> HDRPVignetteMode;

        // ===== 头盔面罩移除 (来自 Fix-Camera-Resolution) =====
        public static ConfigEntry<bool> DisableVisor;

        // ===== HUD 宽高比 (来自 Fix-Camera-Resolution) =====
        public static ConfigEntry<bool> FixedAspectRatio;

        public static void Init(ConfigFile configFile)
        {
            // 快捷键
            ToggleUIKey = configFile.Bind("Hotkeys", "ToggleUI", KeyCode.F5,
                "打开/关闭性能优化面板");
            ToggleFPSKey = configFile.Bind("Hotkeys", "ToggleFPS", KeyCode.F6,
                "切换 FPS 显示");

            // 通用
            EnableOnStart = configFile.Bind("General", "EnableOnStart", true,
                "游戏启动时自动应用优化");
            QualityPreset = configFile.Bind("General", "QualityPreset", "Balanced",
                "质量预设: Ultra/High/Balanced/Performance/Extreme");

            // 模块启用开关
            EnableRenderOpt = configFile.Bind("Modules", "EnableRenderOpt", true, "启用渲染优化");
            EnableMemoryOpt = configFile.Bind("Modules", "EnableMemoryOpt", true, "启用内存优化");
            EnablePhysicsOpt = configFile.Bind("Modules", "EnablePhysicsOpt", true, "启用物理优化");
            EnableCullingOpt = configFile.Bind("Modules", "EnableCullingOpt", true, "启用遮挡剔除");
            EnableParticleOpt = configFile.Bind("Modules", "EnableParticleOpt", true, "启用粒子优化");
            EnableLightingOpt = configFile.Bind("Modules", "EnableLightingOpt", true, "启用灯光优化");
            EnableAudioOpt = configFile.Bind("Modules", "EnableAudioOpt", true, "启用音频优化");
            EnableCameraOpt = configFile.Bind("Modules", "EnableCameraOpt", true, "启用摄像头优化");

            // 渲染优化
            LODBias = configFile.Bind("Render", "LODBias", 1.0f,
                "LOD 偏移值 (越高越精细)");
            DisableShadows = configFile.Bind("Render", "DisableShadows", false,
                "禁用所有阴影");
            MaxShadowDistance = configFile.Bind("Render", "MaxShadowDistance", 150f,
                "最大阴影距离");
            ShadowResolution = configFile.Bind("Render", "ShadowResolution", 1024,
                "阴影分辨率 (256/512/1024/2048/4096)");
            MaxDrawDistance = configFile.Bind("Render", "MaxDrawDistance", 500f,
                "最大绘制距离");
            MaxFPS = configFile.Bind("Render", "MaxFPS", 0,
                "最大帧率 (0=不限制)");
            EnableBatching = configFile.Bind("Render", "EnableBatching", true,
                "启用动态合批");

            // HDRP 渲染管线覆盖
            DecalDrawDistance = configFile.Bind("HDRP", "DecalDrawDistance", 1000,
                "贴花绘制距离");
            DecalAtlasSize = configFile.Bind("HDRP", "DecalAtlasSize", 4096,
                "贴花图集尺寸");
            ShadowMaxResolution = configFile.Bind("HDRP", "ShadowMaxResolution", 2048,
                "阴影最大分辨率");
            ShadowAtlasSize = configFile.Bind("HDRP", "ShadowAtlasSize", 4096,
                "阴影图集尺寸");
            MaxCubeReflectionProbes = configFile.Bind("HDRP", "MaxCubeReflectionProbes", 48,
                "最大立方体反射探针数");
            MaxPlanarReflectionProbes = configFile.Bind("HDRP", "MaxPlanarReflectionProbes", 16,
                "最大平面反射探针数");
            FogBudget = configFile.Bind("HDRP", "FogBudget", 0.17f,
                "雾效预算 (0-1)");
            DeferredOnly = configFile.Bind("HDRP", "DeferredOnly", false,
                "仅使用延迟渲染模式");

            // 后处理效果
            DisableDOF = configFile.Bind("PostProcessing", "DisableDOF", false,
                "禁用景深效果");
            DisableMotionBlur = configFile.Bind("PostProcessing", "DisableMotionBlur", false,
                "禁用运动模糊");
            DisableBloom = configFile.Bind("PostProcessing", "DisableBloom", false,
                "禁用泛光效果");
            DisableReflections = configFile.Bind("PostProcessing", "DisableReflections", false,
                "禁用反射效果");

            // 内存优化
            GCInterval = configFile.Bind("Memory", "GCInterval", 30f,
                "GC 回收间隔（秒）");
            AggressiveGC = configFile.Bind("Memory", "AggressiveGC", false,
                "激进 GC 模式（更频繁但更轻量）");
            EnableDailyCleanup = configFile.Bind("Memory", "EnableDailyCleanup", true,
                "每日自动清理资源（参考 LethalSponge）");

            // 资源去重
            EnableMeshDedup = configFile.Bind("Dedup", "EnableMeshDedup", false,
                "启用网格去重（增加加载时间，减少内存）");
            EnableTextureDedup = configFile.Bind("Dedup", "EnableTextureDedup", false,
                "启用纹理去重（增加加载时间，减少显存）");
            EnableAudioDedup = configFile.Bind("Dedup", "EnableAudioDedup", false,
                "启用音频去重（增加加载时间，减少内存）");
            EnableTextureResize = configFile.Bind("Dedup", "EnableTextureResize", true,
                "启用纹理缩放（减少显存占用）");
            MaxTextureSize = configFile.Bind("Dedup", "MaxTextureSize", 2048,
                "纹理最大尺寸 (64/128/256/512/1024/2048)");

            // 遮挡剔除
            EnableFrustumCulling = configFile.Bind("Culling", "EnableFrustumCulling", true,
                "启用视锥剔除");
            EnableRoomCulling = configFile.Bind("Culling", "EnableRoomCulling", false,
                "启用房间剔除（可能影响游戏体验）");
            CullingDistance = configFile.Bind("Culling", "CullingDistance", 200f,
                "剔除距离");
            FrustumCullingMargin = configFile.Bind("Culling", "FrustumCullingMargin", 10f,
                "视锥剔除边距");

            // 粒子优化
            DisableRainParticles = configFile.Bind("Particles", "DisableRainParticles", false,
                "禁用雨天粒子");
            ParticleUpdateRate = configFile.Bind("Particles", "ParticleUpdateRate", 0.1f,
                "粒子更新间隔（秒）");
            MaxParticlesPerSystem = configFile.Bind("Particles", "MaxParticlesPerSystem", 100,
                "每个粒子系统最大粒子数");
            ParticleCullingDistance = configFile.Bind("Particles", "ParticleCullingDistance", 100f,
                "粒子剔除距离");

            // 物理优化
            PhysicsUpdateRate = configFile.Bind("Physics", "PhysicsUpdateRate", 0.05f,
                "物理更新间隔（秒）");
            DisableRagdoll = configFile.Bind("Physics", "DisableRagdoll", false,
                "禁用布娃娃系统");
            PhysicsTimeStep = configFile.Bind("Physics", "PhysicsTimeStep", 0.02f,
                "物理时间步长");
            MaxPhysicsIterations = configFile.Bind("Physics", "MaxPhysicsIterations", 6,
                "物理求解器最大迭代次数");
            SleepThreshold = configFile.Bind("Physics", "SleepThreshold", 0.005f,
                "刚体休眠阈值");
            DisableSleepingBodies = configFile.Bind("Physics", "DisableSleepingBodies", true,
                "优化休眠刚体");

            // 灯光优化
            LightIntensityMultiplier = configFile.Bind("Lighting", "LightIntensityMultiplier", 1.0f,
                "灯光强度乘数");
            LightFadeDistanceMultiplier = configFile.Bind("Lighting", "LightFadeDistanceMultiplier", 1.0f,
                "灯光淡入距离乘数");
            DisableLightShadows = configFile.Bind("Lighting", "DisableLightShadows", false,
                "禁用灯光阴影");
            VolumetricFogDistanceMultiplier = configFile.Bind("Lighting", "VolumetricFogDistanceMultiplier", 1.0f,
                "体积雾距离乘数");
            VolumetricFogDistanceCap = configFile.Bind("Lighting", "VolumetricFogDistanceCap", 0f,
                "体积雾距离上限 (0=不限制)");
            DisableDynamicShadows = configFile.Bind("Lighting", "DisableDynamicShadows", false,
                "禁用动态阴影");
            LightCullingDistance = configFile.Bind("Lighting", "LightCullingDistance", 100f,
                "灯光剔除距离");
            MaxDynamicLights = configFile.Bind("Lighting", "MaxDynamicLights", 8,
                "最大动态灯光数");
            ShadowCascades = configFile.Bind("Lighting", "ShadowCascades", 2,
                "阴影级联数");

            // 摄像头优化
            MapCameraFramerate = configFile.Bind("Camera", "MapCameraFramerate", 10,
                "地图摄像头帧率");
            SecurityCameraFramerate = configFile.Bind("Camera", "SecurityCameraFramerate", 15,
                "安保摄像头帧率");
            ShipCameraFramerate = configFile.Bind("Camera", "ShipCameraFramerate", 30,
                "飞船摄像头帧率");
            FixCameraSettings = configFile.Bind("Camera", "FixCameraSettings", true,
                "修复摄像头设置（参考 LethalSponge）");
            PatchCameraScript = configFile.Bind("Camera", "PatchCameraScript", true,
                "补丁摄像头脚本（视锥剔除优化）");
            DisableTransparentShipCamera = configFile.Bind("Camera", "DisableTransparentShipCamera", false,
                "禁用飞船摄像头透明渲染");
            DisableTransparentMapCamera = configFile.Bind("Camera", "DisableTransparentMapCamera", false,
                "禁用地图摄像头透明渲染");

            // 音频优化
            DisableDistantAudio = configFile.Bind("Audio", "DisableDistantAudio", false,
                "禁用远距离音频");
            DistantAudioThreshold = configFile.Bind("Audio", "DistantAudioThreshold", 10f,
                "远距离音频阈值");
            ReduceAudioUpdateRate = configFile.Bind("Audio", "ReduceAudioUpdateRate", false,
                "降低音频更新频率");
            DisableReverb = configFile.Bind("Audio", "DisableReverb", false,
                "禁用混响效果");
            AudioCullingDistance = configFile.Bind("Audio", "AudioCullingDistance", 100f,
                "音频剔除距离");
            MaxAudioSources = configFile.Bind("Audio", "MaxAudioSources", 16,
                "最大音频源数");
            MaxParticles = configFile.Bind("Particles", "MaxParticles", 500,
                "每系统最大粒子数");

            // 组件缓存
            EnableComponentCaching = configFile.Bind("Memory", "EnableComponentCaching", true,
                "启用组件缓存");
            MaxCachedComponents = configFile.Bind("Memory", "MaxCachedComponents", 500,
                "最大缓存组件数");

            // 网格 LOD
            GenerateLODs = configFile.Bind("Mesh", "GenerateLODs", true,
                "自动生成 LOD（参考 LethalSponge）");
            FixComplexMeshes = configFile.Bind("Mesh", "FixComplexMeshes", true,
                "修复复杂网格（减少顶点数）");
            ComplexMeshVertCutoff = configFile.Bind("Mesh", "ComplexMeshVertCutoff", 5000f,
                "复杂网格顶点阈值");

            // 植被 LOD 修复
            FixFoliageLOD = configFile.Bind("Fixes", "FixFoliageLOD", true,
                "修复植被 LOD 材质泄漏（参考 LethalSponge）");

            // 输入延迟修复
            FixInputActions = configFile.Bind("Fixes", "FixInputActions", true,
                "修复输入延迟（重复实例化 PlayerActions 导致）");

            // VSync
            VSyncCount = configFile.Bind("Render", "VSyncCount", 1,
                "垂直同步次数 (0=关闭, 1=60fps, 2=30fps)");

            // ===== 摄像头分辨率自定义 (来自 Fix-Camera-Resolution) =====
            EnableCameraResCustom = configFile.Bind("Camera", "EnableCameraResCustom", false,
                "启用摄像头分辨率自定义（参考 Fix-Camera-Resolution）");
            CameraResAutoSize = configFile.Bind("Camera", "CameraResAutoSize", true,
                "自动适配窗口大小");
            CameraResWidth = configFile.Bind("Camera", "CameraResWidth", 1920,
                "摄像头分辨率宽度 (10-3840)");
            CameraResHeight = configFile.Bind("Camera", "CameraResHeight", 1080,
                "摄像头分辨率高度 (10-2160)");
            CheckResEveryFrame = configFile.Bind("Camera", "CheckResEveryFrame", false,
                "每帧检测分辨率变化（可能影响性能）");

            // ===== HDRP 后处理模式 (来自 Fix-Camera-Resolution) =====
            AntialiasingMode = configFile.Bind("HDRP", "AntialiasingMode", "None",
                "抗锯齿模式: None/FXAA/TAA/SMAA");
            HDRPBloomMode = configFile.Bind("HDRP", "HDRPBloomMode", "Vanilla",
                "泛光效果: Vanilla/Disable");
            FogMode = configFile.Bind("HDRP", "FogMode", "Vanilla",
                "雾效模式: Vanilla/Hide/Disable/ForceDisable");
            HDRPShadowMode = configFile.Bind("HDRP", "HDRPShadowMode", "Vanilla",
                "阴影渲染: Vanilla/Disable");
            HDRPPostProcessingMode = configFile.Bind("HDRP", "HDRPPostProcessingMode", "Vanilla",
                "后处理效果: Vanilla/Disable");
            HDRPVignetteMode = configFile.Bind("HDRP", "HDRPVignetteMode", "Vanilla",
                "暗角效果: Vanilla/Disable");

            // ===== 头盔面罩移除 (来自 Fix-Camera-Resolution) =====
            DisableVisor = configFile.Bind("Visor", "DisableVisor", false,
                "移除头盔面罩渲染（参考 Fix-Camera-Resolution）");

            // ===== HUD 宽高比 (来自 Fix-Camera-Resolution) =====
            FixedAspectRatio = configFile.Bind("HUD", "FixedAspectRatio", true,
                "固定 HUD 宽高比（关闭以动态适配窗口）");
        }
    }
}
