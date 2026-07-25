using HarmonyLib;
using UnityEngine;
using TAIGU_LC_OptimizePerformance.UI;
using GameNetcodeStuff;

namespace TAIGU_LC_OptimizePerformance.Patches
{
    /// <summary>
    /// Harmony 补丁：钩入游戏对象的 OnGUI 方法，确保 UI 始终渲染
    /// 使用 PlayerControllerB 作为钩子对象，因为它在游戏中始终存在
    /// </summary>
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal static class OnGUIHookPatch
    {
        private static UIRenderer _renderer;

        /// <summary>
        /// 设置渲染器实例
        /// </summary>
        public static void SetRenderer(UIRenderer renderer)
        {
            _renderer = renderer;
        }

        [HarmonyPostfix]
        [HarmonyPatch("OnGUI")]
        private static void OnGUIPostfix()
        {
            _renderer?.RenderGUI();
        }
    }
}