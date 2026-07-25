using UnityEngine;

namespace TAIGU_LC_OptimizePerformance.Optimizers
{
    /// <summary>
    /// Performance monitoring utility.
    /// Tracks FPS, frame time, memory usage, draw calls, and other metrics.
    /// </summary>
    public class PerformanceMonitor
    {
        // FPS tracking
        private float _fpsUpdateInterval = 0.5f;
        private float _fpsAccumulator;
        private int _fpsFrameCount;
        private float _fpsTimeLeft;
        private float _currentFPS;
        private float _minFPS = float.MaxValue;
        private float _maxFPS;
        private float _avgFPS;
        private int _totalFrames;

        // Frame time tracking
        private float _currentFrameTime;
        private float _maxFrameTime;

        // Memory tracking
        private long _currentMemoryMB;
        private long _peakMemoryMB;

        // Stats
        private int _activeRenderers;
        private int _activeLights;
        private int _activeAudioSources;
        private int _activeParticles;

        public void Update()
        {
            UpdateFPS();
            UpdateFrameTime();
            UpdateMemoryStats();
            UpdateRenderStats();
        }

        private void UpdateFPS()
        {
            _fpsTimeLeft -= Time.unscaledDeltaTime;
            _fpsAccumulator += 1.0f / Time.unscaledDeltaTime;
            _fpsFrameCount++;
            _totalFrames++;

            if (_fpsTimeLeft <= 0f)
            {
                _currentFPS = _fpsAccumulator / _fpsFrameCount;

                if (_currentFPS < _minFPS && _currentFPS > 0) _minFPS = _currentFPS;
                if (_currentFPS > _maxFPS) _maxFPS = _currentFPS;

                // Running average
                _avgFPS = _avgFPS == 0 ? _currentFPS : (_avgFPS * 0.9f + _currentFPS * 0.1f);

                _fpsTimeLeft = _fpsUpdateInterval;
                _fpsAccumulator = 0f;
                _fpsFrameCount = 0;
            }
        }

        private void UpdateFrameTime()
        {
            _currentFrameTime = Time.unscaledDeltaTime * 1000f; // Convert to ms
            if (_currentFrameTime > _maxFrameTime) _maxFrameTime = _currentFrameTime;
        }

        private void UpdateMemoryStats()
        {
            _currentMemoryMB = System.GC.GetTotalMemory(false) / (1024 * 1024);
            if (_currentMemoryMB > _peakMemoryMB) _peakMemoryMB = _currentMemoryMB;
        }

        private void UpdateRenderStats()
        {
            // These are updated less frequently to avoid overhead
            if (Time.frameCount % 30 == 0)
            {
                _activeRenderers = GetActiveRendererCount();
                _activeLights = GetActiveLightCount();
                _activeAudioSources = GetActiveAudioSourceCount();
                _activeParticles = GetActiveParticleSystemCount();
            }
        }

        private int GetActiveRendererCount()
        {
            int count = 0;
            var renderers = Object.FindObjectsOfType<Renderer>();
            foreach (var r in renderers)
            {
                if (r != null && r.enabled && r.gameObject.activeInHierarchy)
                    count++;
            }
            return count;
        }

        private int GetActiveLightCount()
        {
            int count = 0;
            var lights = Object.FindObjectsOfType<Light>();
            foreach (var l in lights)
            {
                if (l != null && l.enabled)
                    count++;
            }
            return count;
        }

        private int GetActiveAudioSourceCount()
        {
            int count = 0;
            var sources = Object.FindObjectsOfType<AudioSource>();
            foreach (var s in sources)
            {
                if (s != null && s.isPlaying)
                    count++;
            }
            return count;
        }

        private int GetActiveParticleSystemCount()
        {
            int count = 0;
            var systems = Object.FindObjectsOfType<ParticleSystem>();
            foreach (var ps in systems)
            {
                if (ps != null && ps.IsAlive())
                    count++;
            }
            return count;
        }

        // Public accessors
        public float CurrentFPS => _currentFPS;
        public float MinFPS => _minFPS == float.MaxValue ? 0 : _minFPS;
        public float MaxFPS => _maxFPS;
        public float AvgFPS => _avgFPS;
        public float CurrentFrameTime => _currentFrameTime;
        public float MaxFrameTime => _maxFrameTime;
        public long CurrentMemoryMB => _currentMemoryMB;
        public long PeakMemoryMB => _peakMemoryMB;
        public int ActiveRenderers => _activeRenderers;
        public int ActiveLights => _activeLights;
        public int ActiveAudioSources => _activeAudioSources;
        public int ActiveParticles => _activeParticles;
        public int TotalFrames => _totalFrames;

        public void ResetStats()
        {
            _minFPS = float.MaxValue;
            _maxFPS = 0;
            _avgFPS = 0;
            _maxFrameTime = 0;
            _peakMemoryMB = 0;
            _totalFrames = 0;
        }
    }
}
