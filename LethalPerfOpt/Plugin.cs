using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using TAIGU_LC_OptimizePerformance.Config;
using TAIGU_LC_OptimizePerformance.UI;
using TAIGU_LC_OptimizePerformance.Optimizers;

namespace TAIGU_LC_OptimizePerformance
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGUID = "com.taigu.lc_optimizeperformance";
        public const string PluginName = "TAIGU-LC_OptimizePerformance";
        public const string PluginVersion = "1.0.0";
        public const string PluginAuthor = "TAIGU";

        internal static Plugin Instance;
        internal static ManualLogSource LogSource;
        internal static Harmony HarmonyInstance;

        // Core systems
        internal static RenderOptimizer RenderOpt;
        internal static MemoryOptimizer MemoryOpt;
        internal static PhysicsOptimizer PhysicsOpt;
        internal static CullingOptimizer CullingOpt;
        internal static QualityOptimizer QualityOpt;
        internal static ParticleOptimizer ParticleOpt;
        internal static LightingOptimizer LightingOpt;
        internal static AudioOptimizer AudioOpt;
        internal static CameraOptimizer CameraOpt;
        internal static PerformanceMonitor PerfMonitor;
        internal static PerformanceUI PerfUI;

        private void Awake()
        {
            Instance = this;
            LogSource = Logger;

            // Initialize configuration
            ModConfig.Init(Config);

            // Apply Harmony patches
            HarmonyInstance = new Harmony(PluginGUID);
            HarmonyInstance.PatchAll();

            // Initialize optimizers
            RenderOpt = new RenderOptimizer();
            MemoryOpt = new MemoryOptimizer();
            PhysicsOpt = new PhysicsOptimizer();
            CullingOpt = new CullingOptimizer();
            QualityOpt = new QualityOptimizer();
            ParticleOpt = new ParticleOptimizer();
            LightingOpt = new LightingOptimizer();
            AudioOpt = new AudioOptimizer();
            CameraOpt = new CameraOptimizer();
            PerfMonitor = new PerformanceMonitor();
            PerfUI = new PerformanceUI();

            // 创建独立的 UIRenderer 组件来渲染 UI
            var uiRenderer = gameObject.AddComponent<UIRenderer>();
            uiRenderer.Initialize(PerfUI);

            LogSource.LogInfo($"[{PluginName}] v{PluginVersion} 作者: {PluginAuthor} 加载成功！");
            LogSource.LogInfo($"[{PluginName}] 按 F5 打开性能优化面板，按 F6 切换 FPS 显示。");
        }

        private void Start()
        {
            // Apply initial optimizations based on config
            if (ModConfig.EnableOnStart.Value)
            {
                ApplyAllOptimizations();
            }
        }

        private void OnDestroy()
        {
            HarmonyInstance?.UnpatchSelf();
        }

        public static void ApplyAllOptimizations()
        {
            LogSource.LogInfo($"[{PluginName}] 正在应用全部优化...");

            if (ModConfig.EnableRenderOpt.Value)
                RenderOpt.Apply();

            if (ModConfig.EnableMemoryOpt.Value)
                MemoryOpt.Apply();

            if (ModConfig.EnablePhysicsOpt.Value)
                PhysicsOpt.Apply();

            if (ModConfig.EnableCullingOpt.Value)
                CullingOpt.Apply();

            if (ModConfig.EnableParticleOpt.Value)
                ParticleOpt.Apply();

            if (ModConfig.EnableLightingOpt.Value)
                LightingOpt.Apply();

            if (ModConfig.EnableAudioOpt.Value)
                AudioOpt.Apply();

            if (ModConfig.EnableCameraOpt.Value)
                CameraOpt.Apply();

            LogSource.LogInfo($"[{PluginName}] 全部优化已应用！");
        }

        public static void RevertAllOptimizations()
        {
            LogSource.LogInfo($"[{PluginName}] 正在恢复全部优化...");

            RenderOpt.Revert();
            MemoryOpt.Revert();
            PhysicsOpt.Revert();
            CullingOpt.Revert();
            ParticleOpt.Revert();
            LightingOpt.Revert();
            AudioOpt.Revert();
            CameraOpt.Revert();

            LogSource.LogInfo($"[{PluginName}] 全部优化已恢复！");
        }
    }
}
