using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using GameNetcodeStuff;
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

            // Apply Harmony patches - 显式 Patch() 方法，带诊断日志
            HarmonyInstance = new Harmony(PluginGUID);
            ApplyExplicitPatches();
            LogSource.LogInfo($"[{PluginName}] Harmony 补丁已应用");

            // 创建 UI 渲染器
            CreateUIRenderer();

            // 注册场景加载事件 - 每次场景加载后重激活 UI
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;

            // Apply initial optimizations based on config
            if (ModConfig.EnableOnStart.Value)
            {
                ApplyAllOptimizations();
            }

            LogSource.LogInfo($"[{PluginName}] v{PluginVersion} 作者: {PluginAuthor} 加载成功！");
            LogSource.LogInfo($"[{PluginName}] 按 F5 打开性能优化面板，按 F6 切换 FPS 显示。");
        }

        /// <summary>
        /// 显式应用 Harmony 补丁 - 每个补丁使用 AccessTools + Harmony.Patch()
        /// 带完整诊断日志，确认方法是否存在以及补丁是否成功
        /// </summary>
        private void ApplyExplicitPatches()
        {
            // 先 PatchAll() 处理 FixCameraResPatches, HUDPatches, VisorPatches 等带 [HarmonyPatch] 属性的类
            try
            {
                HarmonyInstance.PatchAll();
                LogSource.LogInfo($"[{PluginName}] PatchAll 完成");
            }
            catch (System.Exception ex)
            {
                LogSource.LogWarning($"[{PluginName}] PatchAll 部分失败: {ex.Message}，继续显式补丁...");
            }

            // ---- 补丁1: PlayerControllerB.Update ----
            var pcUpdate = AccessTools.Method(typeof(PlayerControllerB), "Update");
            if (pcUpdate != null)
            {
                LogSource.LogInfo($"[{PluginName}] 找到 PlayerControllerB.Update，正在应用补丁...");
                HarmonyInstance.Patch(pcUpdate,
                    postfix: new HarmonyMethod(typeof(Patches.InputHandler), nameof(Patches.InputHandler.PlayerControllerB_Update)));
                LogSource.LogInfo($"[{PluginName}] PlayerControllerB.Update 补丁成功");
            }
            else
            {
                LogSource.LogWarning($"[{PluginName}] 未找到 PlayerControllerB.Update！");
            }

            // ---- 补丁2: HUDManager.Update ----
            var hudUpdate = AccessTools.Method(typeof(HUDManager), "Update");
            if (hudUpdate != null)
            {
                LogSource.LogInfo($"[{PluginName}] 找到 HUDManager.Update，正在应用补丁...");
                HarmonyInstance.Patch(hudUpdate,
                    postfix: new HarmonyMethod(typeof(Patches.InputHandler), nameof(Patches.InputHandler.HUDManager_Update)));
                LogSource.LogInfo($"[{PluginName}] HUDManager.Update 补丁成功");
            }
            else
            {
                LogSource.LogWarning($"[{PluginName}] 未找到 HUDManager.Update！");
            }

            // ---- 补丁3: StartOfRound.Start ----
            var sorStart = AccessTools.Method(typeof(StartOfRound), "Start");
            if (sorStart != null)
            {
                HarmonyInstance.Patch(sorStart,
                    postfix: new HarmonyMethod(typeof(Patches.GamePatches), "StartOfRound_Start"));
                LogSource.LogInfo($"[{PluginName}] StartOfRound.Start 补丁成功");
            }

            // ---- 补丁4: StartOfRound.PassTimeToNextDay ----
            var passTime = AccessTools.Method(typeof(StartOfRound), "PassTimeToNextDay");
            if (passTime != null)
            {
                HarmonyInstance.Patch(passTime,
                    postfix: new HarmonyMethod(typeof(Patches.GamePatches), "StartOfRound_PassTimeToNextDay"));
            }

            // ---- 补丁5: RoundManager.FinishGeneratingLevel ----
            var finishGen = AccessTools.Method(typeof(RoundManager), "FinishGeneratingLevel");
            if (finishGen != null)
            {
                HarmonyInstance.Patch(finishGen,
                    postfix: new HarmonyMethod(typeof(Patches.GamePatches), "RoundManager_FinishGeneratingLevel"));
            }

            // ---- 补丁6: FoliageDetailDistance.Update ----
            var foliageUpdate = AccessTools.Method(typeof(FoliageDetailDistance), "Update");
            if (foliageUpdate != null)
            {
                HarmonyInstance.Patch(foliageUpdate,
                    prefix: new HarmonyMethod(typeof(Patches.GamePatches), "FoliageDetailDistance_Update"));
            }

            // ---- 补丁7: ManualCameraRenderer.Update ----
            var mcrUpdate = AccessTools.Method(typeof(ManualCameraRenderer), "Update");
            if (mcrUpdate != null)
            {
                HarmonyInstance.Patch(mcrUpdate,
                    postfix: new HarmonyMethod(typeof(Patches.GamePatches), "ManualCameraRenderer_Update"));
            }

            // ---- 补丁8: ManualCameraRenderer.MeetsCameraEnabledConditions ----
            var meetsCond = AccessTools.Method(typeof(ManualCameraRenderer), "MeetsCameraEnabledConditions");
            if (meetsCond != null)
            {
                HarmonyInstance.Patch(meetsCond,
                    postfix: new HarmonyMethod(typeof(Patches.GamePatches), "ManualCameraRenderer_MeetsCameraEnabledConditions"));
            }

            LogSource.LogInfo($"[{PluginName}] 显式补丁应用完毕");
        }

        /// <summary>
        /// 场景加载完成后重新激活 UI
        /// </summary>
        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            LogSource.LogInfo($"[{PluginName}] 场景加载完成: {scene.name}，正在重激活 UI...");

            var uiGo = GameObject.Find("TAIGU_UI_Renderer");
            if (uiGo == null)
            {
                LogSource.LogWarning($"[{PluginName}] UI GameObject 不存在，重新创建...");
                CreateUIRenderer();
                return;
            }

            // 确保 GameObject 和组件都激活
            if (!uiGo.activeSelf)
            {
                LogSource.LogInfo($"[{PluginName}] 重激活 UI GameObject...");
                uiGo.SetActive(true);
            }

            var renderer = uiGo.GetComponent<UIRenderer>();
            if (renderer != null && !renderer.enabled)
            {
                LogSource.LogInfo($"[{PluginName}] 启用 UIRenderer 组件...");
                renderer.enabled = true;
            }

            LogSource.LogInfo($"[{PluginName}] UI 重激活完成: activeSelf={uiGo.activeSelf}");

            // 重新应用场景级优化（灯光、音频、粒子等需要场景对象的优化）
            // 只在非菜单场景中重新应用，避免频繁重扫
            if (scene.name != "InitSceneLaunchOptions" && scene.name != "InitScene" && scene.name != "MainMenu")
            {
                LogSource.LogInfo($"[{PluginName}] 场景 {scene.name} 已加载，重新应用场景级优化...");
                if (ModConfig.EnableLightingOpt.Value) LightingOpt?.Reapply();
                if (ModConfig.EnableAudioOpt.Value) AudioOpt?.Reapply();
                if (ModConfig.EnableParticleOpt.Value) ParticleOpt?.Reapply();
                if (ModConfig.EnableCameraOpt.Value) CameraOpt?.Reapply();
                LogSource.LogInfo($"[{PluginName}] 场景级优化已重新应用");
            }
        }

        /// <summary>
        /// 创建 UI 渲染器 GameObject
        /// </summary>
        private void CreateUIRenderer()
        {
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
            if (ModConfig.EnableRenderOpt.Value) RenderOpt?.Apply();
            if (ModConfig.EnableMemoryOpt.Value) MemoryOpt?.Apply();
            if (ModConfig.EnablePhysicsOpt.Value) PhysicsOpt?.Apply();
            if (ModConfig.EnableCullingOpt.Value) CullingOpt?.Apply();
            QualityOpt?.Apply();
            if (ModConfig.EnableParticleOpt.Value) ParticleOpt?.Apply();
            if (ModConfig.EnableLightingOpt.Value) LightingOpt?.Apply();
            if (ModConfig.EnableAudioOpt.Value) AudioOpt?.Apply();
            if (ModConfig.EnableCameraOpt.Value) CameraOpt?.Apply();
            LogSource.LogInfo($"[{PluginName}] 全部优化已应用。");
        }

        public static void ReapplyAllOptimizations()
        {
            LogSource.LogInfo($"[{PluginName}] 正在重新应用全部优化...");
            if (ModConfig.EnableRenderOpt.Value) RenderOpt?.Reapply();
            if (ModConfig.EnableMemoryOpt.Value) MemoryOpt?.Reapply();
            if (ModConfig.EnablePhysicsOpt.Value) PhysicsOpt?.Reapply();
            if (ModConfig.EnableCullingOpt.Value) CullingOpt?.Reapply();
            QualityOpt?.Reapply();
            if (ModConfig.EnableParticleOpt.Value) ParticleOpt?.Reapply();
            if (ModConfig.EnableLightingOpt.Value) LightingOpt?.Reapply();
            if (ModConfig.EnableAudioOpt.Value) AudioOpt?.Reapply();
            if (ModConfig.EnableCameraOpt.Value) CameraOpt?.Reapply();
            LogSource.LogInfo($"[{PluginName}] 全部优化已重新应用。");
        }

        public static void RevertAllOptimizations()
        {
            LogSource.LogInfo($"[{PluginName}] 正在恢复全部默认...");
            RenderOpt?.Revert();
            MemoryOpt?.Revert();
            PhysicsOpt?.Revert();
            CullingOpt?.Revert();
            QualityOpt?.Revert();
            ParticleOpt?.Revert();
            LightingOpt?.Revert();
            AudioOpt?.Revert();
            CameraOpt?.Revert();
            LogSource.LogInfo($"[{PluginName}] 全部默认已恢复。");
        }
    }
}