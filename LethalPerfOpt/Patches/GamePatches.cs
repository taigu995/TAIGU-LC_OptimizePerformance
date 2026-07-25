using HarmonyLib;
using UnityEngine;
using TAIGU_LC_OptimizePerformance.Config;

namespace TAIGU_LC_OptimizePerformance.Patches
{
    /// <summary>
    /// 渲染优化 Harmony 补丁
    /// </summary>
    [HarmonyPatch]
    public static class RenderPatches
    {
        /// <summary>
        /// 补丁 QualitySettings 的 targetFrameRate 设置，保持我们的帧率上限
        /// </summary>
        [HarmonyPatch(typeof(Application), "set_targetFrameRate")]
        [HarmonyPrefix]
        public static bool TargetFrameRatePatch(ref int value)
        {
            if (ModConfig.MaxFPS.Value > 0)
            {
                value = ModConfig.MaxFPS.Value;
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
            if (ModConfig.EnableMemoryOpt.Value)
            {
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                Resources.UnloadUnusedAssets();
            }
        }
    }
}