using BepInEx.Configuration;
using UnityEngine;

namespace TAIGU_LC_OptimizePerformance.Config
{
    public static class ModConfig
    {
        // UI Settings
        public static ConfigEntry<KeyCode> ToggleUIKey;
        public static ConfigEntry<KeyCode> ToggleFPSKey;
        public static ConfigEntry<bool> EnableOnStart;

        // Render Optimization
        public static ConfigEntry<bool> EnableRenderOpt;
        public static ConfigEntry<int> MaxDrawDistance;
        public static ConfigEntry<float> LODBias;
        public static ConfigEntry<int> ShadowResolution;
        public static ConfigEntry<bool> DisableShadows;
        public static ConfigEntry<int> MaxShadowDistance;
        public static ConfigEntry<bool> EnableBatching;
        public static ConfigEntry<int> MaxFPS;

        // Memory/GC Optimization
        public static ConfigEntry<bool> EnableMemoryOpt;
        public static ConfigEntry<float> GCInterval;
        public static ConfigEntry<bool> AggressiveGC;
        public static ConfigEntry<bool> EnableComponentCaching;
        public static ConfigEntry<int> MaxCachedComponents;

        // Physics Optimization
        public static ConfigEntry<bool> EnablePhysicsOpt;
        public static ConfigEntry<float> PhysicsTimeStep;
        public static ConfigEntry<int> MaxPhysicsIterations;
        public static ConfigEntry<bool> DisableSleepingBodies;
        public static ConfigEntry<float> SleepThreshold;

        // Culling Optimization
        public static ConfigEntry<bool> EnableCullingOpt;
        public static ConfigEntry<float> CullingDistance;
        public static ConfigEntry<bool> EnableRoomCulling;
        public static ConfigEntry<bool> EnableFrustumCulling;
        public static ConfigEntry<float> FrustumCullingMargin;

        // Particle Optimization
        public static ConfigEntry<bool> EnableParticleOpt;
        public static ConfigEntry<float> ParticleCullingDistance;
        public static ConfigEntry<bool> DisableRainParticles;
        public static ConfigEntry<int> MaxParticles;
        public static ConfigEntry<float> ParticleUpdateRate;

        // Lighting Optimization
        public static ConfigEntry<bool> EnableLightingOpt;
        public static ConfigEntry<float> LightCullingDistance;
        public static ConfigEntry<int> MaxDynamicLights;
        public static ConfigEntry<bool> DisableDynamicShadows;
        public static ConfigEntry<int> ShadowCascades;

        // Audio Optimization
        public static ConfigEntry<bool> EnableAudioOpt;
        public static ConfigEntry<float> AudioCullingDistance;
        public static ConfigEntry<int> MaxAudioSources;
        public static ConfigEntry<bool> DisableReverb;

        // Quality Presets
        public static ConfigEntry<string> QualityPreset;

