using UnityEngine;
using TAIGU_LC_OptimizePerformance.Config;
using TAIGU_LC_OptimizePerformance.Optimizers;

namespace TAIGU_LC_OptimizePerformance.UI
{
    /// <summary>
    /// Main performance optimization UI panel.
    /// Toggle with F5 (configurable). Shows FPS counter, performance stats,
    /// and per-module enable/disable controls.
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
            "Overview", "Render", "Memory", "Physics", "Culling",
            "Particles", "Lighting", "Audio", "Presets"
        };

        public PerformanceUI()
        {
            _windowRect = new Rect(20, 20, 520, 600);
        }

        public void Update()
        {
            // UI update logic if needed
        }

        public void OnGUI()
        {
            if (!_stylesInitialized)
            {
                UIStyles.Init();
                _stylesInitialized = true;
            }

            // Draw FPS counter
            if (ShowFPS && !IsVisible)
            {
                DrawFPSCounter();
            }

            // Draw main panel
            if (IsVisible)
            {
                GUI.ModalWindow(9527, _windowRect, DrawMainWindow,
                    "TAIGU-LC_OptimizePerformance v1.0.0",
                    UIStyles.WindowStyle);
            }
        }

        private void DrawFPSCounter()
        {
            var monitor = Plugin.PerfMonitor;
            float fps = monitor.CurrentFPS;

            var fpsStyle = UIStyles.GetFPSStyle(fps);
            GUI.Label(new Rect(10, 10, 120, 30),
                $"FPS: {fps:F1}", fpsStyle);
        }

        private void DrawMainWindow(int windowId)
        {
            // Header
            GUILayout.Label("TAIGU-LC_OptimizePerformance - Ultimate Performance Suite", UIStyles.HeaderStyle);
            GUILayout.Space(5);

            // Tab bar
            _selectedTab = GUILayout.Toolbar(_selectedTab, _tabNames, UIStyles.ButtonStyle);
            GUILayout.Space(8);

            // Scroll view for content
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

            // Bottom buttons
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Apply All", UIStyles.ButtonStyle))
            {
                Plugin.ApplyAllOptimizations();
            }

            if (GUILayout.Button("Revert All", UIStyles.ButtonStyle))
            {
                Plugin.RevertAllOptimizations();
            }

            if (GUILayout.Button("Reset Stats", UIStyles.ButtonStyle))
            {
                Plugin.PerfMonitor.ResetStats();
            }

            GUILayout.EndHorizontal();

            // Make window draggable
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        private void DrawOverviewTab()
        {
            var monitor = Plugin.PerfMonitor;

            // FPS Display
            GUILayout.BeginHorizontal();
            GUILayout.Label("Current FPS:", UIStyles.BoldLabelStyle, GUILayout.Width(120));
            float fps = monitor.CurrentFPS;
            GUIStyle fpsStyle = fps >= 60 ? UIStyles.GoodStyle :
                               fps >= 30 ? UIStyles.WarnStyle : UIStyles.BadStyle;
            GUILayout.Label($"{fps:F1} FPS", fpsStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Frame Time:", UIStyles.BoldLabelStyle, GUILayout.Width(120));
            GUILayout.Label($"{monitor.CurrentFrameTime:F2} ms", UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Avg / Min / Max:", UIStyles.BoldLabelStyle, GUILayout.Width(120));
            GUILayout.Label($"{monitor.AvgFPS:F1} / {monitor.MinFPS:F1} / {monitor.MaxFPS:F1}",
                UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label("--- System Stats ---", UIStyles.SectionHeaderStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Memory Usage:", UIStyles.BoldLabelStyle, GUILayout.Width(120));
            GUILayout.Label($"{monitor.CurrentMemoryMB} MB (Peak: {monitor.PeakMemoryMB} MB)",
                UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Active Renderers:", UIStyles.BoldLabelStyle, GUILayout.Width(120));
            GUILayout.Label($"{monitor.ActiveRenderers}", UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Active Lights:", UIStyles.BoldLabelStyle, GUILayout.Width(120));
            GUILayout.Label($"{monitor.ActiveLights}", UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Active Audio:", UIStyles.BoldLabelStyle, GUILayout.Width(120));
            GUILayout.Label($"{monitor.ActiveAudioSources}", UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Active Particles:", UIStyles.BoldLabelStyle, GUILayout.Width(120));
            GUILayout.Label($"{monitor.ActiveParticles}", UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label("--- Optimization Status ---", UIStyles.SectionHeaderStyle);

            DrawModuleStatus("Render", Plugin.RenderOpt?.IsApplied ?? false);
            DrawModuleStatus("Memory", Plugin.MemoryOpt?.IsApplied ?? false);
            DrawModuleStatus("Physics", Plugin.PhysicsOpt?.IsApplied ?? false);
            DrawModuleStatus("Culling", Plugin.CullingOpt?.IsApplied ?? false);
            DrawModuleStatus("Particles", Plugin.ParticleOpt?.IsApplied ?? false);
            DrawModuleStatus("Lighting", Plugin.LightingOpt?.IsApplied ?? false);
            DrawModuleStatus("Audio", Plugin.AudioOpt?.IsApplied ?? false);

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Component Cache:", UIStyles.BoldLabelStyle, GUILayout.Width(120));
            GUILayout.Label($"Hits: {MemoryOptimizer.CacheHits} | Miss: {MemoryOptimizer.CacheMisses} | Rate: {MemoryOptimizer.GetCacheHitRate():F1}%",
                UIStyles.LabelStyle);
            GUILayout.EndHorizontal();
        }

        private void DrawModuleStatus(string name, bool isActive)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name + ":", UIStyles.BoldLabelStyle, GUILayout.Width(120));
            GUILayout.Label(isActive ? "ACTIVE" : "INACTIVE",
                isActive ? UIStyles.GoodStyle : UIStyles.WarnStyle);
            GUILayout.EndHorizontal();
        }

        private void DrawRenderTab()
        {
            GUILayout.Label("Render Optimization Settings", UIStyles.SectionHeaderStyle);
            GUILayout.Space(5);

            ModConfig.EnableRenderOpt.Value = GUILayout.Toggle(
                ModConfig.EnableRenderOpt.Value, "Enable Render Optimization", UIStyles.ToggleStyle);

            GUILayout.Space(5);

            // LOD Bias
            GUILayout.BeginHorizontal();
            GUILayout.Label("LOD Bias:", UIStyles.LabelStyle, GUILayout.Width(150));
            ModConfig.LODBias.Value = GUILayout.HorizontalSlider(
                ModConfig.LODBias.Value, 0.1f, 3.0f);
            GUILayout.Label($"{ModConfig.LODBias.Value:F2}", UIStyles.LabelStyle, GUILayout.Width(40));
            GUILayout.EndHorizontal();

            // Max Draw Distance
            GUILayout.BeginHorizontal();
            GUILayout.Label("Draw Distance:", UIStyles.LabelStyle, GUILayout.Width(150));
            ModConfig.MaxDrawDistance.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.MaxDrawDistance.Value, 100, 2000);
            GUILayout.Label($"{ModConfig.MaxDrawDistance.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            // Shadow Resolution
            GUILayout.BeginHorizontal();
            GUILayout.Label("Shadow Resolution:", UIStyles.LabelStyle, GUILayout.Width(150));
            ModConfig.ShadowResolution.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.ShadowResolution.Value, 256, 4096);
            GUILayout.Label($"{ModConfig.ShadowResolution.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            // Max Shadow Distance
            GUILayout.BeginHorizontal();
            GUILayout.Label("Shadow Distance:", UIStyles.LabelStyle, GUILayout.Width(150));
            ModConfig.MaxShadowDistance.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.MaxShadowDistance.Value, 50, 500);
            GUILayout.Label($"{ModConfig.MaxShadowDistance.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            // FPS Limit
            GUILayout.BeginHorizontal();
            GUILayout.Label("FPS Limit (0=unlimited):", UIStyles.LabelStyle, GUILayout.Width(150));
            ModConfig.MaxFPS.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.MaxFPS.Value, 0, 300);
            GUILayout.Label($"{ModConfig.MaxFPS.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            ModConfig.DisableShadows.Value = GUILayout.Toggle(
                ModConfig.DisableShadows.Value, "Disable All Shadows", UIStyles.ToggleStyle);
            ModConfig.EnableBatching.Value = GUILayout.Toggle(
                ModConfig.EnableBatching.Value, "Enable Static Batching", UIStyles.ToggleStyle);

            GUILayout.Space(10);
            if (GUILayout.Button("Apply Render Settings", UIStyles.ButtonStyle))
            {
                Plugin.RenderOpt?.Apply();
            }
        }

        private void DrawMemoryTab()
        {
            GUILayout.Label("Memory & GC Optimization", UIStyles.SectionHeaderStyle);
            GUILayout.Space(5);

            ModConfig.EnableMemoryOpt.Value = GUILayout.Toggle(
                ModConfig.EnableMemoryOpt.Value, "Enable Memory Optimization", UIStyles.ToggleStyle);
            ModConfig.EnableComponentCaching.Value = GUILayout.Toggle(
                ModConfig.EnableComponentCaching.Value, "Enable Component Caching (LethalPerformance)",
                UIStyles.ToggleStyle);
            ModConfig.AggressiveGC.Value = GUILayout.Toggle(
                ModConfig.AggressiveGC.Value, "Aggressive GC Mode", UIStyles.ToggleStyle);

            GUILayout.Space(5);

            // GC Interval
            GUILayout.BeginHorizontal();
            GUILayout.Label("GC Interval (sec):", UIStyles.LabelStyle, GUILayout.Width(150));
            ModConfig.GCInterval.Value = GUILayout.HorizontalSlider(
                ModConfig.GCInterval.Value, 1.0f, 30.0f);
            GUILayout.Label($"{ModConfig.GCInterval.Value:F1}", UIStyles.LabelStyle, GUILayout.Width(40));
            GUILayout.EndHorizontal();

            // Max Cached Components
            GUILayout.BeginHorizontal();
            GUILayout.Label("Max Cache Size:", UIStyles.LabelStyle, GUILayout.Width(150));
            ModConfig.MaxCachedComponents.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.MaxCachedComponents.Value, 100, 2000);
            GUILayout.Label($"{ModConfig.MaxCachedComponents.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label("--- Cache Statistics ---", UIStyles.SectionHeaderStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Cache Size:", UIStyles.LabelStyle, GUILayout.Width(150));
            GUILayout.Label($"{MemoryOptimizer.CacheSize}", UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Hit Rate:", UIStyles.LabelStyle, GUILayout.Width(150));
            GUILayout.Label($"{MemoryOptimizer.GetCacheHitRate():F1}%", UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Memory Usage:", UIStyles.LabelStyle, GUILayout.Width(150));
            GUILayout.Label($"{Plugin.MemoryOpt?.GetMemoryUsageMB()} MB", UIStyles.LabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            if (GUILayout.Button("Force GC Now", UIStyles.ButtonStyle))
            {
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.GC.Collect();
            }
        }

        private void DrawPhysicsTab()
        {
            GUILayout.Label("Physics Optimization", UIStyles.SectionHeaderStyle);
            GUILayout.Space(5);

            ModConfig.EnablePhysicsOpt.Value = GUILayout.Toggle(
                ModConfig.EnablePhysicsOpt.Value, "Enable Physics Optimization", UIStyles.ToggleStyle);
            ModConfig.DisableSleepingBodies.Value = GUILayout.Toggle(
                ModConfig.DisableSleepingBodies.Value, "Optimize Sleeping Bodies", UIStyles.ToggleStyle);

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Physics Timestep:", UIStyles.LabelStyle, GUILayout.Width(150));
            ModConfig.PhysicsTimeStep.Value = GUILayout.HorizontalSlider(
                ModConfig.PhysicsTimeStep.Value, 0.01f, 0.05f);
            GUILayout.Label($"{ModConfig.PhysicsTimeStep.Value:F3}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Solver Iterations:", UIStyles.LabelStyle, GUILayout.Width(150));
            ModConfig.MaxPhysicsIterations.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.MaxPhysicsIterations.Value, 1, 20);
            GUILayout.Label($"{ModConfig.MaxPhysicsIterations.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Sleep Threshold:", UIStyles.LabelStyle, GUILayout.Width(150));
            ModConfig.SleepThreshold.Value = GUILayout.HorizontalSlider(
                ModConfig.SleepThreshold.Value, 0.001f, 0.1f);
            GUILayout.Label($"{ModConfig.SleepThreshold.Value:F4}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            if (GUILayout.Button("Apply Physics Settings", UIStyles.ButtonStyle))
            {
                Plugin.PhysicsOpt?.Apply();
            }
        }

        private void DrawCullingTab()
        {
            GUILayout.Label("Culling Optimization (CullFactory)", UIStyles.SectionHeaderStyle);
            GUILayout.Space(5);

            ModConfig.EnableCullingOpt.Value = GUILayout.Toggle(
                ModConfig.EnableCullingOpt.Value, "Enable Culling Optimization", UIStyles.ToggleStyle);
            ModConfig.EnableRoomCulling.Value = GUILayout.Toggle(
                ModConfig.EnableRoomCulling.Value, "Enable Room Culling", UIStyles.ToggleStyle);
            ModConfig.EnableFrustumCulling.Value = GUILayout.Toggle(
                ModConfig.EnableFrustumCulling.Value, "Enable Enhanced Frustum Culling", UIStyles.ToggleStyle);

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Culling Distance:", UIStyles.LabelStyle, GUILayout.Width(150));
            ModConfig.CullingDistance.Value = GUILayout.HorizontalSlider(
                ModConfig.CullingDistance.Value, 50f, 500f);
            GUILayout.Label($"{ModConfig.CullingDistance.Value:F0}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Frustum Margin:", UIStyles.LabelStyle, GUILayout.Width(150));
            ModConfig.FrustumCullingMargin.Value = GUILayout.HorizontalSlider(
                ModConfig.FrustumCullingMargin.Value, 1.0f, 2.0f);
            GUILayout.Label($"{ModConfig.FrustumCullingMargin.Value:F2}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            if (GUILayout.Button("Apply Culling Settings", UIStyles.ButtonStyle))
            {
                Plugin.CullingOpt?.Apply();
            }
        }

        private void DrawParticlesTab()
        {
            GUILayout.Label("Particle Optimization (NoRainParticles)", UIStyles.SectionHeaderStyle);
            GUILayout.Space(5);

            ModConfig.EnableParticleOpt.Value = GUILayout.Toggle(
                ModConfig.EnableParticleOpt.Value, "Enable Particle Optimization", UIStyles.ToggleStyle);
            ModConfig.DisableRainParticles.Value = GUILayout.Toggle(
                ModConfig.DisableRainParticles.Value, "Disable Rain Particles", UIStyles.ToggleStyle);

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Particle Cull Distance:", UIStyles.LabelStyle, GUILayout.Width(150));
            ModConfig.ParticleCullingDistance.Value = GUILayout.HorizontalSlider(
                ModConfig.ParticleCullingDistance.Value, 20f, 300f);
            GUILayout.Label($"{ModConfig.ParticleCullingDistance.Value:F0}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Max Particles/System:", UIStyles.LabelStyle, GUILayout.Width(150));
            ModConfig.MaxParticles.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.MaxParticles.Value, 50, 2000);
            GUILayout.Label($"{ModConfig.MaxParticles.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Update Rate (sec):", UIStyles.LabelStyle, GUILayout.Width(150));
            ModConfig.ParticleUpdateRate.Value = GUILayout.HorizontalSlider(
                ModConfig.ParticleUpdateRate.Value, 0.01f, 0.5f);
            GUILayout.Label($"{ModConfig.ParticleUpdateRate.Value:F2}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            if (GUILayout.Button("Apply Particle Settings", UIStyles.ButtonStyle))
            {
                Plugin.ParticleOpt?.Apply();
            }
        }

        private void DrawLightingTab()
        {
            GUILayout.Label("Lighting Optimization (LightsOut)", UIStyles.SectionHeaderStyle);
            GUILayout.Space(5);

            ModConfig.EnableLightingOpt.Value = GUILayout.Toggle(
                ModConfig.EnableLightingOpt.Value, "Enable Lighting Optimization", UIStyles.ToggleStyle);
            ModConfig.DisableDynamicShadows.Value = GUILayout.Toggle(
                ModConfig.DisableDynamicShadows.Value, "Disable Dynamic Shadows", UIStyles.ToggleStyle);

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Light Cull Distance:", UIStyles.LabelStyle, GUILayout.Width(150));
            ModConfig.LightCullingDistance.Value = GUILayout.HorizontalSlider(
                ModConfig.LightCullingDistance.Value, 20f, 300f);
            GUILayout.Label($"{ModConfig.LightCullingDistance.Value:F0}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Max Dynamic Lights:", UIStyles.LabelStyle, GUILayout.Width(150));
            ModConfig.MaxDynamicLights.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.MaxDynamicLights.Value, 1, 32);
            GUILayout.Label($"{ModConfig.MaxDynamicLights.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Shadow Cascades:", UIStyles.LabelStyle, GUILayout.Width(150));
            ModConfig.ShadowCascades.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.ShadowCascades.Value, 1, 4);
            GUILayout.Label($"{ModConfig.ShadowCascades.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            if (GUILayout.Button("Apply Lighting Settings", UIStyles.ButtonStyle))
            {
                Plugin.LightingOpt?.Apply();
            }
        }

        private void DrawAudioTab()
        {
            GUILayout.Label("Audio Optimization", UIStyles.SectionHeaderStyle);
            GUILayout.Space(5);

            ModConfig.EnableAudioOpt.Value = GUILayout.Toggle(
                ModConfig.EnableAudioOpt.Value, "Enable Audio Optimization", UIStyles.ToggleStyle);
            ModConfig.DisableReverb.Value = GUILayout.Toggle(
                ModConfig.DisableReverb.Value, "Disable Reverb Effects", UIStyles.ToggleStyle);

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Audio Cull Distance:", UIStyles.LabelStyle, GUILayout.Width(150));
            ModConfig.AudioCullingDistance.Value = GUILayout.HorizontalSlider(
                ModConfig.AudioCullingDistance.Value, 30f, 500f);
            GUILayout.Label($"{ModConfig.AudioCullingDistance.Value:F0}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Max Audio Sources:", UIStyles.LabelStyle, GUILayout.Width(150));
            ModConfig.MaxAudioSources.Value = (int)GUILayout.HorizontalSlider(
                ModConfig.MaxAudioSources.Value, 4, 64);
            GUILayout.Label($"{ModConfig.MaxAudioSources.Value}", UIStyles.LabelStyle, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            if (GUILayout.Button("Apply Audio Settings", UIStyles.ButtonStyle))
            {
                Plugin.AudioOpt?.Apply();
            }
        }

        private void DrawPresetsTab()
        {
            GUILayout.Label("Quality Presets", UIStyles.SectionHeaderStyle);
            GUILayout.Space(5);
            GUILayout.Label("Select a preset to apply optimized settings for your hardware.", UIStyles.LabelStyle);
            GUILayout.Space(10);

            if (GUILayout.Button("ULTRA - Maximum Visual Quality", UIStyles.ButtonStyle))
            {
                Plugin.QualityOpt?.ApplyPreset("Ultra");
            }
            GUILayout.Space(3);
            GUILayout.Label("Best visuals, light performance optimizations", UIStyles.LabelStyle);
            GUILayout.Space(8);

            if (GUILayout.Button("HIGH - High Quality", UIStyles.ButtonStyle))
            {
                Plugin.QualityOpt?.ApplyPreset("High");
            }
            GUILayout.Space(3);
            GUILayout.Label("Great visuals with moderate performance gains", UIStyles.LabelStyle);
            GUILayout.Space(8);

            if (GUILayout.Button("BALANCED - Recommended", UIStyles.ButtonStyle))
            {
                Plugin.QualityOpt?.ApplyPreset("Balanced");
            }
            GUILayout.Space(3);
            GUILayout.Label("Good balance between visuals and performance", UIStyles.LabelStyle);
            GUILayout.Space(8);

            if (GUILayout.Button("PERFORMANCE - FPS Priority", UIStyles.ButtonStyle))
            {
                Plugin.QualityOpt?.ApplyPreset("Performance");
            }
            GUILayout.Space(3);
            GUILayout.Label("Significant FPS boost, reduced visual quality", UIStyles.LabelStyle);
            GUILayout.Space(8);

            if (GUILayout.Button("EXTREME - Maximum FPS", UIStyles.ButtonStyle))
            {
                Plugin.QualityOpt?.ApplyPreset("Extreme");
            }
            GUILayout.Space(3);
            GUILayout.Label("Maximum FPS on low-end hardware, minimal visuals", UIStyles.LabelStyle);
            GUILayout.Space(15);

            GUILayout.Label("--- Combined Optimization ---", UIStyles.SectionHeaderStyle);
            GUILayout.Space(5);

            if (GUILayout.Button("Apply Full Optimization Suite", UIStyles.ButtonStyle))
            {
                Plugin.QualityOpt?.ApplyPreset("Performance");
                Plugin.ApplyAllOptimizations();
            }
            GUILayout.Space(3);
            GUILayout.Label("Applies Performance preset + all optimization modules", UIStyles.LabelStyle);
        }
    }
}
