using HarmonyLib;
using UnityEngine;
using GameNetcodeStuff;
using TAIGU_LC_OptimizePerformance.Config;

namespace TAIGU_LC_OptimizePerformance.Patches
{
    /// <summary>
    /// 摄像头分辨率自定义补丁。
    /// 参考 Fix-Camera-Resolution 的 FCRResPatches：
    /// - 修改终端显示屏的 RenderTexture 分辨率
    /// - 重置玩家摄像机宽高比
    /// - 默认 860x520 → 可自定义最高 3840x2160
    /// </summary>
    [HarmonyPatch]
    public static class FixCameraResPatches
    {
        public const int OrgWidth = 860;
        public const int OrgHeight = 520;

        public static bool IsEnabled => ModConfig.EnableCameraResCustom != null && ModConfig.EnableCameraResCustom.Value;

        public static int TargetWidth
        {
            get
            {
                if (!IsEnabled) return OrgWidth;
                if (ModConfig.CameraResAutoSize.Value) return Screen.width;
                return ModConfig.CameraResWidth.Value;
            }
        }

        public static int TargetHeight
        {
            get
            {
                if (!IsEnabled) return OrgHeight;
                if (ModConfig.CameraResAutoSize.Value) return Screen.height;
                return ModConfig.CameraResHeight.Value;
            }
        }

        /// <summary>终端 Start 补丁 - 修改终端屏幕分辨率</summary>
        [HarmonyPatch(typeof(Terminal), "Start")]
        [HarmonyPostfix]
        private static void Terminal_Start_Postfix(Terminal __instance)
        {
            if (!IsEnabled) return;
            UpdateRenderTexture(__instance.playerScreenTex);
            UpdateRenderTexture(__instance.playerScreenTexHighRes);
        }

        /// <summary>玩家设置分辨率补丁 - 重置摄像机宽高比</summary>
        [HarmonyPatch(typeof(IngamePlayerSettings), "SetPixelResolution")]
        [HarmonyPostfix]
        private static void IngamePlayerSettings_SetPixelResolution_Postfix()
        {
            if (!IsEnabled) return;
            UpdateAllPlayerCameras();
        }

        /// <summary>HUDManager Update 补丁 - 检测屏幕分辨率变化</summary>
        [HarmonyPatch(typeof(HUDManager), "Update")]
        [HarmonyPostfix]
        private static void HUDManager_Update_Postfix()
        {
            if (!IsEnabled || !ModConfig.CheckResEveryFrame.Value) return;
            CheckScreenResChange();
        }

        private static int _lastScreenWidth;
        private static int _lastScreenHeight;

        private static void CheckScreenResChange()
        {
            if (_lastScreenWidth != Screen.width || _lastScreenHeight != Screen.height)
            {
                UpdateAll();
                _lastScreenWidth = Screen.width;
                _lastScreenHeight = Screen.height;
            }
        }

        public static void UpdateAll()
        {
            UpdateAllTerminals();
            UpdateAllPlayerCameras();
        }

        public static void UpdateAllTerminals()
        {
            Terminal[] terminals = Object.FindObjectsByType<Terminal>(FindObjectsSortMode.None);
            for (int i = 0; i < terminals.Length; i++)
            {
                UpdateRenderTexture(terminals[i].playerScreenTex);
                UpdateRenderTexture(terminals[i].playerScreenTexHighRes);
            }
        }

        public static void UpdateRenderTexture(RenderTexture renderTexture)
        {
            if (renderTexture == null) return;
            renderTexture.Release();
            renderTexture.width = TargetWidth;
            renderTexture.height = TargetHeight;
        }

        public static void UpdateAllPlayerCameras()
        {
            PlayerControllerB[] players = Object.FindObjectsByType<PlayerControllerB>(FindObjectsSortMode.None);
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].gameplayCamera != null)
                    players[i].gameplayCamera.ResetAspect();
            }
        }
    }
}