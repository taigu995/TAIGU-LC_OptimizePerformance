using HarmonyLib;
using UnityEngine;
using TAIGU_LC_OptimizePerformance.Config;
using TAIGU_LC_OptimizePerformance.UI;

namespace TAIGU_LC_OptimizePerformance.Patches
{
    /// <summary>
    /// 输入处理补丁 - 使用显式 Harmony.Patch() 模式
    /// 所有方法必须是 public static 以支持 Harmony.Patch() 直接引用
    /// </summary>
    public static class InputHandler
    {
        private static int _pcCallCount;
        private static int _hudCallCount;

        /// <summary>
        /// PlayerControllerB.Update 的后置补丁 - 每帧检测按键
        /// </summary>
        public static void PlayerControllerB_Update()
        {
            _pcCallCount++;

            if (_pcCallCount % 300 == 1)
            {
                Plugin.LogSource.LogInfo($"[TAIGU-InputPatch] PC.Update 触发中... 已调用 {_pcCallCount} 次");
            }

            // 通过 UIRenderer 处理按键
            var uiRenderer = UIRenderer.Instance;
            if (uiRenderer != null)
            {
                uiRenderer.HandleInput();
            }
            else if (Plugin.PerfUI != null)
            {
                HandleInputDirect();
            }
        }

        /// <summary>
        /// HUDManager.Update 的后置补丁 - 每帧处理按键和渲染 UI
        /// </summary>
        public static void HUDManager_Update()
        {
            _hudCallCount++;

            if (_hudCallCount % 300 == 1)
            {
                Plugin.LogSource.LogInfo($"[TAIGU-GUIPatch] HUD.Update 触发中... 已调用 {_hudCallCount} 次");
            }

            // 通过 UIRenderer 处理按键和渲染
            var uiRenderer = UIRenderer.Instance;
            if (uiRenderer != null)
            {
                uiRenderer.HandleInput();
                uiRenderer.RenderGUI();
            }
            else if (Plugin.PerfUI != null)
            {
                HandleInputDirect();

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
            }
        }

        private static void HandleInputDirect()
        {
            try
            {
                var keyboard = UnityEngine.InputSystem.Keyboard.current;
                if (keyboard != null)
                {
                    if (keyboard.f5Key.wasPressedThisFrame)
                    {
                        Plugin.PerfUI.IsVisible = !Plugin.PerfUI.IsVisible;
                        Plugin.LogSource.LogInfo($"[TAIGU-Input][直接] UI 切换: {Plugin.PerfUI.IsVisible}");
                    }

                    if (keyboard.f6Key.wasPressedThisFrame)
                    {
                        Plugin.PerfUI.ShowFPS = !Plugin.PerfUI.ShowFPS;
                        Plugin.LogSource.LogInfo($"[TAIGU-Input][直接] FPS 切换: {Plugin.PerfUI.ShowFPS}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                Plugin.LogSource.LogError($"[TAIGU-Input] 按键异常: {ex.Message}");
            }
        }
    }
}