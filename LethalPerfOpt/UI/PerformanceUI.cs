using UnityEngine;
using TAIGU_LC_OptimizePerformance.Config;
using TAIGU_LC_OptimizePerformance.Optimizers;

namespace TAIGU_LC_OptimizePerformance.UI
{
    /// <summary>
    /// 性能优化主面板，按 F5 唤出，显示 FPS 及各模块开关控制
    /// </summary>
    public class PerformanceUI
    {
        public bool IsVisible;
        public bool ShowFPS;

        private Rect _windowRect;
        private Vector2 _scrollPosition;
        private int _selectedTab;
        private bool _stylesInitialized;

        private readonly string[] _tabNames = new string[]
        {
            "概览", "渲染", "内存", "物理", "遮挡剔除",
            "粒子", "灯光", "音频", "预设"
        };

        public PerformanceUI()
        {
            _windowRect = new Rect(20, 20, 560, 640);
        }

        public void Update() { }

        public void OnGUI()
        {
            if (!_stylesInitialized)
            {
                UIStyles.Init();
                _stylesInitialized = true;
            }

            if (ShowFPS && !IsVisible)
                DrawFPSCounter();

            if (IsVisible)
            {
                GUI.ModalWindow(9527, _windowRect, DrawMainWindow,
                    "TAIGU-LC_OptimizePerformance v1.0.0 - 终极性能优化套件",
                    UIStyles.WindowStyle);
            }
        }

        private void DrawFPSCounter()
        {
            var monitor = Plugin.PerfMonitor;
            float fps = monitor.CurrentFPS;
            var fpsStyle = UIStyles.GetFPSStyle(fps);
            GUI.Label(new Rect(10, 10, 130, 30),
                $"FPS: {fps:F1}", fpsStyle);
        }

        private void DrawMainWindow(int windowId)
        {
            GUILayout.Label("TAIGU-LC_OptimizePerformance - 终极性能优化套件", UIStyles.HeaderStyle);
            GUILayout.Space(5);

            _selectedTab = GUILayout.Toolbar(_selectedTab, _tabNames, UIStyles.ButtonStyle);
            GUILayout.Space(8);

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            switch (_selectedTab)
            {
                case 0: DrawOverviewTab(); break;
                case 1: DrawRenderTab(); break;
                case 2: DrawMemoryTab(); break;
                case 3: DrawPhysicsTab(); break;
                case 4: DrawCullingTab(); break;
                case 5: DrawParticlesTab(); break;
                case 6: DrawLightingTab(); break;
                case 7: DrawAudioTab(); break;
                case 8: DrawPresetsTab(); break;
            }

            GUILayout.EndScrollView();

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("应用全部优化", UIStyles.ButtonStyle))
                Plugin.ApplyAllOptimizations();

            if (GUILayout.Button("恢复全部默认", UIStyles.ButtonStyle))
                Plugin.RevertAllOptimizations();

            if (GUILayout.Button("重置统计", UIStyles.ButtonStyle))
                Plugin.PerfMonitor.ResetStats();

