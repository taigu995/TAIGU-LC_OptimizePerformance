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

        // 当前鼠标悬停的提示文本
        private string _tooltip = "";

        private readonly string[] _tabNames = new string[]
        {
            "概览", "渲染", "内存", "物理", "遮挡剔除",
            "粒子", "灯光", "音频", "摄像头", "预设"
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
                // 检查是否有新的悬停提示
                if (Event.current.type == EventType.Repaint)
                {
                    _tooltip = GUI.tooltip;
                }

                GUI.Window(9527, _windowRect, DrawMainWindow,
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
                case 8: DrawCameraTab(); break;
                case 9: DrawPresetsTab(); break;
            }

            GUILayout.EndScrollView();

            // 提示栏 - 显示当前鼠标悬停的提示文字
            if (!string.IsNullOrEmpty(_tooltip))
            {
                GUILayout.Space(3);
                GUILayout.Box(_tooltip, UIStyles.TooltipStyle);
            }

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(new GUIContent("应用全部优化", "一键应用所有模块的当前设置"), UIStyles.ButtonStyle))
                Plugin.ReapplyAllOptimizations();

            if (GUILayout.Button(new GUIContent("恢复全部默认", "将所有设置恢复为默认值并重新应用"), UIStyles.ButtonStyle))
            {
                ModConfig.ResetToDefaults();
                Plugin.ReapplyAllOptimizations();
            }

            if (GUILayout.Button(new GUIContent("重置统计", "重置 FPS 和性能监控统计数据"), UIStyles.ButtonStyle))
                Plugin.PerfMonitor.ResetStats();

            GUILayout.EndHorizontal();
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        // ==================== 概览标签 ====================
        private void DrawOverviewTab()
        {
            var monitor = Plugin.PerfMonitor;

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("当前帧率:", "当前游戏画面的 FPS 帧率"), UIStyles.BoldLabelStyle, GUILayout.Width(120));
            float fps = monitor.CurrentFPS;
            GUIStyle fpsStyle = fps >= 60 ? UIStyles.GoodStyle :
                               fps >= 30 ? UIStyles.WarnStyle : UIStyles.BadStyle;
            GUILayout.Label($"{fps:F1} FPS", fpsStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("帧生成时间:", "每帧渲染所需时间，越低越流畅"), UIStyles.BoldLabelStyle, GUILayout.Width(120));
            GUILayout.Label($"{monitor.CurrentFrameTime:F2} 毫秒", UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("平均 / 最低 / 最高:", "会话期间的平均/最低/最高 FPS"), UIStyles.BoldLabelStyle, GUILayout.Width(120));
            GUILayout.Label($"{monitor.AvgFPS:F1} / {monitor.MinFPS:F1} / {monitor.MaxFPS:F1}",
                UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label("--- 系统状态 ---", UIStyles.SectionHeaderStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("内存占用:", "游戏当前内存使用量"), UIStyles.BoldLabelStyle, GUILayout.Width(120));
            GUILayout.Label($"{monitor.CurrentMemoryMB} MB (峰值: {monitor.PeakMemoryMB} MB)",
                UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("活跃渲染器:", "当前场景中活跃的渲染器数量"), UIStyles.BoldLabelStyle, GUILayout.Width(120));
            GUILayout.Label($"{monitor.ActiveRenderers}", UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("活跃灯光:", "当前场景中活跃的灯光数量"), UIStyles.BoldLabelStyle, GUILayout.Width(120));
            GUILayout.Label($"{monitor.ActiveLights}", UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("活跃音频:", "当前场景中活跃的音频源数量"), UIStyles.BoldLabelStyle, GUILayout.Width(120));
            GUILayout.Label($"{monitor.ActiveAudioSources}", UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("活跃粒子:", "当前场景中活跃的粒子系统数量"), UIStyles.BoldLabelStyle, GUILayout.Width(120));
            GUILayout.Label($"{monitor.ActiveParticles}", UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label("--- 优化模块状态 ---", UIStyles.SectionHeaderStyle);

            DrawModuleStatus("渲染", "渲染相关的优化模块", Plugin.RenderOpt?.IsApplied ?? false);
            DrawModuleStatus("内存", "内存和 GC 相关优化模块", Plugin.MemoryOpt?.IsApplied ?? false);
            DrawModuleStatus("物理", "物理引擎相关优化模块", Plugin.PhysicsOpt?.IsApplied ?? false);
            DrawModuleStatus("遮挡剔除", "遮挡剔除和视锥体优化模块", Plugin.CullingOpt?.IsApplied ?? false);
            DrawModuleStatus("粒子", "粒子系统优化模块", Plugin.ParticleOpt?.IsApplied ?? false);
            DrawModuleStatus("灯光", "灯光和雾效优化模块", Plugin.LightingOpt?.IsApplied ?? false);
            DrawModuleStatus("音频", "音频系统优化模块", Plugin.AudioOpt?.IsApplied ?? false);
            DrawModuleStatus("摄像头", "摄像头和帧率优化模块", Plugin.CameraOpt?.IsApplied ?? false);

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("组件缓存:", "组件缓存系统的命中率统计"), UIStyles.BoldLabelStyle, GUILayout.Width(120));
            GUILayout.Label($"命中: {MemoryOptimizer.CacheHits} | 未命中: {MemoryOptimizer.CacheMisses} | 命中率: {MemoryOptimizer.GetCacheHitRate():F1}%",
                UIStyles.LabelStyle);
            GUILayout.EndHorizontal();
        }

        private void DrawModuleStatus(string name, string tooltip, bool isActive)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent(name + ":", tooltip), UIStyles.BoldLabelStyle, GUILayout.Width(120));
            GUILayout.Label(isActive ? "已启用" : "未启用",
                isActive ? UIStyles.GoodStyle : UIStyles.WarnStyle);
            GUILayout.EndHorizontal();
        }

        // ==================== 渲染标签 ====================
        private void DrawRenderTab()
        {
            GUILayout.Label("渲染优化设置", UIStyles.SectionHeaderStyle);
            GUILayout.Space(5);

            ModConfig.EnableRenderOpt.Value = GUILayout.Toggle(
                ModConfig.EnableRenderOpt.Value,
                new GUIContent("启用渲染优化", "开启或关闭渲染优化模块，关闭后所有渲染设置将恢复原版"), UIStyles.ToggleStyle);

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("LOD 偏差:", "控制细节层次切换的敏感度，值越低远处物体越简化，性能越好 (0.1=极低画质, 3.0=极高画质)"), UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.LODBias.Value = GUILayout.HorizontalSlider(
                ModConfig.LODBias.Value, 0.1f, 3.0f);
            GUILayout.Label($"{ModConfig.LODBias.Value:F2}", UIStyles.LabelStyle, GUILayout.Width(40));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("绘制距离:", "场景物体的最大可见距离，值越小性能越好，建议室内场景 500-800"), UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.MaxDrawDistance.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.MaxDrawDistance.Value, 100, 2000);
            GUILayout.Label($"{ModConfig.MaxDrawDistance.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("阴影分辨率:", "阴影贴图分辨率，值越低性能越好 (256=极低, 4096=极高)"), UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.ShadowResolution.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.ShadowResolution.Value, 256, 4096);
            GUILayout.Label($"{ModConfig.ShadowResolution.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("阴影距离:", "阴影的最大渲染距离，值越小性能越好，建议 50-150"), UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.MaxShadowDistance.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.MaxShadowDistance.Value, 50, 500);
            GUILayout.Label($"{ModConfig.MaxShadowDistance.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("帧率上限 (0=不限):", "限制最大帧率，0=不限，建议设为 60 或 144 以节省性能"), UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.MaxFPS.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.MaxFPS.Value, 0, 300);
            GUILayout.Label($"{ModConfig.MaxFPS.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            ModConfig.DisableShadows.Value = GUILayout.Toggle(
                ModConfig.DisableShadows.Value,
                new GUIContent("禁用全部阴影", "完全禁用所有阴影渲染，可大幅提升 FPS，但画面会失去立体感"), UIStyles.ToggleStyle);
            ModConfig.EnableBatching.Value = GUILayout.Toggle(
                ModConfig.EnableBatching.Value,
                new GUIContent("启用静态批处理", "合并静态物体以降低绘制调用次数，建议开启"), UIStyles.ToggleStyle);

            GUILayout.Space(10);
            if (GUILayout.Button(new GUIContent("应用渲染设置", "应用当前渲染标签页的所有设置"), UIStyles.ButtonStyle))
                Plugin.RenderOpt?.Reapply();
        }

        // ==================== 内存标签 ====================
        private void DrawMemoryTab()
        {
            GUILayout.Label("内存 & GC 优化", UIStyles.SectionHeaderStyle);
            GUILayout.Space(5);

            ModConfig.EnableMemoryOpt.Value = GUILayout.Toggle(
                ModConfig.EnableMemoryOpt.Value,
                new GUIContent("启用内存优化", "开启内存优化模块，包括 GC 管理和资源清理，建议开启"), UIStyles.ToggleStyle);
            ModConfig.EnableComponentCaching.Value = GUILayout.Toggle(
                ModConfig.EnableComponentCaching.Value,
                new GUIContent("启用组件缓存", "缓存常用组件引用，减少运行时查找开销，提升 CPU 性能"), UIStyles.ToggleStyle);
            ModConfig.AggressiveGC.Value = GUILayout.Toggle(
                ModConfig.AggressiveGC.Value,
                new GUIContent("激进 GC 模式", "更频繁地触发垃圾回收，减少内存峰值，但可能造成微小卡顿"), UIStyles.ToggleStyle);

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("GC 间隔 (秒):", "两次垃圾回收之间的间隔时间，越短内存占用越低，但可能引起卡顿"), UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.GCInterval.Value = GUILayout.HorizontalSlider(
                ModConfig.GCInterval.Value, 1.0f, 30.0f);
            GUILayout.Label($"{ModConfig.GCInterval.Value:F1}", UIStyles.LabelStyle, GUILayout.Width(40));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("最大缓存数:", "组件缓存的最大条目数，越大缓存命中率越高，但占用更多内存"), UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.MaxCachedComponents.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.MaxCachedComponents.Value, 100, 2000);
            GUILayout.Label($"{ModConfig.MaxCachedComponents.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label("--- 缓存统计 ---", UIStyles.SectionHeaderStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("缓存大小:", "当前组件缓存中缓存的条目数"), UIStyles.LabelStyle, GUILayout.Width(160));
            GUILayout.Label($"{MemoryOptimizer.CacheSize}", UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("命中率:", "缓存命中率，越高说明缓存效果越好"), UIStyles.LabelStyle, GUILayout.Width(160));
            GUILayout.Label($"{MemoryOptimizer.GetCacheHitRate():F1}%", UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("内存用量:", "当前内存使用量"), UIStyles.LabelStyle, GUILayout.Width(160));
            GUILayout.Label($"{Plugin.MemoryOpt?.GetMemoryUsageMB()} MB", UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            if (GUILayout.Button(new GUIContent("立即强制 GC", "立即执行一次垃圾回收，清理未使用的内存"), UIStyles.ButtonStyle))
            {
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.GC.Collect();
            }
        }

        // ==================== 物理标签 ====================
        private void DrawPhysicsTab()
        {
            GUILayout.Label("物理引擎优化", UIStyles.SectionHeaderStyle);
            GUILayout.Space(5);

            ModConfig.EnablePhysicsOpt.Value = GUILayout.Toggle(
                ModConfig.EnablePhysicsOpt.Value,
                new GUIContent("启用物理优化", "开启物理引擎优化模块，包括时间步长和迭代优化"), UIStyles.ToggleStyle);
            ModConfig.DisableSleepingBodies.Value = GUILayout.Toggle(
                ModConfig.DisableSleepingBodies.Value,
                new GUIContent("优化休眠刚体", "自动让不运动的刚体进入休眠状态，节省 CPU 资源"), UIStyles.ToggleStyle);

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("物理时间步长:", "物理模拟的更新间隔，越大性能越好但精度降低 (0.01=高精度, 0.05=高性能)"), UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.PhysicsTimeStep.Value = GUILayout.HorizontalSlider(
                ModConfig.PhysicsTimeStep.Value, 0.01f, 0.05f);
            GUILayout.Label($"{ModConfig.PhysicsTimeStep.Value:F3}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("求解器迭代次数:", "物理约束求解的迭代次数，越少性能越好，但物理效果可能变差 (1=低质量, 20=高质量)"), UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.MaxPhysicsIterations.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.MaxPhysicsIterations.Value, 1, 20);
            GUILayout.Label($"{ModConfig.MaxPhysicsIterations.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("休眠阈值:", "刚体速度低于此值时进入休眠，越大越容易休眠，节省 CPU"), UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.SleepThreshold.Value = GUILayout.HorizontalSlider(
                ModConfig.SleepThreshold.Value, 0.001f, 0.1f);
            GUILayout.Label($"{ModConfig.SleepThreshold.Value:F4}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            if (GUILayout.Button(new GUIContent("应用物理设置", "应用当前物理标签页的所有设置"), UIStyles.ButtonStyle))
                Plugin.PhysicsOpt?.Reapply();
        }

        // ==================== 遮挡剔除标签 ====================
        private void DrawCullingTab()
        {
            GUILayout.Label("遮挡剔除优化", UIStyles.SectionHeaderStyle);
            GUILayout.Space(5);

            ModConfig.EnableCullingOpt.Value = GUILayout.Toggle(
                ModConfig.EnableCullingOpt.Value,
                new GUIContent("启用遮挡剔除优化", "开启遮挡剔除优化模块，减少渲染不可见物体，建议开启"), UIStyles.ToggleStyle);
            ModConfig.EnableRoomCulling.Value = GUILayout.Toggle(
                ModConfig.EnableRoomCulling.Value,
                new GUIContent("启用房间级遮挡剔除", "利用房间结构进行更高效的遮挡剔除，室内场景效果显著"), UIStyles.ToggleStyle);
            ModConfig.EnableFrustumCulling.Value = GUILayout.Toggle(
                ModConfig.EnableFrustumCulling.Value,
                new GUIContent("启用增强视锥体剔除", "更激进的视锥剔除，剔除视野外的物体，提升 FPS"), UIStyles.ToggleStyle);

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("剔除距离:", "物体超出此距离即被剔除，越小性能越好，建议 50-200"), UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.CullingDistance.Value = GUILayout.HorizontalSlider(
                ModConfig.CullingDistance.Value, 50f, 500f);
            GUILayout.Label($"{ModConfig.CullingDistance.Value:F0}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("视锥体余量:", "视锥体剔除的额外余量，越小剔除越激进，但可能导致物体突然消失"), UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.FrustumCullingMargin.Value = GUILayout.HorizontalSlider(
                ModConfig.FrustumCullingMargin.Value, 1.0f, 2.0f);
            GUILayout.Label($"{ModConfig.FrustumCullingMargin.Value:F2}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            if (GUILayout.Button(new GUIContent("应用遮挡剔除设置", "应用当前遮挡剔除标签页的所有设置"), UIStyles.ButtonStyle))
                Plugin.CullingOpt?.Reapply();
        }

        // ==================== 粒子标签 ====================
        private void DrawParticlesTab()
        {
            GUILayout.Label("粒子系统优化", UIStyles.SectionHeaderStyle);
            GUILayout.Space(5);

            ModConfig.EnableParticleOpt.Value = GUILayout.Toggle(
                ModConfig.EnableParticleOpt.Value,
                new GUIContent("启用粒子优化", "开启粒子系统优化模块，减少粒子渲染开销"), UIStyles.ToggleStyle);
            ModConfig.DisableRainParticles.Value = GUILayout.Toggle(
                ModConfig.DisableRainParticles.Value,
                new GUIContent("禁用雨滴粒子", "完全禁用雨水粒子效果，大幅提升户外场景 FPS"), UIStyles.ToggleStyle);

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("粒子剔除距离:", "粒子超出此距离即被剔除，越小性能越好，建议 20-50"), UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.ParticleCullingDistance.Value = GUILayout.HorizontalSlider(
                ModConfig.ParticleCullingDistance.Value, 20f, 300f);
            GUILayout.Label($"{ModConfig.ParticleCullingDistance.Value:F0}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("每系统最大粒子数:", "每个粒子系统最多生成的粒子数，越少性能越好"), UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.MaxParticles.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.MaxParticles.Value, 50, 2000);
            GUILayout.Label($"{ModConfig.MaxParticles.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("更新频率 (秒):", "粒子系统的更新间隔，越大性能越好，但粒子动画会变粗糙"), UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.ParticleUpdateRate.Value = GUILayout.HorizontalSlider(
                ModConfig.ParticleUpdateRate.Value, 0.01f, 0.5f);
            GUILayout.Label($"{ModConfig.ParticleUpdateRate.Value:F2}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            if (GUILayout.Button(new GUIContent("应用粒子设置", "应用当前粒子标签页的所有设置"), UIStyles.ButtonStyle))
                Plugin.ParticleOpt?.Reapply();
        }

        // ==================== 灯光标签 ====================
        private void DrawLightingTab()
        {
            GUILayout.Label("灯光优化", UIStyles.SectionHeaderStyle);
            GUILayout.Space(5);

            ModConfig.EnableLightingOpt.Value = GUILayout.Toggle(
                ModConfig.EnableLightingOpt.Value,
                new GUIContent("启用灯光优化", "开启灯光优化模块，包括灯光剔除和雾效优化"), UIStyles.ToggleStyle);
            ModConfig.DisableDynamicShadows.Value = GUILayout.Toggle(
                ModConfig.DisableDynamicShadows.Value,
                new GUIContent("禁用动态阴影", "禁用所有动态光源的阴影投射，大幅提升 FPS"), UIStyles.ToggleStyle);

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("灯光剔除距离:", "灯光超出此距离即被剔除，越小性能越好，建议 20-50"), UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.LightCullingDistance.Value = GUILayout.HorizontalSlider(
                ModConfig.LightCullingDistance.Value, 20f, 300f);
            GUILayout.Label($"{ModConfig.LightCullingDistance.Value:F0}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("最大动态灯光数:", "同时渲染的动态灯光最大数量，越少性能越好 (1=极低, 32=极高)"), UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.MaxDynamicLights.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.MaxDynamicLights.Value, 1, 32);
            GUILayout.Label($"{ModConfig.MaxDynamicLights.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("阴影级联数:", "阴影级联层数，越少性能越好，但远处阴影质量会下降 (1=低质量, 4=高质量)"), UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.ShadowCascades.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.ShadowCascades.Value, 1, 4);
            GUILayout.Label($"{ModConfig.ShadowCascades.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.Label("--- 雾效优化 ---", UIStyles.SectionHeaderStyle);

            string[] fogModes = { "Vanilla", "Hide", "Disable", "ForceDisable" };
            string[] fogModeLabels = { "原版雾效", "隐藏雾效", "禁用雾效", "强制禁用" };
            string[] fogModeTooltips = { "保持原版雾效不变", "隐藏雾效渲染（视觉上消失，但仍在计算）", "禁用雾效系统（停止计算，提升性能）", "强制禁用所有雾效相关组件（最大性能提升）" };
            int currentFogIndex = System.Array.IndexOf(fogModes, ModConfig.FogMode.Value);
            if (currentFogIndex < 0) currentFogIndex = 0;

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("雾效模式:", "控制雾效的渲染方式，禁用雾效可显著提升 FPS"), UIStyles.BoldLabelStyle, GUILayout.Width(120));
            int newFogIndex = GUILayout.SelectionGrid(currentFogIndex, fogModeLabels, 2, UIStyles.ToggleStyle);
            if (newFogIndex != currentFogIndex)
            {
                ModConfig.FogMode.Value = fogModes[newFogIndex];
            }
            GUILayout.EndHorizontal();

            // 显示当前雾效模式的说明
            if (currentFogIndex >= 0 && currentFogIndex < fogModeTooltips.Length)
            {
                GUILayout.Label(fogModeTooltips[currentFogIndex], UIStyles.TooltipStyle);
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("体积雾距离乘数:", "体积雾的影响范围乘数，越小性能越好 (0=完全禁用体积雾)"), UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.VolumetricFogDistanceMultiplier.Value = GUILayout.HorizontalSlider(
                ModConfig.VolumetricFogDistanceMultiplier.Value, 0f, 2f);
            GUILayout.Label($"{ModConfig.VolumetricFogDistanceMultiplier.Value:F2}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            if (GUILayout.Button(new GUIContent("应用灯光/雾效设置", "应用当前灯光标签页的所有设置"), UIStyles.ButtonStyle))
                Plugin.LightingOpt?.Reapply();
        }

        // ==================== 音频标签 ====================
        private void DrawAudioTab()
        {
            GUILayout.Label("音频优化", UIStyles.SectionHeaderStyle);
            GUILayout.Space(5);

            ModConfig.EnableAudioOpt.Value = GUILayout.Toggle(
                ModConfig.EnableAudioOpt.Value,
                new GUIContent("启用音频优化", "开启音频优化模块，包括音频剔除和混响控制"), UIStyles.ToggleStyle);
            ModConfig.DisableReverb.Value = GUILayout.Toggle(
                ModConfig.DisableReverb.Value,
                new GUIContent("禁用混响效果", "关闭所有音频混响效果，节省 CPU 资源"), UIStyles.ToggleStyle);

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("音频剔除距离:", "音频源超出此距离即被静音，越小性能越好，建议 30-100"), UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.AudioCullingDistance.Value = GUILayout.HorizontalSlider(
                ModConfig.AudioCullingDistance.Value, 30f, 500f);
            GUILayout.Label($"{ModConfig.AudioCullingDistance.Value:F0}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("最大音频源数:", "同时播放的音频源最大数量，越少性能越好 (4=极低, 64=极高)"), UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.MaxAudioSources.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.MaxAudioSources.Value, 4, 64);
            GUILayout.Label($"{ModConfig.MaxAudioSources.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            if (GUILayout.Button(new GUIContent("应用音频设置", "应用当前音频标签页的所有设置"), UIStyles.ButtonStyle))
                Plugin.AudioOpt?.Reapply();
        }

        // ==================== 摄像头标签 ====================
        private void DrawCameraTab()
        {
            GUILayout.Label("摄像头优化", UIStyles.SectionHeaderStyle);
            GUILayout.Space(5);

            ModConfig.EnableCameraOpt.Value = GUILayout.Toggle(
                ModConfig.EnableCameraOpt.Value,
                new GUIContent("启用摄像头优化", "开启摄像头优化模块，包括帧率限制和透明渲染控制"), UIStyles.ToggleStyle);

            GUILayout.Space(5);
            GUILayout.Label("--- 飞船摄像头 ---", UIStyles.SectionHeaderStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("飞船摄像头帧率:", "飞船摄像头的渲染帧率，越低性能越好 (0=暂停, 60=流畅)"), UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.ShipCameraFramerate.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.ShipCameraFramerate.Value, 0, 60);
            GUILayout.Label($"{ModConfig.ShipCameraFramerate.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            ModConfig.DisableTransparentShipCamera.Value = GUILayout.Toggle(
                ModConfig.DisableTransparentShipCamera.Value,
                new GUIContent("禁用飞船摄像头透明渲染", "禁用飞船摄像头的透明渲染，节省性能，建议开启"), UIStyles.ToggleStyle);

            GUILayout.Space(5);
            GUILayout.Label("--- 地图/监视摄像头 ---", UIStyles.SectionHeaderStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("地图摄像头帧率:", "地图和监控摄像头的渲染帧率，越低性能越好 (0=暂停, 60=流畅)"), UIStyles.LabelStyle, GUILayout.Width(160));
            ModConfig.MapCameraFramerate.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.MapCameraFramerate.Value, 0, 60);
            GUILayout.Label($"{ModConfig.MapCameraFramerate.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            ModConfig.DisableTransparentMapCamera.Value = GUILayout.Toggle(
                ModConfig.DisableTransparentMapCamera.Value,
                new GUIContent("禁用地图摄像头透明渲染", "禁用地图摄像头的透明渲染，节省性能，建议开启"), UIStyles.ToggleStyle);

            GUILayout.Space(10);
            if (GUILayout.Button(new GUIContent("应用摄像头设置", "应用当前摄像头标签页的所有设置"), UIStyles.ButtonStyle))
                Plugin.CameraOpt?.Reapply();
        }

        // ==================== 预设标签 ====================
        private void DrawPresetsTab()
        {
            GUILayout.Label("质量预设方案", UIStyles.SectionHeaderStyle);
            GUILayout.Space(5);
            GUILayout.Label("选择一个预设方案，一键应用适合你硬件的优化设置。", UIStyles.LabelStyle);
            GUILayout.Space(10);

            if (GUILayout.Button(new GUIContent("极致画质 - 最高视觉质量", "最高画质，轻微性能优化，适合高端显卡"), UIStyles.ButtonStyle))
            {
                Plugin.QualityOpt?.ApplyPreset("Ultra");
            }
            GUILayout.Space(3);
            GUILayout.Label("最佳画质，少量性能优化", UIStyles.LabelStyle);
            GUILayout.Space(8);

            if (GUILayout.Button(new GUIContent("高画质 - 高质量", "优秀画质，适中的性能提升，适合中高端显卡"), UIStyles.ButtonStyle))
            {
                Plugin.QualityOpt?.ApplyPreset("High");
            }
            GUILayout.Space(3);
            GUILayout.Label("优秀画质，适中的性能提升", UIStyles.LabelStyle);
            GUILayout.Space(8);

            if (GUILayout.Button(new GUIContent("均衡 - 推荐方案", "画质与性能的平衡之选，适合大多数配置"), UIStyles.ButtonStyle))
            {
                Plugin.QualityOpt?.ApplyPreset("Balanced");
            }
            GUILayout.Space(3);
            GUILayout.Label("画质与性能的平衡之选", UIStyles.LabelStyle);
            GUILayout.Space(8);

            if (GUILayout.Button(new GUIContent("性能优先 - FPS 优先", "显著提升帧率，适度降低画质，适合中低端显卡"), UIStyles.ButtonStyle))
            {
                Plugin.QualityOpt?.ApplyPreset("Performance");
            }
            GUILayout.Space(3);
            GUILayout.Label("显著提升帧率，适度降低画质", UIStyles.LabelStyle);
            GUILayout.Space(8);

            if (GUILayout.Button(new GUIContent("极限性能 - 最大帧率", "低配硬件最大帧率，最低画质开销，适合低端配置"), UIStyles.ButtonStyle))
            {
                Plugin.QualityOpt?.ApplyPreset("Extreme");
            }
            GUILayout.Space(3);
            GUILayout.Label("低配硬件最大帧率，最低画质开销", UIStyles.LabelStyle);
            GUILayout.Space(15);

            GUILayout.Label("--- 一键优化 ---", UIStyles.SectionHeaderStyle);
            GUILayout.Space(5);

            if (GUILayout.Button(new GUIContent("应用完整优化方案", "一键应用性能预设 + 全部优化模块，推荐首选"), UIStyles.ButtonStyle))
            {
                Plugin.QualityOpt?.ApplyPreset("Performance");
                Plugin.ReapplyAllOptimizations();
            }
            GUILayout.Space(3);
            GUILayout.Label("应用性能预设 + 全部优化模块", UIStyles.LabelStyle);
        }
    }
}