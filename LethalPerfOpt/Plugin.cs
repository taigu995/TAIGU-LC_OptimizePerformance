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
    [BepInDependency("BepInEx", BepInDependency.DependencyFlags.HardDependency)]
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
            PerfMonitor = new PerformanceMonitor();
            PerfUI = new PerformanceUI();

            LogSource.LogInfo($"[{PluginName}] v{PluginVersion} by {PluginAuthor} loaded successfully!");
            LogSource.LogInfo($"[{PluginName}] Press F5 to open the performance optimization panel.");
        }

        private void Start()
        {
            // Apply initial optimizations based on config
            if (ModConfig.EnableOnStart.Value)
            {
                ApplyAllOptimizations();
            }
        }

        private void Update()
        {
            // Handle hotkey for UI toggle
            if (Input.GetKeyDown(ModConfig.ToggleUIKey.Value))
            {
                PerfUI.IsVisible = !PerfUI.IsVisible;
                LogSource.LogDebug($"[{PluginName}] UI toggled: {PerfUI.IsVisible}");
            }

            // Handle FPS counter toggle
            if (Input.GetKeyDown(ModConfig.ToggleFPSKey.Value))
            {
                PerfUI.ShowFPS = !PerfUI.ShowFPS;
            }

            // Update performance monitor
            PerfMonitor.Update();

            // Update UI if visible
            if (PerfUI.IsVisible)
            {
                PerfUI.Update();
            }
        }

        private void OnGUI()
        {
            if (PerfUI.IsVisible || PerfUI.ShowFPS)
            {
                PerfUI.OnGUI();
            }
        }

        private void OnDestroy()
        {
            HarmonyInstance?.UnpatchSelf();
        }

        public static void ApplyAllOptimizations()
        {
            LogSource.LogInfo($"[{PluginName}] Applying all optimizations...");

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

            LogSource.LogInfo($"[{PluginName}] All optimizations applied!");
        }

        public static void RevertAllOptimizations()
        {
            LogSource.LogInfo($"[{PluginName}] Reverting all optimizations...");

            RenderOpt.Revert();
            MemoryOpt.Revert();
            PhysicsOpt.Revert();
            CullingOpt.Revert();
            ParticleOpt.Revert();
            LightingOpt.Revert();
            AudioOpt.Revert();

            LogSource.LogInfo($"[{PluginName}] All optimizations reverted!");
        }
    }
}
