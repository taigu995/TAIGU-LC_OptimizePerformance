using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using TAIGU_LC_OptimizePerformance.Config;

namespace TAIGU_LC_OptimizePerformance.Patches
{
    /// <summary>
    /// HUD 宽高比解锁补丁。
    /// 参考 Fix-Camera-Resolution 的 FCRHUDPatches：
    /// - 关闭固定宽高比，让 HUD 随窗口大小动态缩放
    /// - 默认固定 1.76:1，解锁后适配窗口实际比例
    /// </summary>
    [HarmonyPatch]
    public static class HUDPatches
    {
        public static bool IsFixedAspect => ModConfig.FixedAspectRatio == null || ModConfig.FixedAspectRatio.Value;

        [HarmonyPatch(typeof(HUDManager), "Start")]
        [HarmonyPostfix]
        static void HUDManager_Start_Postfix(HUDManager __instance) => UpdateHUDManager(__instance);

        [HarmonyPatch(typeof(HUDManager), "Update")]
        [HarmonyPostfix]
        static void HUDManager_Update_Postfix(HUDManager __instance)
        {
            // 检测屏幕分辨率变化时更新
            if (_lastScreenWidth != Screen.width || _lastScreenHeight != Screen.height)
            {
                UpdateHUDManager(__instance);
                _lastScreenWidth = Screen.width;
                _lastScreenHeight = Screen.height;
            }
        }

        static int _lastScreenWidth;
        static int _lastScreenHeight;

        public static void UpdateHUDManager(HUDManager instance)
        {
            if (instance == null) return;

            AspectRatioFitter hudContainer = instance.HUDContainer.GetComponent<AspectRatioFitter>();
            if (hudContainer != null)
                UpdateFitter(hudContainer);

            Transform hudContainerParent = instance.HUDContainer.transform.parent;
            Transform panelTransform = hudContainerParent != null ? hudContainerParent.Find("Panel") : null;

            if (panelTransform != null)
            {
                AspectRatioFitter panel = panelTransform.GetComponent<AspectRatioFitter>();
                if (panel != null)
                    UpdateFitter(panel);
            }
        }

        static void UpdateFitter(AspectRatioFitter fitter)
        {
            if (IsFixedAspect)
                fitter.aspectRatio = 1.76f;
            else
                fitter.aspectRatio = (float)Screen.width / Screen.height;
        }
    }
}