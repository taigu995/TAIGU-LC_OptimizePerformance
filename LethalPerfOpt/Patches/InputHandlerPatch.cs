using HarmonyLib;
using UnityEngine;
using GameNetcodeStuff;
using TAIGU_LC_OptimizePerformance.Config;
using TAIGU_LC_OptimizePerformance.UI;

namespace TAIGU_LC_OptimizePerformance.Patches
{
    /// <summary>
    /// 输入处理兜底补丁。
    /// 当 UIRenderer 的 Update/OnGUI 未被 Unity 调用时，
    /// 通过 Harmony 补丁 PlayerControllerB.Update 来检测按键并渲染 UI。
    /// 这是确保 UI 可用的最后保障。
    /// </summary>
    [HarmonyPatch(typeof(PlayerControllerB))]
    public class InputHandlerPatch
    {
        private static int _callCount;

        /// <summary>
        /// 补丁 PlayerControllerB.Update - 每帧检测按键 + 渲染 UI
        /// </summary>
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void PlayerControllerB_Update()
        {
            _callCount++;

            // 每 300 帧输出一次日志
            if (_callCount % 300 == 1)
            {
                Plugin.LogSource.LogInfo($"[TAIGU-InputPatch] PlayerControllerB.Update 补丁触发中... 已调用 {_callCount} 次");
            }

            var uiRenderer = UIRenderer.Instance;
            if (uiRenderer != null)
            {
                // 使用 UIRenderer 的 HandleInput 方法检测按键
                uiRenderer.HandleInput();
            }
            else if (Plugin.PerfUI != null)
            {
                // UIRenderer 不存在时，直接处理按键
                HandleInputDirect();
            }
        }

        /// <summary>
        /// 直接处理按键（当 UIRenderer 实例不存在时的兜底）
        /// </summary>
        private static void HandleInputDirect()
        {
            try
            {
                if (Input.GetKeyDown(ModConfig.ToggleUIKey.Value))
                {
                    Plugin.PerfUI.IsVisible = !Plugin.PerfUI.IsVisible;
                    Plugin.LogSource.LogInfo($"[TAIGU-InputPatch][直接] UI 切换: {Plugin.PerfUI.IsVisible}");
                }

                if (Input.GetKeyDown(ModConfig.ToggleFPSKey.Value))
                {
                    Plugin.PerfUI.ShowFPS = !Plugin.PerfUI.ShowFPS;
                    Plugin.LogSource.LogInfo($"[TAIGU-InputPatch][直接] FPS 切换: {Plugin.PerfUI.ShowFPS}");
                }
            }
            catch (System.Exception ex)
            {
                Plugin.LogSource.LogError($"[TAIGU-InputPatch] 按键检测异常: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// UI 渲染兜底补丁 - 使用 HUDManager.Update 来检测按键和渲染 UI。
    /// HUDManager 是游戏中始终存在的 UI 管理器，其 Update 每帧执行。
    /// </summary>
    [HarmonyPatch(typeof(HUDManager))]
    public class GUIRenderPatch
    {
        private static int _callCount;

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void HUDManager_Update()
        {
            _callCount++;

            // 每 300 帧输出一次日志
            if (_callCount % 300 == 1)
            {
                Plugin.LogSource.LogInfo($"[TAIGU-GUIPatch] HUDManager.Update 补丁触发中... 已调用 {_callCount} 次");
            }

            // 尝试通过 UIRenderer 渲染
            var uiRenderer = UIRenderer.Instance;
            if (uiRenderer != null)
            {
                uiRenderer.RenderGUI();

                // 也通过 UIRenderer 处理按键（双重保险）
                uiRenderer.HandleInput();
            }
            else if (Plugin.PerfUI != null)
            {
                // 直接渲染（最终兜底）
                try
                {
                    if (Plugin.PerfUI.IsVisible || Plugin.PerfUI.ShowFPS)
                    {
                        Plugin.PerfUI.OnGUI();
                    }
                }
                catch (System.Exception ex)
                {
                    Plugin.LogSource.LogError($"[TAIGU-GUIPatch] 渲染异常: {ex.Message}");
                }

                // 直接处理按键
                HandleInputDirect();
            }
        }

        /// <summary>
        /// 直接处理按键
        /// </summary>
        private static void HandleInputDirect()
        {
            try
            {
                if (Input.GetKeyDown(ModConfig.ToggleUIKey.Value))
                {
                    Plugin.PerfUI.IsVisible = !Plugin.PerfUI.IsVisible;
                    Plugin.LogSource.LogInfo($"[TAIGU-GUIPatch][直接] UI 切换: {Plugin.PerfUI.IsVisible}");
                }

                if (Input.GetKeyDown(ModConfig.ToggleFPSKey.Value))
                {
                    Plugin.PerfUI.ShowFPS = !Plugin.PerfUI.ShowFPS;
                    Plugin.LogSource.LogInfo($"[TAIGU-GUIPatch][直接] FPS 切换: {Plugin.PerfUI.ShowFPS}");
                }
            }
            catch (System.Exception ex)
            {
                Plugin.LogSource.LogError($"[TAIGU-GUIPatch] 按键异常: {ex.Message}");
            }
        }
    }
}