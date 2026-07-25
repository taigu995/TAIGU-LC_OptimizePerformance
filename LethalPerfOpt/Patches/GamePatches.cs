using HarmonyLib;
using UnityEngine;
using TAIGU_LC_OptimizePerformance.Config;

namespace TAIGU_LC_OptimizePerformance.Patches
{
    /// <summary>
    /// Harmony patches for render optimization.
    /// Intercepts game rendering calls to apply optimizations.
    /// </summary>
    [HarmonyPatch]
    public static class RenderPatches
    {
        /// <summary>
        /// Patch Camera setup to apply render distance and culling settings.
        /// </summary>
        [HarmonyPatch(typeof(Camera), "Awake")]
        [HarmonyPostfix]
        public static void CameraAwakePatch(Camera __instance)
        {
            if (!ModConfig.EnableRenderOpt.Value) return;

            // Apply render distance to new cameras
            __instance.farClipPlane = ModConfig.MaxDrawDistance.Value;

            // Disable HDR on non-main cameras for performance
            if (__instance != Camera.main)
            {
                __instance.allowHDR = false;
                __instance.allowMSAA = false;
            }
        }

        /// <summary>
        /// Patch QualitySettings changes to maintain our optimizations.
        /// </summary>
        [HarmonyPatch(typeof(Application), "set_targetFrameRate")]
        [HarmonyPrefix]
        public static bool TargetFrameRatePatch(ref int value)
        {
            // Only allow the game to change frame rate if we haven't set a limit
            if (ModConfig.MaxFPS.Value > 0)
            {
                value = ModConfig.MaxFPS.Value;
            }
            return true;
        }
    }

    /// <summary>
    /// Harmony patches for particle optimization.
    /// </summary>
    [HarmonyPatch]
    public static class ParticlePatches
    {
        /// <summary>
        /// Patch ParticleSystem.Play to apply distance culling.
        /// </summary>
        [HarmonyPatch(typeof(ParticleSystem), "Play")]
        [HarmonyPrefix]
        public static bool ParticleSystemPlayPatch(ParticleSystem __instance)
        {
            if (!ModConfig.EnableParticleOpt.Value) return true;

            Camera mainCam = Camera.main;
            if (mainCam == null || __instance == null) return true;

            float distance = Vector3.Distance(
                mainCam.transform.position,
                __instance.transform.position);

            // Don't play particles beyond culling distance
            if (distance > ModConfig.ParticleCullingDistance.Value)
            {
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Harmony patches for lighting optimization.
    /// </summary>
    [HarmonyPatch]
    public static class LightingPatches
    {
        /// <summary>
        /// Patch Light component to apply distance-based optimization.
        /// </summary>
        [HarmonyPatch(typeof(Light), "Awake")]
        [HarmonyPostfix]
        public static void LightAwakePatch(Light __instance)
        {
            if (!ModConfig.EnableLightingOpt.Value) return;
            if (__instance == null) return;

            // Reduce shadow quality for point/spot lights by default
            if (__instance.type == LightType.Point || __instance.type == LightType.Spot)
            {
                if (ModConfig.DisableDynamicShadows.Value)
                {
                    __instance.shadows = LightShadows.None;
                }

                // Reduce light range for performance
                if (__instance.range > ModConfig.LightCullingDistance.Value)
                {
                    __instance.range = ModConfig.LightCullingDistance.Value;
                }
            }
        }
    }

    /// <summary>
    /// Harmony patches for physics optimization.
    /// </summary>
    [HarmonyPatch]
    public static class PhysicsPatches
    {
        /// <summary>
        /// Patch Rigidbody to optimize sleeping behavior.
        /// </summary>
        [HarmonyPatch(typeof(Rigidbody), "Awake")]
        [HarmonyPostfix]
        public static void RigidbodyAwakePatch(Rigidbody __instance)
        {
            if (!ModConfig.EnablePhysicsOpt.Value) return;
            if (__instance == null) return;

            // Apply optimized sleep threshold
            __instance.sleepThreshold = ModConfig.SleepThreshold.Value;

            // Reduce collision detection mode for non-essential objects
            string name = __instance.gameObject.name.ToLower();
            if (!name.Contains("player") && !name.Contains("enemy"))
            {
                __instance.collisionDetectionMode = CollisionDetectionMode.Discrete;
                __instance.maxAngularVelocity = 7f; // Reduced from default
            }
        }
    }

    /// <summary>
    /// Harmony patches for audio optimization.
    /// </summary>
    [HarmonyPatch]
    public static class AudioPatches
    {
        /// <summary>
        /// Patch AudioSource.Play to apply distance culling.
        /// </summary>
        [HarmonyPatch(typeof(AudioSource), "Play")]
        [HarmonyPrefix]
        public static bool AudioSourcePlayPatch(AudioSource __instance)
        {
            if (!ModConfig.EnableAudioOpt.Value) return true;
            if (__instance == null) return true;

            Camera mainCam = Camera.main;
            if (mainCam == null) return true;

            float distance = Vector3.Distance(
                mainCam.transform.position,
                __instance.transform.position);

            // Don't play audio beyond culling distance
            if (distance > ModConfig.AudioCullingDistance.Value)
            {
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Harmony patches for scene loading optimization.
    /// </summary>
    [HarmonyPatch]
    public static class ScenePatches
    {
        /// <summary>
        /// Patch scene loading to trigger memory cleanup.
        /// </summary>
        [HarmonyPatch(typeof(UnityEngine.SceneManagement.SceneManager), "LoadScene",
            new System.Type[] { typeof(string) })]
        [HarmonyPrefix]
        public static void SceneLoadPatch(string sceneName)
        {
            // Force GC before scene load to reduce memory spike
            if (ModConfig.EnableMemoryOpt.Value)
            {
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                Resources.UnloadUnusedAssets();
            }
        }
    }
}
