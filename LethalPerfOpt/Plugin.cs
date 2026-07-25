using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System.Collections;
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

            LogSource.LogInfo($"[{PluginName}] Awake() 完成，等待 Start() 创建 UI...");
        }

        private void Start()
        {
            LogSource.LogInfo($"[{PluginName}] Start() 被调用 - 正在创建 UI...");

            // 方案1: 在 Start() 中创建 UI（此时场景已加载，Unity 生命周期正常）
            CreateUIRenderer();

            // 方案2: 启动协程兜底 - 如果方案1创建的组件未被 Unity 调用，
            // 协程会在 3 秒后检测并尝试修复
            StartCoroutine(LateInitUICoroutine());

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

            // 确保激活
            uiGo.SetActive(true);

            var uiRenderer = uiGo.AddComponent<UIRenderer>();
            uiRenderer.enabled = true;
            uiRenderer.Initialize(PerfUI);

            LogSource.LogInfo($"[{PluginName}] UI Renderer 已创建: activeSelf={uiGo.activeSelf}, enabled={uiRenderer.enabled}");
        }

        /// <summary>
        /// 协程兜底：延迟检测 UI 是否正常工作
        /// 如果 Update/OnGUI 未被调用，尝试重新创建或启用
        /// </summary>
        private IEnumerator LateInitUICoroutine()
        {
            // 等待 3 秒让 Unity 完全初始化
            yield return new WaitForSeconds(3f);

            LogSource.LogInfo($"[{PluginName}][兜底] 3秒检测开始...");

            if (UIRenderer.Instance == null)
            {
                LogSource.LogWarning($"[{PluginName}][兜底] UIRenderer.Instance 为 null！尝试重新创建...");
                CreateUIRenderer();
                yield break;
            }

            // 检查 GameObject 状态
            var uiGo = GameObject.Find("TAIGU_UI_Renderer");
            if (uiGo == null)
            {
                LogSource.LogWarning($"[{PluginName}][兜底] TAIGU_UI_Renderer GameObject 不存在！重新创建...");
                CreateUIRenderer();
                yield break;
            }

            // 确保激活
            if (!uiGo.activeSelf)
            {
                LogSource.LogWarning($"[{PluginName}][兜底] GameObject 未激活，正在激活...");
                uiGo.SetActive(true);
            }

            // 确保组件启用
            var renderer = uiGo.GetComponent<UIRenderer>();
            if (renderer != null && !renderer.enabled)
            {
                LogSource.LogWarning($"[{PluginName}][兜底] UIRenderer 组件未启用，正在启用...");
                renderer.enabled = true;
            }

            LogSource.LogInfo($"[{PluginName}][兜底] 检测完成: GameObject存在={uiGo != null}, 激活={uiGo.activeSelf}");
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
