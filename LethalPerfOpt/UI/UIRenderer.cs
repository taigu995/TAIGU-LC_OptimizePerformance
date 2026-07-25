using UnityEngine;
using TAIGU_LC_OptimizePerformance.Config;

namespace TAIGU_LC_OptimizePerformance.UI
{
    /// <summary>
    /// 独立的 MonoBehaviour UI 渲染器。
    /// 通过独立 GameObject 运行，确保 Update/OnGUI 被 Unity 生命周期调用。
    /// </summary>
    public class UIRenderer : MonoBehaviour
    {
        public static UIRenderer Instance { get; private set; }

        private PerformanceUI _perfUI;
        private bool _isVisible;
        private bool _initialized;
        private int _frameCount;

        // 诊断
        private bool _updateCalled;
        private bool _onGUICalled;

        public bool IsVisible => _isVisible;
        public bool IsInitialized => _initialized;

        public void Initialize(PerformanceUI perfUI)
        {
            _perfUI = perfUI;
            _initialized = true;
            Instance = this;
            Plugin.LogSource.LogInfo("[TAIGU-UI] Initialize() 已调用, _initialized=true");
        }

        private void Awake()
        {
            Plugin.LogSource.LogInfo("[TAIGU-UI] UIRenderer.Awake() 被调用");
            Instance = this;
        }

        private void OnEnable()
        {
            Plugin.LogSource.LogInfo("[TAIGU-UI] UIRenderer.OnEnable() 被调用");
        }

        private void OnDisable()
        {
            Plugin.LogSource.LogInfo("[TAIGU-UI] UIRenderer.OnDisable() 被调用");
        }

        private void Start()
        {
            Plugin.LogSource.LogInfo("[TAIGU-UI] UIRenderer.Start() 被调用 - Unity 生命周期正常");
            Plugin.LogSource.LogInfo($"[TAIGU-UI] enabled={enabled}, activeSelf={gameObject.activeSelf}, activeInHierarchy={gameObject.activeInHierarchy}");
        }

        private void Update()
        {
            if (!_initialized || _perfUI == null) return;

            if (!_updateCalled)
            {
                _updateCalled = true;
                Plugin.LogSource.LogInfo("[TAIGU-UI] Update() 首次被调用！ Keyboard.current 可用: " + (UnityEngine.InputSystem.Keyboard.current != null));
            }

            // 每 300 帧输出一次诊断日志
            _frameCount++;
            if (_frameCount % 300 == 0)
            {
                Plugin.LogSource.LogInfo($"[TAIGU-UI] Update() 运行中... 帧={_frameCount}, OnGUI={_onGUICalled}, KB={UnityEngine.InputSystem.Keyboard.current != null}, IsVisible={_isVisible}");
            }

            // 使用 Input System 检测 F5 键
            if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.f5Key.wasPressedThisFrame)
            {
                _isVisible = !_isVisible;
                _perfUI.IsVisible = _isVisible;
                Plugin.LogSource.LogInfo($"[TAIGU-UI] F5 切换 UI: {_isVisible}");
            }

            // 使用 Input System 检测 F6 键 (切换 FPS 显示)
            if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.f6Key.wasPressedThisFrame)
            {
                _perfUI.ShowFPS = !_perfUI.ShowFPS;
                Plugin.LogSource.LogInfo($"[TAIGU-UI] F6 切换 FPS: {_perfUI.ShowFPS}");
            }

            // 每帧管理光标状态
            // 当 UI 可见时，强制解锁光标以支持拖动
            if (_isVisible)
            {
                // 强制解锁光标 - 让玩家可以拖动 UI 窗口
                if (Cursor.lockState != CursorLockMode.None || !Cursor.visible)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }

        private void LateUpdate()
        {
            // LateUpdate 每帧重新强制光标状态（因为游戏自己的 Update 会重新锁定光标）
            if (_isVisible)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void OnGUI()
        {
            if (!_initialized || _perfUI == null) return;

            if (!_onGUICalled)
            {
                _onGUICalled = true;
                Plugin.LogSource.LogInfo("[TAIGU-UI] OnGUI() 首次被调用！");
            }

            if (_isVisible)
            {
                _perfUI.OnGUI();
            }
        }

        /// <summary>
        /// 由 Harmony 补丁调用的备用渲染方法
        /// </summary>
        public void RenderGUI()
        {
            if (!_initialized || _perfUI == null) return;
            if (_isVisible)
            {
                _perfUI.OnGUI();
            }
        }
    }
}