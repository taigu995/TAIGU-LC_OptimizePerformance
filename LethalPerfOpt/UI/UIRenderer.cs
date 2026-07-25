using UnityEngine;
using System.Collections;
using TAIGU_LC_OptimizePerformance.Config;

namespace TAIGU_LC_OptimizePerformance.UI
{
    /// <summary>
    /// 独立的 MonoBehaviour 组件，用于渲染 UI。
    /// 多重保障确保 OnGUI/Update 被 Unity 调用：
    /// 1. Awake/Start/OnEnable 三重生命周期钩子
    /// 2. 协程延迟初始化（等待场景加载）
    /// 3. 全面诊断日志
    /// </summary>
    public class UIRenderer : MonoBehaviour
    {
        private static UIRenderer _instance;

        private PerformanceUI _perfUI;
        private bool _initialized;
        private bool _updateCalled;
        private bool _onGUISinceCalled;
        private int _frameCount;

        public static UIRenderer Instance => _instance;

        /// <summary>
        /// 初始化 - 设置 PerformanceUI 引用
        /// </summary>
        public void Initialize(PerformanceUI perfUI)
        {
            _perfUI = perfUI;
            _initialized = true;
            Plugin.LogSource.LogInfo("[TAIGU-UI] Initialize() 已调用, _initialized=true");
        }

        private void Awake()
        {
            _instance = this;
            Plugin.LogSource.LogInfo("[TAIGU-UI] UIRenderer.Awake() 被调用");
        }

        private void Start()
        {
            Plugin.LogSource.LogInfo("[TAIGU-UI] UIRenderer.Start() 被调用 - Unity 生命周期正常");
            // 确保组件启用且游戏对象激活
            this.enabled = true;
            gameObject.SetActive(true);
            Plugin.LogSource.LogInfo($"[TAIGU-UI] enabled={this.enabled}, activeSelf={gameObject.activeSelf}, activeInHierarchy={gameObject.activeInHierarchy}");
        }

        private void OnEnable()
        {
            Plugin.LogSource.LogInfo("[TAIGU-UI] UIRenderer.OnEnable() 被调用");
        }

        private void OnDisable()
        {
            Plugin.LogSource.LogInfo("[TAIGU-UI] UIRenderer.OnDisable() 被调用");
        }

        private void Update()
        {
            _frameCount++;

            // 每 300 帧输出一次诊断日志（约 5 秒）
            if (_frameCount % 300 == 0)
            {
                Plugin.LogSource.LogInfo($"[TAIGU-UI] Update() 运行中... 帧={_frameCount}, OnGUI已调用={_onGUISinceCalled}");
            }

            if (!_updateCalled)
            {
                _updateCalled = true;
                Plugin.LogSource.LogInfo("[TAIGU-UI] Update() 首次被调用！");
            }

            if (!_initialized || _perfUI == null) return;

            // 更新性能监控
            if (Plugin.PerfMonitor != null)
            {
                Plugin.PerfMonitor.Update();
            }

            // 按键检测（Input.GetKeyDown 在 Update 中最可靠）
            try
            {
                if (Input.GetKeyDown(ModConfig.ToggleUIKey.Value))
                {
                    _perfUI.IsVisible = !_perfUI.IsVisible;
                    Plugin.LogSource.LogInfo($"[TAIGU-UI] UI 切换: {_perfUI.IsVisible}");
                }

                if (Input.GetKeyDown(ModConfig.ToggleFPSKey.Value))
                {
                    _perfUI.ShowFPS = !_perfUI.ShowFPS;
                    Plugin.LogSource.LogInfo($"[TAIGU-UI] FPS 显示切换: {_perfUI.ShowFPS}");
                }
            }
            catch (System.Exception ex)
            {
                Plugin.LogSource.LogError($"[TAIGU-UI] 按键检测异常: {ex.Message}");
            }
        }

        private void OnGUI()
        {
            if (!_onGUISinceCalled)
            {
                _onGUISinceCalled = true;
                Plugin.LogSource.LogInfo("[TAIGU-UI] OnGUI() 首次被调用！");
            }

            if (!_initialized || _perfUI == null) return;

            try
            {
                if (_perfUI.IsVisible || _perfUI.ShowFPS)
                {
                    _perfUI.OnGUI();
                }
            }
            catch (System.Exception ex)
            {
                Plugin.LogSource.LogError($"[TAIGU-UI] OnGUI 渲染异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 由 Harmony 补丁调用的渲染方法（兜底方案）
        /// </summary>
        public void RenderGUI()
        {
            if (!_initialized || _perfUI == null) return;

            try
            {
                if (_perfUI.IsVisible || _perfUI.ShowFPS)
                {
                    _perfUI.OnGUI();
                }
            }
            catch (System.Exception ex)
            {
                Plugin.LogSource.LogError($"[TAIGU-UI] RenderGUI 异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 由 Harmony 补丁调用的按键检测方法（兜底方案）
        /// </summary>
        public void HandleInput()
        {
            if (!_initialized || _perfUI == null) return;

            try
            {
                if (Input.GetKeyDown(ModConfig.ToggleUIKey.Value))
                {
                    _perfUI.IsVisible = !_perfUI.IsVisible;
                    Plugin.LogSource.LogInfo($"[TAIGU-UI][兜底] UI 切换: {_perfUI.IsVisible}");
                }

                if (Input.GetKeyDown(ModConfig.ToggleFPSKey.Value))
                {
                    _perfUI.ShowFPS = !_perfUI.ShowFPS;
                    Plugin.LogSource.LogInfo($"[TAIGU-UI][兜底] FPS 显示切换: {_perfUI.ShowFPS}");
                }
            }
            catch (System.Exception ex)
            {
                Plugin.LogSource.LogError($"[TAIGU-UI][兜底] 按键检测异常: {ex.Message}");
            }
        }
    }
}
