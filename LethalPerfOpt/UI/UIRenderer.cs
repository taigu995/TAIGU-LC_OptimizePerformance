using UnityEngine;
using TAIGU_LC_OptimizePerformance.Config;

namespace TAIGU_LC_OptimizePerformance.UI
{
    /// <summary>
    /// 独立的 MonoBehaviour 组件，用于渲染 UI。
    /// 创建独立的 GameObject 并添加到场景中，确保 OnGUI 被 Unity 调用。
    /// </summary>
    public class UIRenderer : MonoBehaviour
    {
        private PerformanceUI _perfUI;
        private bool _initialized;

        public void Initialize(PerformanceUI perfUI)
        {
            _perfUI = perfUI;
            _initialized = true;
            Plugin.LogSource.LogInfo("[TAIGU-LC_OptimizePerformance] UIRenderer 已初始化");
        }

        /// <summary>
        /// 由 Harmony 补丁调用的渲染方法（备用方案）
        /// </summary>
        public void RenderGUI()
        {
            if (!_initialized || _perfUI == null) return;

            if (_perfUI.IsVisible || _perfUI.ShowFPS)
            {
                _perfUI.OnGUI();
            }
        }

        private void Update()
        {
            if (!_initialized || _perfUI == null) return;

            // Update performance monitor
            Plugin.PerfMonitor.Update();

            // Update UI if visible
            if (_perfUI.IsVisible)
            {
                _perfUI.Update();
            }

            // 使用 Input.GetKeyDown 检测按键（在 Update 中可靠）
            if (Input.GetKeyDown(ModConfig.ToggleUIKey.Value))
            {
                _perfUI.IsVisible = !_perfUI.IsVisible;
                Plugin.LogSource.LogInfo($"[TAIGU-LC_OptimizePerformance] UI 切换: {_perfUI.IsVisible}");
            }

            if (Input.GetKeyDown(ModConfig.ToggleFPSKey.Value))
            {
                _perfUI.ShowFPS = !_perfUI.ShowFPS;
                Plugin.LogSource.LogInfo($"[TAIGU-LC_OptimizePerformance] FPS 显示切换: {_perfUI.ShowFPS}");
            }
        }

        private void OnGUI()
        {
            if (!_initialized || _perfUI == null) return;

            if (_perfUI.IsVisible || _perfUI.ShowFPS)
            {
                _perfUI.OnGUI();
            }
        }
    }
}