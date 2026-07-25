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

            LogSource.LogInfo($"[{PluginName}] v{PluginVersion} 作者: {PluginAuthor} - Awake() 开始");

            // Initialize configuration
            ModConfig.Init(Config);

            // Apply Harmony patches
            // 注意: 使用 PatchAll() 自动扫描所有 [HarmonyPatch] 类
            // GUIRenderPatch 现在补丁 HUDManager.Update (存在的方法)
            HarmonyInstance = new Harmony(PluginGUID);
            HarmonyInstance.PatchAll();
            LogSource.LogInfo($"[{PluginName}] Harmony 补丁已应用");

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

            // 直接在 Awake() 中创建 UI（避免 Start() 不被调用的问题）
            CreateUIRenderer();

            // Apply initial optimizations based on config
            if (ModConfig.EnableOnStart.Value)
            {
                ApplyAllOptimizations();
            }

            LogSource.LogInfo($"[{PluginName}] v{PluginVersion} 作者: {PluginAuthor} 加载成功！");
            LogSource.LogInfo($"[{PluginName}] 按 F5 打开性能优化面板，按 F6 切换 FPS 显示。");
        }

        /// <summary>
        /// 创建 UI 渲染器 GameObject
        /// </summary>
        private void CreateUIRenderer()
        {
            // 检查是否已存在（防止重复创建）
            var existing = GameObject.Find("TAIGU_UI_Renderer");
            if (existing != null)
            {
                LogSource.LogInfo($"[{PluginName}] UI Renderer 已存在，跳过创建");
                var existingRenderer = existing.GetComponent<UIRenderer>();
                if (existingRenderer != null && UIRenderer.Instance == null)
                {
                    existingRenderer.Initialize(PerfUI);
                }
                return;
            }

            var uiGo = new GameObject("TAIGU_UI_Renderer");
            DontDestroyOnLoad(uiGo);
            uiGo.SetActive(true);

            var uiRenderer = uiGo.AddComponent<UIRenderer>();
            uiRenderer.enabled = true;
            uiRenderer.Initialize(PerfUI);

            LogSource.LogInfo($"[{PluginName}] UI Renderer 已创建: activeSelf={uiGo.activeSelf}, enabled={uiRenderer.enabled}");
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

            // Quality optimizer is always applied (no separate toggle)
            QualityOpt.Apply();

            if (ModConfig.EnableParticleOpt.Value)
                ParticleOpt.Apply();

            if (ModConfig.EnableLightingOpt.Value)
                LightingOpt.Apply();

            if (ModConfig.EnableAudioOpt.Value)
                AudioOpt.Apply();

            if (ModConfig.EnableCameraOpt.Value)
                CameraOpt.Apply();

            LogSource.LogInfo($"[{PluginName}] 全部优化已应用。");
        }

        public static void RevertAllOptimizations()
        {
            LogSource.LogInfo($"[{PluginName}] 正在恢复全部默认...");

            RenderOpt.Revert();
            MemoryOpt.Revert();
            PhysicsOpt.Revert();
            CullingOpt.Revert();
            QualityOpt.Revert();
            ParticleOpt.Revert();
            LightingOpt.Revert();
            AudioOpt.Revert();
            CameraOpt.Revert();

            LogSource.LogInfo($"[{PluginName}] 全部默认已恢复。");
        }
    }
}