        public static void Init(ConfigFile config)
        {
            // UI Section
            ToggleUIKey = config.Bind("UI", "Toggle UI Key", KeyCode.F5,
                "Key to toggle the performance optimization panel");
            ToggleFPSKey = config.Bind("UI", "Toggle FPS Key", KeyCode.F6,
                "Key to toggle FPS counter display");
            EnableOnStart = config.Bind("UI", "Enable On Start", true,
                "Automatically apply optimizations when game starts");

            // Render Optimization Section
            EnableRenderOpt = config.Bind("Render", "Enable Render Optimization", true,
                "Enable render pipeline optimizations");
            MaxDrawDistance = config.Bind("Render", "Max Draw Distance", 500,
                new ConfigDescription("Maximum render distance for objects", new AcceptableValueRange<int>(100, 2000)));
            LODBias = config.Bind("Render", "LOD Bias", 1.0f,
                new ConfigDescription("Level of Detail bias (lower = more aggressive LOD switching)", new AcceptableValueRange<float>(0.1f, 3.0f)));
            ShadowResolution = config.Bind("Render", "Shadow Resolution", 1024,
                new ConfigDescription("Shadow map resolution", new AcceptableValueRange<int>(256, 4096)));
            DisableShadows = config.Bind("Render", "Disable Shadows", false,
                "Completely disable all shadows for maximum performance");
            MaxShadowDistance = config.Bind("Render", "Max Shadow Distance", 150,
                new ConfigDescription("Maximum distance for shadow rendering", new AcceptableValueRange<int>(50, 500)));
            EnableBatching = config.Bind("Render", "Enable Static Batching", true,
                "Enable static batching for draw call reduction");
            MaxFPS = config.Bind("Render", "Max FPS Limit", 0,
                new ConfigDescription("Maximum FPS limit (0 = unlimited)", new AcceptableValueRange<int>(0, 300)));

            // Memory/GC Optimization Section
            EnableMemoryOpt = config.Bind("Memory", "Enable Memory Optimization", true,
                "Enable memory and garbage collection optimizations");
            GCInterval = config.Bind("Memory", "GC Interval", 5.0f,
                new ConfigDescription("Interval between forced GC collections (seconds)", new AcceptableValueRange<float>(1.0f, 30.0f)));
            AggressiveGC = config.Bind("Memory", "Aggressive GC", false,
                "Enable aggressive garbage collection (may cause micro-stutters)");
            EnableComponentCaching = config.Bind("Memory", "Enable Component Caching", true,
                "Cache frequently accessed components to reduce GC pressure (from LethalPerformance)");
            MaxCachedComponents = config.Bind("Memory", "Max Cached Components", 500,
                new ConfigDescription("Maximum number of cached components", new AcceptableValueRange<int>(100, 2000)));

            // Physics Optimization Section
            EnablePhysicsOpt = config.Bind("Physics", "Enable Physics Optimization", true,
                "Enable physics system optimizations");
            PhysicsTimeStep = config.Bind("Physics", "Physics Time Step", 0.02f,
                new ConfigDescription("Fixed physics time step", new AcceptableValueRange<float>(0.01f, 0.05f)));
            MaxPhysicsIterations = config.Bind("Physics", "Max Physics Iterations", 6,
                new ConfigDescription("Maximum physics solver iterations per step", new AcceptableValueRange<int>(1, 20)));
            DisableSleepingBodies = config.Bind("Physics", "Optimize Sleeping Bodies", true,
                "Reduce update frequency for sleeping rigidbodies");
            SleepThreshold = config.Bind("Physics", "Sleep Threshold", 0.005f,
                new ConfigDescription("Velocity threshold for sleep detection", new AcceptableValueRange<float>(0.001f, 0.1f)));

            // Culling Optimization Section
            EnableCullingOpt = config.Bind("Culling", "Enable Culling Optimization", true,
                "Enable object culling optimizations (inspired by CullFactory)");
            CullingDistance = config.Bind("Culling", "Culling Distance", 200f,
                new ConfigDescription("Distance beyond which objects are culled", new AcceptableValueRange<float>(50f, 500f)));
            EnableRoomCulling = config.Bind("Culling", "Enable Room Culling", true,
                "Disable rendering of rooms not visible to player (CullFactory feature)");
            EnableFrustumCulling = config.Bind("Culling", "Enable Frustum Culling", true,
                "Enhanced frustum culling for off-screen objects");
            FrustumCullingMargin = config.Bind("Culling", "Frustum Margin", 1.2f,
                new ConfigDescription("Extra margin for frustum culling bounds", new AcceptableValueRange<float>(1.0f, 2.0f)));

            // Particle Optimization Section
            EnableParticleOpt = config.Bind("Particles", "Enable Particle Optimization", true,
                "Enable particle system optimizations (inspired by NoRainParticles)");
            ParticleCullingDistance = config.Bind("Particles", "Particle Culling Distance", 100f,
                new ConfigDescription("Distance beyond which particles are hidden", new AcceptableValueRange<float>(20f, 300f)));
            DisableRainParticles = config.Bind("Particles", "Disable Rain Particles", false,
                "Completely disable rain particle effects for performance");
            MaxParticles = config.Bind("Particles", "Max Particles Per System", 500,
                new ConfigDescription("Maximum particles per particle system", new AcceptableValueRange<int>(50, 2000)));
            ParticleUpdateRate = config.Bind("Particles", "Particle Update Rate", 0.05f,
                new ConfigDescription("How often to update distant particles (seconds)", new AcceptableValueRange<float>(0.01f, 0.5f)));

            // Lighting Optimization Section
            EnableLightingOpt = config.Bind("Lighting", "Enable Lighting Optimization", true,
                "Enable lighting system optimizations (inspired by LightsOut)");
            LightCullingDistance = config.Bind("Lighting", "Light Culling Distance", 100f,
                new ConfigDescription("Distance beyond which dynamic lights are disabled", new AcceptableValueRange<float>(20f, 300f)));
            MaxDynamicLights = config.Bind("Lighting", "Max Dynamic Lights", 8,
                new ConfigDescription("Maximum number of active dynamic lights", new AcceptableValueRange<int>(1, 32)));
            DisableDynamicShadows = config.Bind("Lighting", "Disable Dynamic Shadows", false,
                "Disable shadows from dynamic lights only");
            ShadowCascades = config.Bind("Lighting", "Shadow Cascades", 2,
                new ConfigDescription("Number of shadow cascades", new AcceptableValueRange<int>(1, 4)));

            // Audio Optimization Section
            EnableAudioOpt = config.Bind("Audio", "Enable Audio Optimization", true,
                "Enable audio system optimizations");
            AudioCullingDistance = config.Bind("Audio", "Audio Culling Distance", 150f,
                new ConfigDescription("Distance beyond which audio sources are muted", new AcceptableValueRange<float>(30f, 500f)));
            MaxAudioSources = config.Bind("Audio", "Max Audio Sources", 16,
                new ConfigDescription("Maximum number of simultaneously playing audio sources", new AcceptableValueRange<int>(4, 64)));
            DisableReverb = config.Bind("Audio", "Disable Reverb", false,
                "Disable reverb effects for performance");

            // Quality Presets Section
            QualityPreset = config.Bind("Presets", "Quality Preset", "Balanced",
                new ConfigDescription("Quality preset: Ultra, High, Balanced, Performance, Extreme",
                new AcceptableValueList<string>("Ultra", "High", "Balanced", "Performance", "Extreme")));
        }
    }
}
