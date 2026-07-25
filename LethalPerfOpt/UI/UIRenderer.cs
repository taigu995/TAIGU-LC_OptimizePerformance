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

        // 光标状态保存
        private CursorLockMode _savedLockState = CursorLockMode.Locked;
        private bool _savedVisible = false;
        private bool _cursorStateSaved = false;

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
                ToggleUI();
            }

            // 使用 Input System 检测 F6 键 (切换 FPS 显示)
            if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.f6Key.wasPressedThisFrame)
            {
                _perfUI.ShowFPS = !_perfUI.ShowFPS;
                Plugin.LogSource.LogInfo($"[TAIGU-UI] F6 切换 FPS: {_perfUI.ShowFPS}");
            }

            // 每帧管理光标状态
            UpdateCursorState();
        }

        private void LateUpdate()
        {
            // LateUpdate 再次强制光标状态（游戏可能在自己的 Update 中重新锁定）
            UpdateCursorState();
        }

        /// <summary>
        /// 切换 UI 显示状态
        /// </summary>
        private void ToggleUI()
        {
            _isVisible = !_isVisible;
            _perfUI.IsVisible = _isVisible;

            if (_isVisible)
            {
                // 显示 UI：保存当前光标状态，然后解锁
                if (!_cursorStateSaved)
                {
                    _savedLockState = Cursor.lockState;
                    _savedVisible = Cursor.visible;
                    _cursorStateSaved = true;
                    Plugin.LogSource.LogInfo($"[TAIGU-UI] 保存光标状态: lockState={_savedLockState}, visible={_savedVisible}");
                }
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                // 隐藏 UI：恢复保存的光标状态
                if (_cursorStateSaved)
                {
                    Cursor.lockState = _savedLockState;
                    Cursor.visible = _savedVisible;
                    _cursorStateSaved = false;
                    Plugin.LogSource.LogInfo($"[TAIGU-UI] 恢复光标状态: lockState={_savedLockState}, visible={_savedVisible}");
                }
            }

            Plugin.LogSource.LogInfo($"[TAIGU-UI] F5 切换 UI: {_isVisible}");
        }

        /// <summary>
        /// 更新光标状态：UI 可见时强制解锁，隐藏时由游戏控制
        /// </summary>
        private void UpdateCursorState()
        {
            if (_isVisible)
            {
                // 强制解锁光标 - 让玩家可以拖动 UI 窗口
                if (Cursor.lockState != CursorLockMode.None && Cursor.visible)
                {
                    // 光标已经被解锁，无需操作
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
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
                // OnGUI 中再次强制解锁光标（确保 IMGUI 事件能正确处理鼠标）
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

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