using UnityEngine;

namespace TAIGU_LC_OptimizePerformance.UI
{
    /// <summary>
    /// UI styling for the performance optimization panel.
    /// Dark theme matching Lethal Company's aesthetic.
    /// </summary>
    public static class UIStyles
    {
        private static GUIStyle _windowStyle;
        private static GUIStyle _labelStyle;
        private static GUIStyle _boldLabelStyle;
        private static GUIStyle _buttonStyle;
        private static GUIStyle _toggleStyle;
        private static GUIStyle _sliderStyle;
        private static GUIStyle _headerStyle;
        private static GUIStyle _fpsStyle;
        private static GUIStyle _sectionHeaderStyle;
        private static GUIStyle _goodStyle;
        private static GUIStyle _warnStyle;
        private static GUIStyle _badStyle;
        private static Texture2D _backgroundTexture;
        private static bool _initialized;

        public static void Init()
        {
            if (_initialized) return;

            // Create background texture
            _backgroundTexture = new Texture2D(1, 1);
            _backgroundTexture.SetPixel(0, 0, new Color(0.08f, 0.08f, 0.12f, 0.95f));
            _backgroundTexture.Apply();

            // Window style
            _windowStyle = new GUIStyle(GUI.skin.window);
            _windowStyle.normal.background = _backgroundTexture;
            _windowStyle.onNormal.background = _backgroundTexture;
            _windowStyle.padding = new RectOffset(12, 12, 12, 12);
            _windowStyle.fontSize = 13;
            _windowStyle.normal.textColor = Color.white;
            _windowStyle.fontStyle = FontStyle.Normal;

            // Label style
            _labelStyle = new GUIStyle(GUI.skin.label);
            _labelStyle.normal.textColor = new Color(0.85f, 0.85f, 0.85f);
            _labelStyle.fontSize = 12;
            _labelStyle.wordWrap = true;

            // Bold label
            _boldLabelStyle = new GUIStyle(_labelStyle);
            _boldLabelStyle.fontStyle = FontStyle.Bold;
            _boldLabelStyle.normal.textColor = Color.white;
            _boldLabelStyle.fontSize = 13;

            // Header style
            _headerStyle = new GUIStyle(_boldLabelStyle);
            _headerStyle.fontSize = 16;
            _headerStyle.normal.textColor = new Color(0.4f, 0.9f, 0.4f);
            _headerStyle.alignment = TextAnchor.MiddleCenter;

            // Section header
            _sectionHeaderStyle = new GUIStyle(_boldLabelStyle);
            _sectionHeaderStyle.fontSize = 14;
            _sectionHeaderStyle.normal.textColor = new Color(0.3f, 0.7f, 1.0f);

            // Button style
            _buttonStyle = new GUIStyle(GUI.skin.button);
            _buttonStyle.fontSize = 12;
            _buttonStyle.fontStyle = FontStyle.Bold;
            _buttonStyle.normal.textColor = Color.white;
            _buttonStyle.fixedHeight = 28;

            // Toggle style
            _toggleStyle = new GUIStyle(GUI.skin.toggle);
            _toggleStyle.normal.textColor = new Color(0.85f, 0.85f, 0.85f);
            _toggleStyle.fontSize = 12;

            // Slider style
            _sliderStyle = new GUIStyle(GUI.skin.horizontalSlider);

            // FPS counter style
            _fpsStyle = new GUIStyle(GUI.skin.box);
            _fpsStyle.fontSize = 14;
            _fpsStyle.fontStyle = FontStyle.Bold;
            _fpsStyle.alignment = TextAnchor.MiddleCenter;
            _fpsStyle.normal.background = _backgroundTexture;
            _fpsStyle.normal.textColor = Color.white;
            _fpsStyle.padding = new RectOffset(8, 8, 4, 4);

            // Status colors
            _goodStyle = new GUIStyle(_labelStyle);
            _goodStyle.normal.textColor = new Color(0.3f, 0.9f, 0.3f);

            _warnStyle = new GUIStyle(_labelStyle);
            _warnStyle.normal.textColor = new Color(1.0f, 0.8f, 0.2f);

            _badStyle = new GUIStyle(_labelStyle);
            _badStyle.normal.textColor = new Color(1.0f, 0.3f, 0.3f);

            _initialized = true;
        }

        public static GUIStyle WindowStyle => _windowStyle;
        public static GUIStyle LabelStyle => _labelStyle;
        public static GUIStyle BoldLabelStyle => _boldLabelStyle;
        public static GUIStyle ButtonStyle => _buttonStyle;
        public static GUIStyle ToggleStyle => _toggleStyle;
        public static GUIStyle SliderStyle => _sliderStyle;
        public static GUIStyle HeaderStyle => _headerStyle;
        public static GUIStyle FPSStyle => _fpsStyle;
        public static GUIStyle SectionHeaderStyle => _sectionHeaderStyle;
        public static GUIStyle GoodStyle => _goodStyle;
        public static GUIStyle WarnStyle => _warnStyle;
        public static GUIStyle BadStyle => _badStyle;

        public static GUIStyle GetFPSStyle(float fps)
        {
            var style = new GUIStyle(_fpsStyle);
            if (fps >= 60)
                style.normal.textColor = new Color(0.3f, 0.9f, 0.3f);
            else if (fps >= 30)
                style.normal.textColor = new Color(1.0f, 0.8f, 0.2f);
            else
                style.normal.textColor = new Color(1.0f, 0.3f, 0.3f);
            return style;
        }
    }
}
