using UnityEngine;
using TAIGU_LC_OptimizePerformance.Config;

namespace TAIGU_LC_OptimizePerformance.UI
{
    /// <summary>
    /// 独立的 MonoBehaviour 组件，用于渲染 UI。
    /// 通过 gameObject.AddComponent<UIRenderer>() 添加到 BaseUnityPlugin 的 GameObject 上。
    /// 这样可以确保 OnGUI() 被 Unity 正确调用。
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
        }

        private void OnGUI()
        {
            if (!_initialized || _perfUI == null) return;

            // Handle hotkey for UI toggle (using Event for reliability)
            if (Event.current != null && Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == ModConfig.ToggleUIKey.Value)
                {
                    _perfUI.IsVisible = !_perfUI.IsVisible;
                    Plugin.LogSource.LogInfo($"[TAIGU-LC_OptimizePerformance] UI 切换: {_perfUI.IsVisible}");
                    Event.current.Use();
                }
                else if (Event.current.keyCode == ModConfig.ToggleFPSKey.Value)
                {
                    _perfUI.ShowFPS = !_perfUI.ShowFPS;
                    Plugin.LogSource.LogInfo($"[TAIGU-LC_OptimizePerformance] FPS 显示切换: {_perfUI.ShowFPS}");
                    Event.current.Use();
                }
            }

            // Render UI
            if (_perfUI.IsVisible || _perfUI.ShowFPS)
            {
                _perfUI.OnGUI();
            }
        }
    }
}
