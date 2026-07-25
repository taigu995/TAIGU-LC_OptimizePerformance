using HarmonyLib;
using UnityEngine;
using TAIGU_LC_OptimizePerformance.Config;

namespace TAIGU_LC_OptimizePerformance.Patches
{
    /// <summary>
    /// 渲染优化 Harmony 补丁
    /// 拦截游戏渲染调用以应用优化
    /// </summary>
    [HarmonyPatch]
    public static class RenderPatches
    {
        /// <summary>
        /// 补丁 Camera 初始化，应用绘制距离和遮挡剔除设置
        /// </summary>
        [HarmonyPatch(typeof(Camera), "Awake")]
        [HarmonyPostfix]
        public static void CameraAwakePatch(Camera __instance)
        {
            if (!ModConfig.EnableRenderOpt.Value) return;

            // 为新相机应用绘制距离
            __instance.farClipPlane = ModConfig.MaxDrawDistance.Value;

            // 非主相机关闭 HDR 以提升性能
            if (__instance != Camera.main)
            {
                __instance.allowHDR = false;
                __instance.allowMSAA = false;
            }
        }

        /// <summary>
        /// 补丁 QualitySettings 变更，保持我们的优化设置
        /// </summary>
        [HarmonyPatch(typeof(Application), "set_targetFrameRate")]
        [HarmonyPrefix]
        public static bool TargetFrameRatePatch(ref int value)
        {
            // 如果我们设置了帧率上限，阻止游戏修改
            if (ModConfig.MaxFPS.Value > 0)
            {
                value = ModConfig.MaxFPS.Value;
            }
            return true;
        }
    }

    /// <summary>
    /// 粒子系统优化 Harmony 补丁
    /// </summary>
    [HarmonyPatch]
    public static class ParticlePatches
    {
        /// <summary>
        /// 补丁 ParticleSystem.Play，应用距离剔除
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

            // 超出剔除距离的粒子不播放
            if (distance > ModConfig.ParticleCullingDistance.Value)
            {
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// 灯光优化 Harmony 补丁
    /// </summary>
    [HarmonyPatch]
    public static class LightingPatches
    {
        /// <summary>
        /// 补丁 Light 组件，应用基于距离的优化
        /// </summary>
        [HarmonyPatch(typeof(Light), "Awake")]
        [HarmonyPostfix]
        public static void LightAwakePatch(Light __instance)
        {
            if (!ModConfig.EnableLightingOpt.Value) return;
            if (__instance == null) return;

            // 默认降低点光源/聚光灯的阴影质量
            if (__instance.type == LightType.Point || __instance.type == LightType.Spot)
            {
                if (ModConfig.DisableDynamicShadows.Value)
                {
                    __instance.shadows = LightShadows.None;
                }

                // 为性能缩减灯光范围
                if (__instance.range > ModConfig.LightCullingDistance.Value)
                {
                    __instance.range = ModConfig.LightCullingDistance.Value;
                }
            }
        }
    }

    /// <summary>
    /// 物理引擎优化 Harmony 补丁
    /// </summary>
    [HarmonyPatch]
    public static class PhysicsPatches
    {
        /// <summary>
        /// 补丁 Rigidbody，优化休眠行为
        /// </summary>
        [HarmonyPatch(typeof(Rigidbody), "Awake")]
        [HarmonyPostfix]
        public static void RigidbodyAwakePatch(Rigidbody __instance)
        {
            if (!ModConfig.EnablePhysicsOpt.Value) return;
            if (__instance == null) return;

            // 应用优化后的休眠阈值
            __instance.sleepThreshold = ModConfig.SleepThreshold.Value;

            // 非玩家/非敌人对象降低碰撞检测模式
            string name = __instance.gameObject.name.ToLower();
            if (!name.Contains("player") && !name.Contains("enemy"))
            {
                __instance.collisionDetectionMode = CollisionDetectionMode.Discrete;
                __instance.maxAngularVelocity = 7f; // 从默认值降低
            }
        }
    }

    /// <summary>
    /// 音频优化 Harmony 补丁
    /// </summary>
    [HarmonyPatch]
    public static class AudioPatches
    {
        /// <summary>
        /// 补丁 AudioSource.Play，应用距离剔除
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

            // 超出剔除距离的音频不播放
            if (distance > ModConfig.AudioCullingDistance.Value)
            {
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// 场景加载优化 Harmony 补丁
    /// </summary>
    [HarmonyPatch]
    public static class ScenePatches
    {
        /// <summary>
        /// 补丁场景加载，触发内存清理
        /// </summary>
        [HarmonyPatch(typeof(UnityEngine.SceneManagement.SceneManager), "LoadScene",
            new System.Type[] { typeof(string) })]
        [HarmonyPrefix]
        public static void SceneLoadPatch(string sceneName)
        {
            // 场景加载前强制 GC 以减少内存峰值
            if (ModConfig.EnableMemoryOpt.Value)
            {
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                Resources.UnloadUnusedAssets();
            }
        }
    }
}