            GUILayout.EndHorizontal();
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        private void DrawOverviewTab()
        {
            var monitor = Plugin.PerfMonitor;

            GUILayout.BeginHorizontal();
            GUILayout.Label("当前帧率:", UIStyles.BoldLabelStyle, GUILayout.Width(120));
            float fps = monitor.CurrentFPS;
            GUIStyle fpsStyle = fps >= 60 ? UIStyles.GoodStyle :
                               fps >= 30 ? UIStyles.WarnStyle : UIStyles.BadStyle;
            GUILayout.Label($"{fps:F1} FPS", fpsStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("帧生成时间:", UIStyles.BoldLabelStyle, GUILayout.Width(120));
            GUILayout.Label($"{monitor.CurrentFrameTime:F2} 毫秒", UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("平均 / 最低 / 最高:", UIStyles.BoldLabelStyle, GUILayout.Width(120));
            GUILayout.Label($"{monitor.AvgFPS:F1} / {monitor.MinFPS:F1} / {monitor.MaxFPS:F1}",
                UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label("--- 系统状态 ---", UIStyles.SectionHeaderStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label("内存占用:", UIStyles.BoldLabelStyle, GUILayout.Width(120));
            GUILayout.Label($"{monitor.CurrentMemoryMB} MB (峰值: {monitor.PeakMemoryMB} MB)",
                UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("活跃渲染器:", UIStyles.BoldLabelStyle, GUILayout.Width(120));
            GUILayout.Label($"{monitor.ActiveRenderers}", UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("活跃灯光:", UIStyles.BoldLabelStyle, GUILayout.Width(120));
            GUILayout.Label($"{monitor.ActiveLights}", UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("活跃音频:", UIStyles.BoldLabelStyle, GUILayout.Width(120));
            GUILayout.Label($"{monitor.ActiveAudioSources}", UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("活跃粒子:", UIStyles.BoldLabelStyle, GUILayout.Width(120));
            GUILayout.Label($"{monitor.ActiveParticles}", UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label("--- 优化模块状态 ---", UIStyles.SectionHeaderStyle);

            DrawModuleStatus("渲染", Plugin.RenderOpt?.IsApplied ?? false);
            DrawModuleStatus("内存", Plugin.MemoryOpt?.IsApplied ?? false);
            DrawModuleStatus("物理", Plugin.PhysicsOpt?.IsApplied ?? false);
            DrawModuleStatus("遮挡剔除", Plugin.CullingOpt?.IsApplied ?? false);
            DrawModuleStatus("粒子", Plugin.ParticleOpt?.IsApplied ?? false);
            DrawModuleStatus("灯光", Plugin.LightingOpt?.IsApplied ?? false);
            DrawModuleStatus("音频", Plugin.AudioOpt?.IsApplied ?? false);

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("组件缓存:", UIStyles.BoldLabelStyle, GUILayout.Width(120));
            GUILayout.Label($"命中: {MemoryOptimizer.CacheHits} | 未命中: {MemoryOptimizer.CacheMisses} | 命中率: {MemoryOptimizer.GetCacheHitRate():F1}%",
                UIStyles.LabelStyle);
            GUILayout.EndHorizontal();
        }

        private void DrawModuleStatus(string name, bool isActive)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name + ":", UIStyles.BoldLabelStyle, GUILayout.Width(120));
            GUILayout.Label(isActive ? "已启用" : "未启用",
                isActive ? UIStyles.GoodStyle : UIStyles.WarnStyle);
            GUILayout.EndHorizontal();
        }

        private void DrawRenderTab()
        {
            GUILayout.Label("渲染优化设置", UIStyles.SectionHeaderStyle);
            GUILayout.Space(5);

            ModConfig.EnableRenderOpt.Value = GUILayout.Toggle(
                ModConfig.EnableRenderOpt.Value, "启用渲染优化", UIStyles.ToggleStyle);

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("LOD 偏差:", UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.LODBias.Value = GUILayout.HorizontalSlider(
                ModConfig.LODBias.Value, 0.1f, 3.0f);
            GUILayout.Label($"{ModConfig.LODBias.Value:F2}", UIStyles.LabelStyle, GUILayout.Width(40));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("绘制距离:", UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.MaxDrawDistance.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.MaxDrawDistance.Value, 100, 2000);
            GUILayout.Label($"{ModConfig.MaxDrawDistance.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("阴影分辨率:", UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.ShadowResolution.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.ShadowResolution.Value, 256, 4096);
            GUILayout.Label($"{ModConfig.ShadowResolution.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("阴影距离:", UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.MaxShadowDistance.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.MaxShadowDistance.Value, 50, 500);
            GUILayout.Label($"{ModConfig.MaxShadowDistance.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("帧率上限 (0=不限):", UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.MaxFPS.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.MaxFPS.Value, 0, 300);
            GUILayout.Label($"{ModConfig.MaxFPS.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            ModConfig.DisableShadows.Value = GUILayout.Toggle(
                ModConfig.DisableShadows.Value, "禁用全部阴影", UIStyles.ToggleStyle);
            ModConfig.EnableBatching.Value = GUILayout.Toggle(
                ModConfig.EnableBatching.Value, "启用静态批处理", UIStyles.ToggleStyle);

            GUILayout.Space(10);
            if (GUILayout.Button("应用渲染设置", UIStyles.ButtonStyle))
                Plugin.RenderOpt?.Apply();
        }

        private void DrawMemoryTab()
        {
            GUILayout.Label("内存 & GC 优化", UIStyles.SectionHeaderStyle);
            GUILayout.Space(5);

            ModConfig.EnableMemoryOpt.Value = GUILayout.Toggle(
                ModConfig.EnableMemoryOpt.Value, "启用内存优化", UIStyles.ToggleStyle);
            ModConfig.EnableComponentCaching.Value = GUILayout.Toggle(
                ModConfig.EnableComponentCaching.Value, "启用组件缓存 (LethalPerformance 风格)",
                UIStyles.ToggleStyle);
            ModConfig.AggressiveGC.Value = GUILayout.Toggle(
                ModConfig.AggressiveGC.Value, "激进 GC 模式", UIStyles.ToggleStyle);

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("GC 间隔 (秒):", UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.GCInterval.Value = GUILayout.HorizontalSlider(
                ModConfig.GCInterval.Value, 1.0f, 30.0f);
            GUILayout.Label($"{ModConfig.GCInterval.Value:F1}", UIStyles.LabelStyle, GUILayout.Width(40));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("最大缓存数:", UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.MaxCachedComponents.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.MaxCachedComponents.Value, 100, 2000);
            GUILayout.Label($"{ModConfig.MaxCachedComponents.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label("--- 缓存统计 ---", UIStyles.SectionHeaderStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label("缓存大小:", UIStyles.LabelStyle, GUILayout.Width(160));
            GUILayout.Label($"{MemoryOptimizer.CacheSize}", UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("命中率:", UIStyles.LabelStyle, GUILayout.Width(160));
            GUILayout.Label($"{MemoryOptimizer.GetCacheHitRate():F1}%", UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("内存用量:", UIStyles.LabelStyle, GUILayout.Width(160));
            GUILayout.Label($"{Plugin.MemoryOpt?.GetMemoryUsageMB()} MB", UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            if (GUILayout.Button("立即强制 GC", UIStyles.ButtonStyle))
            {
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.GC.Collect();
            }
        }

        private void DrawPhysicsTab()
        {
            GUILayout.Label("物理引擎优化", UIStyles.SectionHeaderStyle);
            GUILayout.Space(5);

            ModConfig.EnablePhysicsOpt.Value = GUILayout.Toggle(
                ModConfig.EnablePhysicsOpt.Value, "启用物理优化", UIStyles.ToggleStyle);
            ModConfig.DisableSleepingBodies.Value = GUILayout.Toggle(
                ModConfig.DisableSleepingBodies.Value, "优化休眠刚体", UIStyles.ToggleStyle);

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("物理时间步长:", UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.PhysicsTimeStep.Value = GUILayout.HorizontalSlider(
                ModConfig.PhysicsTimeStep.Value, 0.01f, 0.05f);
            GUILayout.Label($"{ModConfig.PhysicsTimeStep.Value:F3}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("求解器迭代次数:", UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.MaxPhysicsIterations.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.MaxPhysicsIterations.Value, 1, 20);
            GUILayout.Label($"{ModConfig.MaxPhysicsIterations.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("休眠阈值:", UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.SleepThreshold.Value = GUILayout.HorizontalSlider(
                ModConfig.SleepThreshold.Value, 0.001f, 0.1f);
            GUILayout.Label($"{ModConfig.SleepThreshold.Value:F4}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            if (GUILayout.Button("应用物理设置", UIStyles.ButtonStyle))
                Plugin.PhysicsOpt?.Apply();
        }

        private void DrawCullingTab()
        {
            GUILayout.Label("遮挡剔除优化 (CullFactory 风格)", UIStyles.SectionHeaderStyle);
            GUILayout.Space(5);

            ModConfig.EnableCullingOpt.Value = GUILayout.Toggle(
                ModConfig.EnableCullingOpt.Value, "启用遮挡剔除优化", UIStyles.ToggleStyle);
            ModConfig.EnableRoomCulling.Value = GUILayout.Toggle(
                ModConfig.EnableRoomCulling.Value, "启用房间级遮挡剔除", UIStyles.ToggleStyle);
            ModConfig.EnableFrustumCulling.Value = GUILayout.Toggle(
                ModConfig.EnableFrustumCulling.Value, "启用增强视锥体剔除", UIStyles.ToggleStyle);

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("剔除距离:", UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.CullingDistance.Value = GUILayout.HorizontalSlider(
                ModConfig.CullingDistance.Value, 50f, 500f);
            GUILayout.Label($"{ModConfig.CullingDistance.Value:F0}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("视锥体余量:", UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.FrustumCullingMargin.Value = GUILayout.HorizontalSlider(
                ModConfig.FrustumCullingMargin.Value, 1.0f, 2.0f);
            GUILayout.Label($"{ModConfig.FrustumCullingMargin.Value:F2}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            if (GUILayout.Button("应用遮挡剔除设置", UIStyles.ButtonStyle))
                Plugin.CullingOpt?.Apply();
        }

        private void DrawParticlesTab()
        {
            GUILayout.Label("粒子系统优化 (NoRainParticles 风格)", UIStyles.SectionHeaderStyle);
            GUILayout.Space(5);

            ModConfig.EnableParticleOpt.Value = GUILayout.Toggle(
                ModConfig.EnableParticleOpt.Value, "启用粒子优化", UIStyles.ToggleStyle);
            ModConfig.DisableRainParticles.Value = GUILayout.Toggle(
                ModConfig.DisableRainParticles.Value, "禁用雨滴粒子", UIStyles.ToggleStyle);

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("粒子剔除距离:", UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.ParticleCullingDistance.Value = GUILayout.HorizontalSlider(
                ModConfig.ParticleCullingDistance.Value, 20f, 300f);
            GUILayout.Label($"{ModConfig.ParticleCullingDistance.Value:F0}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("每系统最大粒子数:", UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.MaxParticles.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.MaxParticles.Value, 50, 2000);
            GUILayout.Label($"{ModConfig.MaxParticles.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("更新频率 (秒):", UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.ParticleUpdateRate.Value = GUILayout.HorizontalSlider(
                ModConfig.ParticleUpdateRate.Value, 0.01f, 0.5f);
            GUILayout.Label($"{ModConfig.ParticleUpdateRate.Value:F2}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            if (GUILayout.Button("应用粒子设置", UIStyles.ButtonStyle))
                Plugin.ParticleOpt?.Apply();
        }

        private void DrawLightingTab()
        {
            GUILayout.Label("灯光优化 (LightsOut 风格)", UIStyles.SectionHeaderStyle);
            GUILayout.Space(5);

            ModConfig.EnableLightingOpt.Value = GUILayout.Toggle(
                ModConfig.EnableLightingOpt.Value, "启用灯光优化", UIStyles.ToggleStyle);
            ModConfig.DisableDynamicShadows.Value = GUILayout.Toggle(
                ModConfig.DisableDynamicShadows.Value, "禁用动态阴影", UIStyles.ToggleStyle);

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("灯光剔除距离:", UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.LightCullingDistance.Value = GUILayout.HorizontalSlider(
                ModConfig.LightCullingDistance.Value, 20f, 300f);
            GUILayout.Label($"{ModConfig.LightCullingDistance.Value:F0}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("最大动态灯光数:", UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.MaxDynamicLights.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.MaxDynamicLights.Value, 1, 32);
            GUILayout.Label($"{ModConfig.MaxDynamicLights.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("阴影级联数:", UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.ShadowCascades.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.ShadowCascades.Value, 1, 4);
            GUILayout.Label($"{ModConfig.ShadowCascades.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            if (GUILayout.Button("应用灯光设置", UIStyles.ButtonStyle))
                Plugin.LightingOpt?.Apply();
        }

        private void DrawAudioTab()
        {
            GUILayout.Label("音频优化", UIStyles.SectionHeaderStyle);
            GUILayout.Space(5);

            ModConfig.EnableAudioOpt.Value = GUILayout.Toggle(
                ModConfig.EnableAudioOpt.Value, "启用音频优化", UIStyles.ToggleStyle);
            ModConfig.DisableReverb.Value = GUILayout.Toggle(
                ModConfig.DisableReverb.Value, "禁用混响效果", UIStyles.ToggleStyle);

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("音频剔除距离:", UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.AudioCullingDistance.Value = GUILayout.HorizontalSlider(
                ModConfig.AudioCullingDistance.Value, 30f, 500f);
            GUILayout.Label($"{ModConfig.AudioCullingDistance.Value:F0}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("最大音频源数:", UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.MaxAudioSources.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.MaxAudioSources.Value, 4, 64);
            GUILayout.Label($"{ModConfig.MaxAudioSources.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            if (GUILayout.Button("应用音频设置", UIStyles.ButtonStyle))
                Plugin.AudioOpt?.Apply();
        }

        private void DrawPresetsTab()
        {
            GUILayout.Label("质量预设方案", UIStyles.SectionHeaderStyle);
            GUILayout.Space(5);
            GUILayout.Label("选择一个预设方案，一键应用适合你硬件的优化设置。", UIStyles.LabelStyle);
            GUILayout.Space(10);

            if (GUILayout.Button("极致画质 - 最高视觉质量", UIStyles.ButtonStyle))
            {
                Plugin.QualityOpt?.ApplyPreset("Ultra");
            }
            GUILayout.Space(3);
            GUILayout.Label("最佳画质，少量性能优化", UIStyles.LabelStyle);
            GUILayout.Space(8);

            if (GUILayout.Button("高画质 - 高质量", UIStyles.ButtonStyle))
            {
                Plugin.QualityOpt?.ApplyPreset("High");
            }
            GUILayout.Space(3);
            GUILayout.Label("优秀画质，适中的性能提升", UIStyles.LabelStyle);
            GUILayout.Space(8);

            if (GUILayout.Button("均衡 - 推荐方案", UIStyles.ButtonStyle))
            {
                Plugin.QualityOpt?.ApplyPreset("Balanced");
            }
            GUILayout.Space(3);
            GUILayout.Label("画质与性能的平衡之选", UIStyles.LabelStyle);
            GUILayout.Space(8);

            if (GUILayout.Button("性能优先 - FPS 优先", UIStyles.ButtonStyle))
            {
                Plugin.QualityOpt?.ApplyPreset("Performance");
            }
            GUILayout.Space(3);
            GUILayout.Label("显著提升帧率，适度降低画质", UIStyles.LabelStyle);
            GUILayout.Space(8);

            if (GUILayout.Button("极限性能 - 最大帧率", UIStyles.ButtonStyle))
            {
                Plugin.QualityOpt?.ApplyPreset("Extreme");
            }
            GUILayout.Space(3);
            GUILayout.Label("低配硬件最大帧率，最低画质开销", UIStyles.LabelStyle);
            GUILayout.Space(15);

            GUILayout.Label("--- 一键优化 ---", UIStyles.SectionHeaderStyle);
            GUILayout.Space(5);

            if (GUILayout.Button("应用完整优化方案", UIStyles.ButtonStyle))
            {
                Plugin.QualityOpt?.ApplyPreset("Performance");
                Plugin.ApplyAllOptimizations();
            }
            GUILayout.Space(3);
            GUILayout.Label("应用性能预设 + 全部优化模块", UIStyles.LabelStyle);
        }
    }
}